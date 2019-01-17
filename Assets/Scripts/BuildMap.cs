using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BuildMap : NetworkBehaviour {

    [SerializeField] private float dist;

    public void StartBuildMap(List<List<string>> map)
    {
        for (int x = 0; x < map.Count; x++)
        {
            for (int y = 0; y < map.Count; y++)
            {
                var chunk = Resources.Load<GameObject>(map[x][y]);
                var spawnObject = Instantiate(chunk , new Vector3(x*dist,0,y*dist) , Quaternion.identity);
                NetworkServer.Spawn(spawnObject);
            }
        } 
    }
}
