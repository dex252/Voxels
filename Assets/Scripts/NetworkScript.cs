using System;

using UnityEngine;
using UnityEngine.Networking;

namespace NetWork
{
    [Serializable]
    public class Snap
    {
        [SerializeField] public Vector3 position = Vector3.zero;
        [SerializeField] public Vector3 eulerAngles = Vector3.zero;
        [SerializeField] public int id = 0;

        [SerializeField] private float receiveTime = 0;
        [SerializeField] private bool outDated = false;

        public Snap()
        {
            position = new Vector3(0 , 0 , 0);
            eulerAngles = new Vector3(0 , 0 , 0);
        }

        public Snap(Vector3 position , Vector3 eulerAngles)
        {
            this.position = position;
            this.eulerAngles = eulerAngles;
        }

        public Snap(Snap snap)
        {
            position = snap.position;
            eulerAngles = snap.eulerAngles;
            receiveTime = snap.receiveTime;
            outDated = snap.outDated;
            id = snap.id;
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public Vector3 EulerAngles
        {
            get
            {
                return eulerAngles;
            }

            set
            {
                eulerAngles = value;
            }
        }

        public float ReceiveTime
        {
            get
            {
                return receiveTime;
            }

            set
            {
                receiveTime = value;
            }
        }

        public bool OutDated
        {
            get
            {
                return outDated;
            }

            set
            {
                outDated = value;
            }
        }
    }

    public class NetworkScript : NetworkBehaviour
    {
        public Snap oldServerSnap;
        public Snap newServerSnap;
        private float oldSyncTime;
        private float newSyncTime;
        private Vector2 lastInput;
        [SerializeField] private float lerpConst;
        [SerializeField] private float sendRate=4;
        [SerializeField] private int tactId;

        [SerializeField] private float trashHoldDistance;
        [SerializeField] private float trashHoldAngel;
        [SerializeField] private float maxTankSpeed;
        [SerializeField] private float tankSpeed;

        private float ping;
        private float moveCoolDown;

        [SerializeField] private float incSpeed;

        private float RoundUp(float value)
        {
            if (value > 0)
            {
                return Mathf.Ceil(value);
            }
            else
            {
                return Mathf.Floor(value);
            }
        }

        private float RoundDown(float value)
        {
            if (value < 0)
            {
                return Mathf.Ceil(value);
            }
            else
            {
                return Mathf.Floor(value);
            }
        }

        private float PointsAnglesDiff(Vector3 firstPoint , Vector3 secondPoint)
        {
            float scalarMult = firstPoint.x * secondPoint.x
                + firstPoint.y * secondPoint.y
                + firstPoint.z * secondPoint.z;
            float modulSum = firstPoint.x * firstPoint.x + firstPoint.y * firstPoint.y +
                firstPoint.z * firstPoint.z;
            modulSum += secondPoint.x * secondPoint.x + secondPoint.y * secondPoint.y +
                secondPoint.z * secondPoint.z;
            return (Mathf.Acos(scalarMult / modulSum) * 180 / Mathf.PI);
        }

        private float AnglesDiff(float firstAngle , float secondtAngle)
        {
            firstAngle = firstAngle - RoundDown(firstAngle / 360f) * 360f;
            secondtAngle = secondtAngle - RoundDown(secondtAngle / 360f) * 360f;

            if (firstAngle < 0)
            {
                firstAngle = 360 + firstAngle;
            }

            if (secondtAngle < 0)
            {
                secondtAngle = 360 + secondtAngle;
            }

            return Mathf.Abs(firstAngle - secondtAngle);
        }

        private Vector3 AnglesDiff(Vector3 firstAngeles , Vector3 secondAngeles)
        {
            return (new Vector3(AnglesDiff(firstAngeles.x , secondAngeles.x) ,
                AnglesDiff(firstAngeles.y , secondAngeles.y) ,
                AnglesDiff(firstAngeles.z , secondAngeles.z)));
        }

        private void UpdateSnap()
        {
            oldServerSnap = new Snap(newServerSnap)
            {
                OutDated = true
            };

            newServerSnap.Position = transform.position;
            newServerSnap.EulerAngles = transform.eulerAngles;
            newServerSnap.OutDated = false;
        }

        [Command]
        private void CmdMoveObjectOnServer(float x , float z)
        {
            MoveObject(x , z);
        }

