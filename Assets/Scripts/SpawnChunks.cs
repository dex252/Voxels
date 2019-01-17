using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGeneratorNameSpace;

public class SpawnChunks : MonoBehaviour
{
    [SerializeField] private float dist;
    [SerializeField] private GameObject loadScreen;

    private MapGeneratorV05 t;
    private bool spawnSecurity=false;

    public void Start()
    {
        t = gameObject.GetComponent<MapGeneratorV05>();
        StartBuildMap(t.map);
        loadScreen.SetActive(false);
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
}
