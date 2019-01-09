using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGeneratorNameSpace;
using PathFinderNameSpace;
using System.IO;

public class MapController : MonoBehaviour
{
    MapGeneratorV05 mapGenerator;
    PathFinder pathFinder;
	
	void Start ()
    {
        List<Vector2> way = new List<Vector2>();
        List<Vector2> road = new List<Vector2>();

        mapGenerator = gameObject.GetComponent<MapGeneratorV05>();
        pathFinder = gameObject.GetComponent<PathFinder>();

        mapGenerator.GenerateParams = new GenerateParams(10,10,15,15,70,10);

        mapGenerator.Generate();

        var map = mapGenerator.Map;

        pathFinder.mapWay.GetChunkMap(map);

        way = pathFinder.mapWay.FindWay(new Vector2(4,1), new Vector2(6,7));

        Vector2 start = new Vector2();
        start.x = 5;
        start.y = 5;

        if (way.Count != 0)
        {
            road = pathFinder.mapWay.FindRoad(way, start);
        }

        if (way.Count == 0)
        {
            Debug.Log("Путь не найден");
        }

        foreach (var chunk in way)
        {
            Debug.Log($"{chunk} LocalWayNum = {way.IndexOf(chunk)}");
        }

        foreach (var chunk in road)
        {
            Debug.Log($"{chunk} WorldRoadNum = {road.IndexOf(chunk)}");
        }

        StreamWriter sr = new StreamWriter("D://text2.txt");

        string text = "";

        bool s = false;

        for (int x = 0; x < map.Count; x++)
        {
            text = "";
            for (int y = 0; y < map[x].Count; y++)
            {
                s = false;

                foreach (var chunk in way)
                {
                    if (chunk.x == x && chunk.y == y)
                    {
                        s = true;
                    }
                }
                if (s)
                {
                    text += "#";
                }
                else
                {
                    text += "0";
                    s = false;
                }
            }
            sr.WriteLine(text);
        }
        sr.Close();
    }
	
}
