using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGeneratorNameSpace;
using PathFinderNameSpace;

public class SpawnChunks : MonoBehaviour
{
    [SerializeField] private float dist;
    [SerializeField] private GameObject loadScreen;

    public GameObject bot;

    private MapGeneratorV05 t;
    private bool spawnSecurity=false;

    public void Start()
    {
        t = gameObject.GetComponent<MapGeneratorV05>();
        StartBuildMap(t.map);
        StartCoroutine(spawn(new Vector3(0f, 0.515442f, 0)));
    }


    public void StartBuildMap(List<List<string>> map)
    {
        Debug.Log("Генерация");
        int i = 0;
        for (int x = 0; x < map.Count; x++)
        {
            for (int y = 0; y < map[x].Count; y++)
            {
                i++;
                GameObject chunk = Resources.Load<GameObject>("Chunk2/"+map[x][y]);
                GameObject spawnObject = Instantiate(chunk, new Vector3(x * dist, 0, y * dist), Quaternion.identity);
            }
        }
        t.convertMagistralName();
    }

    IEnumerator spawn(Vector3 spawnPoint)
    {
        for (int i = 0; i < 100; i++)
        {
        var botos = Instantiate(bot, spawnPoint, Quaternion.identity).GetComponent<PathFinder>();
        botos.mapWay.GetChunkMap(t.map);
            for (int y = 0; y < 1000; y++)
            {
                yield return null;
            }
           
        }
    }
}
