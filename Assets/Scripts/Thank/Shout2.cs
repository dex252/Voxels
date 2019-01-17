using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shout2 : MonoBehaviour
{
    public BulletScript2 bulet;
    public GameObject body;
    public GameObject head;
    public GameObject builletPref;
    public GameObject gunPit;
    public double reloadTime;

    private RotateHead2 headScript;
   // private Transform bodyTransform;
    private Transform gunPitTransform;
    private double time = 0;
    // Start is called before the first frame update
    void Start()
    {
        headScript = head.GetComponent<RotateHead2>();
      //  bodyTransform = body.GetComponent<Transform>();
        gunPitTransform = gunPit.GetComponent<Transform>();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (Input.GetKey(KeyCode.X) && time > reloadTime)
        {
            time = 0;
            ShoutStart();
        }
    }

    private void ShoutStart()
    {
       // var buletPoint = Instantiate(builletPref, new Vector3(bodyTransform.position.x, 1, bodyTransform.position.z), Quaternion.identity).GetComponent<BulletScript2>();
        //Physics.IgnoreCollision(body.GetComponent<Collider>(), buletPoint.GetComponent<Collider>());
        var buletPoint = Instantiate(builletPref, new Vector3(gunPitTransform.position.x, 1, gunPitTransform.position.z), Quaternion.identity).GetComponent<BulletScript2>();
        headScript.Remath();
        Vector3 rotationOfBuild = headScript.lastRotate;
        buletPoint.FireProjectile(rotationOfBuild);//процедура скрипта в снаряте
    }
}
