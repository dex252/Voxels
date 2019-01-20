using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shout : MonoBehaviour
{
    public BulletScript2 bulet;
    public GameObject body;
    public GameObject head;
    public GameObject builletPref;
    public GameObject gunPit;
    public double reloadTime;

    private RotateHead headScript;
    // private Transform bodyTransform;
    private Transform gunPitTransform;
    private double time = 0;
    // Start is called before the first frame update
    void Start()
    {
        headScript = head.GetComponent<RotateHead>();
        //    bodyTransform= body.GetComponent<Transform>();
        gunPitTransform = gunPit.GetComponent<Transform>();
    }

    void Update()
    {
        time += Time.deltaTime;
        //bool mouseButtonDown = Input.GetMouseButtonDown(0);
        //if (mouseButtonDown && time > reloadTime )
        //{
        //    time = 0;
        //    ShoutStart();
        //}
        if (Input.GetKey(KeyCode.End) && time > reloadTime)
        {
            time = 0;
            ShoutStart();
        }
    }

    private void ShoutStart()
    {
        //  var buletPoint = Instantiate(builletPref, new Vector3(bodyTransform.position.x, 1, bodyTransform.position.z), Quaternion.identity).GetComponent<BulletScript2>();
        //Physics.IgnoreCollision(body.GetComponent<Collider>(), buletPoint.GetComponent<Collider>());
        var buletPoint = Instantiate(builletPref, new Vector3(gunPitTransform.position.x, 1, gunPitTransform.position.z), Quaternion.identity).GetComponent<BulletScript2>();
        headScript.Remath();
        Vector3 rotationOfBuild = headScript.lastRotate;
        buletPoint.FireProjectile(rotationOfBuild, "Player1");//процедура скрипта в снаряте
    }
}