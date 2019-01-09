using UnityEngine;
using UnityEngine.Networking;

public class PlayerGui : NetworkBehaviour
{
    private NetworkStartPosition[] spawnPoints;
    private Vector3 lastMousePosition;
    [SerializeField] private Camera camera;
    [SyncVar]
    private GameObject spawnedTank;
    [SerializeField] private Vector3 offSet;

    [Command]
    private void CmdSpawn(string TankName)
    {
        GameObject tankModel = Resources.Load<GameObject>("TanksModels/" + TankName);

        Vector3 spawnPoint = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0 , spawnPoints.Length)].transform.position;
        }

        GameObject spawnObject = Instantiate(tankModel , spawnPoint , Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(spawnObject , connectionToClient);
        spawnedTank = spawnObject;

        spawnedTank.GetComponent<ObjectStatus.ObjectStatusScript>().Parent = transform;
    }

    private void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    private void OnGUI()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (GUI.Button(new Rect(Screen.width / 2 , 20 , 100 , 100) , "Spawn"))
        {
            CmdSpawn("TankBaseModel");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 newEulerAngles = camera.transform.eulerAngles + new Vector3(lastMousePosition.y - Input.mousePosition.y ,
                -lastMousePosition.x + Input.mousePosition.x ,
                0);
            newEulerAngles.x = Mathf.Min(90 , newEulerAngles.x);


            newEulerAngles.x = Mathf.Max(0 , newEulerAngles.x);


            camera.transform.eulerAngles = newEulerAngles;

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                offSet.x += Input.GetAxis("Horizontal");
                offSet.z+= Input.GetAxis("Vertical");
            }
        }

        if (Input.GetKey(KeyCode.Equals))
        {
            offSet.y++;
        }


        if (Input.GetKey(KeyCode.Minus))
        {
            offSet.y--;
        }

        lastMousePosition = Input.mousePosition;

        if (spawnedTank != null)
        {
            camera.transform.position = spawnedTank.transform.position + offSet;
        }

    }
}
