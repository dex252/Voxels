using System;
using UnityEngine;
using UnityEngine.Networking;

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
        this.position = new Vector3(0, 0, 0);
        this.eulerAngles = new Vector3(0, 0, 0);
    }

    public Snap(Vector3 position, Vector3 eulerAngles)
    {
        this.position = position;
        this.eulerAngles = eulerAngles;
    }

    public Snap(Snap snap)
    {
        this.position = snap.position;
        this.eulerAngles = snap.eulerAngles;
        this.receiveTime = snap.receiveTime;
        this.outDated = snap.outDated;
        this.id = snap.id;
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
    [SerializeField] private float lerpConst;

    public float trashHoldDistance;
    public float trashHoldAngel;
    public float tankSpeed;
    private float ping;
    private float moveCoolDown;

    private NetworkManager manager;
    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<NetworkManager>();
    }
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

    private float VectorsAnglesDiff(Vector3 firstPoint, Vector3 secondPoint)
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

    private float AnglesDiff(float firstAngle, float secondtAngle)
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

    private void SyncSnap()
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
    private void CmdMoveTank(float x, float z)
    {
        MoveTank(x, z);
    }

    private void MoveTank(float x, float z)
    {
        x = RoundUp(x);
        z = RoundUp(z);
        float y = -Mathf.Atan2(z, x) * 180 / Mathf.PI;

        if ((x != 0) || (z != 0))
        {
            transform.eulerAngles = new Vector3(0, y, 0);
        }

        transform.Translate(tankSpeed, 0, 0);

        moveCoolDown += Time.fixedDeltaTime;

    }

    public void Move(float x, float z)
    {
        if (!isServer)
        {
            //MoveTank(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        CmdMoveTank(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            moveCoolDown -= Time.deltaTime;
            if ((Vector3.Distance(transform.position, newServerSnap.Position) > trashHoldDistance) ||
                (newServerSnap.EulerAngles.x > trashHoldAngel) || (newServerSnap.EulerAngles.y > trashHoldAngel) || (newServerSnap.EulerAngles.z > trashHoldAngel))
            {
                SyncSnap();
                newServerSnap.id = oldServerSnap.id + 1;
            }          
            RpcGetSnap(newServerSnap);
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
            oldServerSnap = new Snap(newServerSnap);
            newServerSnap = new Snap(newSnap);
            oldServerSnap.ReceiveTime = oldSyncTime;
            newServerSnap.ReceiveTime = newSyncTime;
            oldServerSnap.OutDated = true;
            newServerSnap.OutDated = false;
        }

        Debug.Log($"RpcGetSna:Координата старого снепа {oldServerSnap.Position} Время старого снепа {oldServerSnap.ReceiveTime} " +
                        $"Координата нового снепа {newServerSnap.Position} Время нового снепа {newServerSnap.ReceiveTime} = {Time.time}");
    }

    private bool DoLerpStep(Vector3 oldPos, Vector3 newPos, Vector3 oldEuler, Vector3 newEuler, float start, float finish)
    {
        var angelX = AnglesDiff(transform.eulerAngles.x, newEuler.x);
        var angelY = AnglesDiff(transform.eulerAngles.y, newEuler.y);
        var angelZ = AnglesDiff(transform.eulerAngles.z, newEuler.z);

        float lerpTime = (Time.time - start)
                   / ((finish - start) * lerpConst);
        //Debug.Log($"LerpTime {lerpTime}");

        if ((Vector3.Distance(transform.position, newPos) < trashHoldDistance) &&
                (angelX < trashHoldAngel) && (angelY < trashHoldAngel) && (angelZ < trashHoldAngel))
        {
            return false;
        }


        transform.position = Vector3.Lerp(oldPos, newPos, lerpTime);

        float x = Mathf.LerpAngle(oldEuler.x, newEuler.x, lerpTime);
        float y = Mathf.LerpAngle(oldEuler.y, newEuler.y, lerpTime);
        float z = Mathf.LerpAngle(oldEuler.z, newEuler.z, lerpTime);

        transform.eulerAngles = new Vector3(x, y, z);

        return true;
    }

    private void Update()
    {
        if (isServer)
        {
            return;
        }

        //Debug.Log($"Update:Координата старого снепа {oldServerSnap.Position} Время старого снепа {oldServerSnap.ReceiveTime} " +
        //                 $"Координата нового снепа {newServerSnap.Position} Время нового снепа {newServerSnap.ReceiveTime}= {Time.time}");


        if (!newServerSnap.OutDated)
        {
            newServerSnap.OutDated = !DoLerpStep(oldServerSnap.Position, newServerSnap.Position, oldServerSnap.EulerAngles, newServerSnap.EulerAngles,
            newServerSnap.ReceiveTime, 2 * newServerSnap.ReceiveTime - oldServerSnap.ReceiveTime);
        }
    }
}
