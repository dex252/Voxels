using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayScan : MonoBehaviour
{
    private GameObject target1;
    private GameObject target2;

    public bool result;
    public RayScan ray;

    // Start is called before the first frame update
    private void Awake()
    {
        ray = gameObject.GetComponent<RayScan>();
        ray.result = false;
        ray.target1 = GameObject.FindGameObjectWithTag("Player1");
        ray.target2 = GameObject.FindGameObjectWithTag("Player2");
    }

    private void OnTriggerEnter(Collider enemy)
    {
        GameObject player = enemy.gameObject;
      //  System.IO.File.AppendAllText(@"D:\logs.txt", "Maincount = " + player.tag + System.Environment.NewLine);

        if (player.tag == "Player1" || player.tag == "Player2")
        {
            ray.result = true;
        }
    }

    private void OnTriggerExit(Collider enemy)
    {
        GameObject player = enemy.gameObject;
      //  System.IO.File.AppendAllText(@"D:\logs.txt", "Maincount = " + player.tag + System.Environment.NewLine);

        if (player.tag == "Player1" || player.tag == "Player2")
        {
            ray.result = false;
        }
    }
}
