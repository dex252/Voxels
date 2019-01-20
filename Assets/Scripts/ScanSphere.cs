using System.Collections.Generic;
using UnityEngine;

public class ScanSphere : MonoBehaviour
{

    public List<GameObject> players;
    public ScanSphere scan;
    private GameObject player1;
    private GameObject player2;

    private void Awake()    
    {
      
        scan = this.gameObject.GetComponent<ScanSphere>();
        scan.player1 = GameObject.FindGameObjectWithTag("Player1");
        scan.player2 = GameObject.FindGameObjectWithTag("Player2");
        scan.players = new List<GameObject>();
    }
    

    void OnTriggerEnter(Collider coll)
    {

        GameObject player = coll.gameObject;

        if (player.tag == "Player1")
        {
            scan.players.Add(player1);
        }

        if (player.tag == "Player2")
        {
            scan.players.Add(player2);
        }
    }


    void OnTriggerExit(Collider coll)
    {
        
        GameObject player = coll.gameObject;

        if (player.tag == "Player1")
        {
            scan.players.Remove(player1);
        }
        if (player.tag == "Player2")
        {
            scan.players.Remove(player2);
        }
    }
}
