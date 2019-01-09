using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using PathFinderNameSpace;

namespace ChunkScaner
{
    public class ChunkScaner : MonoBehaviour
    {
        public string id;
        public List<List<int>> bitMap = new List<List<int>>();
        public string[] direction = new string[4];

        [SerializeField] private GameObject map;
        [SerializeField] private string path;

        private bool stopCheckPosition = false;

        private void Start()
        {
            map.transform.position = new Vector3(4.5f, 0f, 4.5f);  
            gameObject.transform.position = new Vector3(0f, 1f, 0f);

            for (int i = 0; i < 10; i++)
            {
                bitMap.Add(new List<int>());
                for (int j = 0; j < 10; j++)
                {
                    bitMap[i].Add(new int());
                }
            }

            StartCoroutine(CheckPositions(10, 10));
        }

        private IEnumerator CheckPositions(int width, int hight)
        {
            for (int z = 0; z < hight; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    gameObject.transform.position = new Vector3(x, 1f, z);
                    stopCheckPosition = false;
                    for (int i = 0; i < 3 && (!stopCheckPosition); i++)
                    {
                        yield return null;
                    }
                }
            }
            GetMapInfo();

          //  for (int x = 0; x < bitMap.Count; x++)
          //  {
         //       bitMap[x].Reverse();
          //  }

            bitMap.Reverse();

            File.WriteAllText(path + id + ".json", JsonConvert.SerializeObject(new Chunk(id, bitMap, direction), Formatting.None));


        }

        private void GetMapInfo()
        {
            id = map.gameObject.name;
            direction = map.gameObject.name.Substring(map.gameObject.name.Length - 7).Split('_');
        }



        private void OnCollisionStay(Collision collision)
        {
            bitMap[Convert.ToInt32(Math.Ceiling(gameObject.transform.position.z))][Convert.ToInt32(Math.Ceiling(gameObject.transform.position.x))] = 1;
            stopCheckPosition = true;
        }
    }
}
