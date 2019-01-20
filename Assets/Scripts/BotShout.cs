using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotShout : MonoBehaviour
{
    [SerializeField] GameObject shoutEffect;

    public GameObject tower;
    public GameObject builletPref;
    public GameObject gunPit;
    public double reloadTime;

    private RoateGunBot towerScript;
    private Transform gunPitTransform;
    private RayScan rayScan;
    private double time;
    BotShout botShout;

    void Start()
    {
        rayScan = tower.GetComponent<RayScan>();
        towerScript = tower.GetComponent<RoateGunBot>();
        gunPitTransform = gunPit.GetComponent<Transform>();
        botShout = this.gameObject.GetComponent<BotShout>();
        botShout.time = 0;
    }
        
    void Update()
    {
        botShout.time += Time.deltaTime;
        if (botShout.time > reloadTime && rayScan.ray.result == true )
        {
            botShout.time = 0;
            botShout.ShoutStart();
        }

    }

    private void ShoutStart()
    {
        var buletPoint = Instantiate(builletPref, new Vector3(gunPitTransform.position.x, 1, gunPitTransform.position.z), Quaternion.identity).GetComponent<BulletScript2>();
        Destroy(Instantiate(shoutEffect, gunPit.transform.position, Quaternion.FromToRotation(Vector3.forward, gunPit.transform.position)) as GameObject, 1f);
        towerScript.Remath();
        Vector3 rotationOfBuild = towerScript.lastRotate;
        buletPoint.FireProjectile(rotationOfBuild, "Bot");
    }
}