using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoateGunBot : MonoBehaviour
{
    [SerializeField] GameObject detecktor;
    public Vector3 lastRotate;

    private ScanSphere scanSphere;

    const float speedOfRotate = 5f;

    Transform tower;

    private void Awake()
    {
        scanSphere = detecktor.GetComponent<ScanSphere>();
        lastRotate = new Vector3(0, 0, 0);
        tower = GetComponent<Transform>();

    }

    void Update()
    {
        if (scanSphere.players.Count > 0)
        {

            Vector3 playerPosition;

            playerPosition = scanSphere.players[0].transform.position;
            var targetRotation = Quaternion.LookRotation(playerPosition - this.transform.position);
            targetRotation.x = 0f;
            targetRotation.z = 0f;
            transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, speedOfRotate * Time.deltaTime);
            
        }
    }

    public void Remath()
    {
        lastRotate = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
    }
}