        private void MoveObject(float x , float z)
        {
            if (moveCoolDown < Time.fixedDeltaTime)
            {
                Vector2 curInput = new Vector2(RoundUp(x) , RoundUp(z));
                float y = -Mathf.Atan2(z , x) * 180 / Mathf.PI;

                if ((curInput.x != 0) || (curInput.y != 0))
                {
                    transform.eulerAngles = new Vector3(0 , y , 0);
                }

                //Lerp(transform.position , transform.position , transform.eulerAngles , new Vector3(0 , y , 0) , Time.time , Time.time + Time.fixedDeltaTime , false);
                if (lastInput == curInput)
                {
                    tankSpeed = Mathf.Min(tankSpeed + incSpeed , maxTankSpeed);
                    transform.Translate(tankSpeed , 0 , 0);
                }
                else
                {
                    tankSpeed = 0;
                }


                lastInput = new Vector2 (curInput.x,curInput.y);
                moveCoolDown += Time.fixedDeltaTime;
            }
        }

        public void Move(float x , float z)
        {
            if (!isServer)
            {
                //prediction
            }
            CmdMoveObjectOnServer(Input.GetAxis("Horizontal") , Input.GetAxis("Vertical"));
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                tactId++;
                moveCoolDown -= Time.fixedDeltaTime;

                if (tactId % sendRate == 0)
                {
                    if ((Vector3.Distance(transform.position , newServerSnap.Position) > trashHoldDistance) ||
                        (AnglesDiff(newServerSnap.EulerAngles.x , transform.eulerAngles.x) > trashHoldAngel) ||
                        (AnglesDiff(newServerSnap.EulerAngles.y , transform.eulerAngles.y) > trashHoldAngel) ||
                        (AnglesDiff(newServerSnap.EulerAngles.z , transform.eulerAngles.z) > trashHoldAngel))
                    {
                        UpdateSnap();
                        newServerSnap.id = oldServerSnap.id + 1;
                    }
                    RpcGetSnap(newServerSnap);
                }
            }
        }


        [ClientRpc]
        private void RpcGetSnap(Snap newSnap)
        {
            if (isServer)
            {
                return;
            }

            oldSyncTime = newSyncTime;
            newSyncTime = Time.time;

            if (newSnap.id != newServerSnap.id)
            {
                oldServerSnap = new Snap(newServerSnap)
                {
                    OutDated = true ,
                    ReceiveTime = oldSyncTime
                };

                newServerSnap = new Snap(newSnap)
                {
                    OutDated = false ,
                    ReceiveTime = newSyncTime
                };
            }
        }

        private bool Lerp(Vector3 oldPos , Vector3 newPos , Vector3 oldEuler , Vector3 newEuler , float start , float finish , bool compressRotation)
        {
            float angelX = AnglesDiff(transform.eulerAngles.x , newEuler.x);
            float angelY = AnglesDiff(transform.eulerAngles.y , newEuler.y);
            float angelZ = AnglesDiff(transform.eulerAngles.z , newEuler.z);

            float lerpTime = (Time.time - start)
                       / ((finish - start) * lerpConst);

            if ((Vector3.Distance(transform.position , newPos) < trashHoldDistance) &&
                    (angelX < trashHoldAngel) && (angelY < trashHoldAngel) && (angelZ < trashHoldAngel))
            {
                return false;
            }

            transform.position = Vector3.Lerp(oldPos , newPos , lerpTime);

            float x, y, z;

            if (compressRotation)
            {
                x = newEuler.x;
                y = newEuler.y;
                z = newEuler.z;
            }
            else
            {
                x = Mathf.LerpAngle(oldEuler.x , newEuler.x , lerpTime);
                y = Mathf.LerpAngle(oldEuler.y , newEuler.y , lerpTime);
                z = Mathf.LerpAngle(oldEuler.z , newEuler.z , lerpTime);
            }

            transform.eulerAngles = new Vector3(x , y , z);

            return true;
        }

        private void Update()
        {
            if (isServer)
            {
                return;
            }

            if (!newServerSnap.OutDated)
            {
                newServerSnap.OutDated = !Lerp(oldServerSnap.Position , newServerSnap.Position , oldServerSnap.EulerAngles , newServerSnap.EulerAngles ,
                newServerSnap.ReceiveTime , 2 * newServerSnap.ReceiveTime - oldServerSnap.ReceiveTime , true);
            }
        }
    }
}
