using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Snap
{
    public Vector3 position = Vector3.zero;
    public Vector3 eulerAngles = Vector3.zero;
    public int id;

    private float receiveTime;
    private bool outDated = false;

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
    public int snapId = 0;
    [SerializeField] private float trashHoldDistance;
    [SerializeField] private float trashHoldAngel;

    [SerializeField] private float tankSpeed;

    [SyncVar]
    public Snap newServerSnap = new Snap();

    [SyncVar]
    public Snap oldServerSnap = new Snap();

    private float ping;
    private float moveCoolDown;

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
        if (moveCoolDown < Time.fixedDeltaTime)
        {
            x = RoundUp(x);
            z = RoundUp(z);

            if ((x != 0) || (z != 0))
            {
                transform.eulerAngles = new Vector3(0, -Mathf.Atan2(z, x) * 180 / Mathf.PI, 0);
            }

            transform.Translate(tankSpeed, 0, 0);
            moveCoolDown += Time.fixedDeltaTime;
        }
    }

    public void Move(float x, float z)
    {
        if (!isServer)
        {
            MoveTank(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        CmdMoveTank(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            moveCoolDown -= Time.deltaTime;
        }
    }

    void OnSnapUpdate(Snap it)
    {
        newServerSnap.ReceiveTime = Time.time;
    }

    private void Update()
    {
        Debug.Log(oldServerSnap.ReceiveTime + " " + newServerSnap.ReceiveTime+" "+newServerSnap.id);

        if (isServer)
        {
            SyncSnap();
            snapId++;
            newServerSnap.id = snapId;
            return;
        }
        
        var angelX = AnglesDiff(transform.eulerAngles.x, newServerSnap.EulerAngles.x);
        var angelY = AnglesDiff(transform.eulerAngles.y, newServerSnap.EulerAngles.y);
        var angelZ = AnglesDiff(transform.eulerAngles.z, newServerSnap.EulerAngles.z);

        if ((Vector3.Distance(transform.position, newServerSnap.Position) < trashHoldDistance) &&
                (angelX < trashHoldAngel) && (angelY < trashHoldAngel) && (angelZ < trashHoldAngel))
        {
            newServerSnap.OutDated = true;
        }

        float lerpTime = (Time.time - newServerSnap.ReceiveTime)
                        / (newServerSnap.ReceiveTime - oldServerSnap.ReceiveTime);

        if (!newServerSnap.OutDated)
        {
            transform.position = Vector3.Lerp(oldServerSnap.Position, newServerSnap.Position, lerpTime);
            float x = Mathf.LerpAngle(oldServerSnap.EulerAngles.x, newServerSnap.EulerAngles.x, lerpTime);
            float y = Mathf.LerpAngle(oldServerSnap.EulerAngles.y, newServerSnap.EulerAngles.y, lerpTime);
            float z = Mathf.LerpAngle(oldServerSnap.EulerAngles.z, newServerSnap.EulerAngles.z, lerpTime);

            transform.eulerAngles = new Vector3(x, y, z);
            Debug.Log(oldServerSnap.ReceiveTime + " " + newServerSnap.ReceiveTime+ " " + newServerSnap.id);
        }
    }
}
