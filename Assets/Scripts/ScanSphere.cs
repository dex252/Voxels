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
      
        scan = gameObject.GetComponent<ScanSphere>();
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
           // System.IO.File.AppendAllText(@"D:\logs.txt", "Maincount = " + scan.players.Count + System.Environment.NewLine);
        }

        if (player.tag == "Player2")
        { 
            scan.players.Add(player2);
           // System.IO.File.AppendAllText(@"D:\logs.txt", "Maincount = " + scan.players.Count + System.Environment.NewLine);
        }

      //  player = null;
    }


    void OnTriggerExit(Collider coll)
    {
        
        GameObject player = coll.gameObject;

        if (player.tag == "Player1")
        {
            scan.players.Remove(player1);
          //  System.IO.File.AppendAllText(@"D:\logs.txt", "Fullycount = " + scan.players.Count + System.Environment.NewLine);
        }
        if (player.tag == "Player2")
        {
            scan.players.Remove(player2);
          //  System.IO.File.AppendAllText(@"D:\logs.txt", "Fullycount = " + scan.players.Count + System.Environment.NewLine);
        }

       // player = null;
    }
}
