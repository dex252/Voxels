using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

namespace PathFinderNameSpace
{

    [Serializable]
    public class Chunk
    {

        [SerializeField] public string id;
        [SerializeField] public List<List<int>> bitMap;
        [SerializeField] public string[] direction;

        public Chunk(string id, List<List<int>> bitMap, string[] direction)
        {
            this.id = id;
            this.bitMap = bitMap;
            this.direction = direction;
        }
        public Chunk()
        {

        }

    }

    public class MapWays
    {
        private List<List<Chunk>> chunksMap;

        public List<List<Chunk>> ChunksMap
        {
            get
            {
                return chunksMap;
            }

            set
            {
                chunksMap = value;
            }
        }

        public void GetChunkMap(List<List<string>> map)
        {
            ChunksMap = new List<List<Chunk>>();
            for (int x = 0; x < map.Count; x++)
            {
                ChunksMap.Add(new List<Chunk>());
                for (int y = 0; y < map[x].Count; y++)
                {
                    string infoFile = File.ReadAllText(Application.dataPath + "/Resources/ChunksInfo/" + map[x][y] + ".json");
                    ChunksMap[x].Add(new Chunk());
                    ChunksMap[x][y] = JsonConvert.DeserializeObject<Chunk>(infoFile);
                }
            }
        }

        private class bfsChunkInf
        {
            private Vector2 lastPosition;
            private long wayLength;

            public bfsChunkInf()
            {
                lastPosition = new Vector2(-1, -1);
                wayLength = -1;
            }

            public Vector2 LastPosition
            {
                get
                {
                    return lastPosition;
                }

                set
                {
                    lastPosition = value;
                }
            }

            public long WayLength
            {
                get
                {
                    return wayLength;
                }

                set
                {
                    wayLength = value;
                }
            }
        }

        public List<Vector2> FindWay(Vector2 start, Vector2 finish)
        {

            List<Vector2> way = new List<Vector2>();
            bfsChunkInf[,] bfsMatrix = new bfsChunkInf[ChunksMap.Count, ChunksMap[0].Count];
            for (int x = 0; x < ChunksMap.Count; x++)
            {
                for (int y = 0; y < ChunksMap[x].Count; y++)
                {
                    bfsMatrix[x, y] = new bfsChunkInf();
                }
            }
            List<Vector2> visitChunks = new List<Vector2>();
            bool isFind = false;
            int iteration = 0;

            visitChunks.Add(start);
            bfsMatrix[(int)start.x, (int)start.y].WayLength = 0;
            bfsMatrix[(int)start.x, (int)start.y].LastPosition = new Vector2(start.x, start.y);

            while ((visitChunks.Count > 0) && (!isFind))
            {

                iteration++;
                Vector2 visit = visitChunks[0];
                Chunk directChunk = chunksMap[(int)visit.x][(int)visit.y];

                if (visit.y > 0 && directChunk.direction[0] == "1" &&
                    chunksMap[(int)visit.x][(int)visit.y - 1].direction[1] == "1")
                {
                    WaveCheck(visitChunks, bfsMatrix, finish, 0, -1, iteration, ref isFind);
                }
                if (visit.y < ChunksMap[0].Count - 1 && directChunk.direction[1] == "1" &&
                    chunksMap[(int)visit.x][(int)visit.y + 1].direction[0] == "1")
                {
                    WaveCheck(visitChunks, bfsMatrix, finish, 0, 1, iteration, ref isFind);
                }
                if (visit.x < ChunksMap.Count - 1 && directChunk.direction[2] == "1" &&
                    chunksMap[(int)visit.x + 1][(int)visit.y].direction[3] == "1")
                {
                    WaveCheck(visitChunks, bfsMatrix, finish, 1, 0, iteration, ref isFind);
                }
                if (visit.x > 0 && directChunk.direction[3] == "1" &&
                    chunksMap[(int)visit.x - 1][(int)visit.y].direction[2] == "1")
                {
                    WaveCheck(visitChunks, bfsMatrix, finish, -1, 0, iteration, ref isFind);
                }
                visitChunks.RemoveAt(0);

            }

            if (isFind)
            {
                way.Add(finish);
                Vector2 nextChunkPosition = bfsMatrix[(int)finish.x, (int)finish.y].LastPosition;

                while (nextChunkPosition != start)
                {
                    way.Add(nextChunkPosition);
                    nextChunkPosition = bfsMatrix[(int)way[way.Count - 1].x, (int)way[way.Count - 1].y].LastPosition;
                }

                way.Add(start);
            }

            way.Reverse();

            return way;
        }

        public List<Vector2> FindWayInChunk(Vector2 start, Vector2 finish, Vector2 chunkPosition)
        {
            List<Vector2> wayInChunk = new List<Vector2>();

            bfsChunkInf[,] bfsMatrix = new bfsChunkInf[ChunksMap[(int)chunkPosition.x][(int)chunkPosition.y].bitMap.Count, ChunksMap[(int)chunkPosition.x][(int)chunkPosition.y].bitMap[0].Count];
            for (int x = 0; x < ChunksMap[(int)chunkPosition.x][(int)chunkPosition.y].bitMap.Count; x++)
            {
                for (int y = 0; y < ChunksMap[(int)chunkPosition.x][(int)chunkPosition.y].bitMap[x].Count; y++)
                {
                    bfsMatrix[x, y] = new bfsChunkInf();
                }
            }

            List<Vector2> visitFields = new List<Vector2>();
            bool isFind = false;
            int iteration = 0;

            visitFields.Add(start);
            bfsMatrix[(int)start.x, (int)start.y].WayLength = 0;
            bfsMatrix[(int)start.x, (int)start.y].LastPosition = new Vector2(start.x, start.y);


            while ((visitFields.Count > 0) && (!isFind))
            {
                iteration++;
                Vector2 visit = visitFields[0];

                if (visitFields[0].y > 0)
                {
                    WaveCheck(visitFields, bfsMatrix, finish, 0, -1, iteration, ref isFind);
                }
                if (visitFields[0].y < ChunksMap[(int)chunkPosition.x][(int)chunkPosition.y].bitMap[0].Count - 1)
                {
                    WaveCheck(visitFields, bfsMatrix, finish, 0, 1, iteration, ref isFind);
                }
                if (visitFields[0].x < ChunksMap[(int)chunkPosition.x][(int)chunkPosition.y].bitMap.Count - 1)
                {
                    WaveCheck(visitFields, bfsMatrix, finish, 1, 0, iteration, ref isFind);
                }
                if (visitFields[0].x > 0)
                {
                    WaveCheck(visitFields, bfsMatrix, finish, -1, 0, iteration, ref isFind);
                }
                visitFields.RemoveAt(0);

            }

            if (isFind)
            {
                wayInChunk.Add(finish);
                Vector2 nextFieldPosition = bfsMatrix[(int)finish.x, (int)finish.y].LastPosition;

                while (nextFieldPosition != start)
                {
                    wayInChunk.Add(nextFieldPosition);
                    nextFieldPosition = bfsMatrix[(int)wayInChunk[wayInChunk.Count - 1].x, (int)wayInChunk[wayInChunk.Count - 1].y].LastPosition;
                }

                wayInChunk.Add(start);
            }
            wayInChunk.Reverse();


            return wayInChunk;
        }

        void WaveCheck(List<Vector2> visitFields, bfsChunkInf[,] bfsMatrix, Vector2 finish, int reviewX, int reviewY, int iteration, ref bool isFind)
        {
            if (bfsMatrix[(int)visitFields[0].x + reviewX, (int)visitFields[0].y + reviewY].WayLength == -1)
            {
                if (visitFields[0].x + reviewX == finish.x && visitFields[0].y + reviewY == finish.y)
                {
                    isFind = true;
                }
                else
                {
                    visitFields.Add(new Vector2(visitFields[0].x + reviewX, visitFields[0].y + reviewY));
                }
                bfsMatrix[(int)visitFields[0].x + reviewX, (int)visitFields[0].y + reviewY].WayLength = iteration;
                bfsMatrix[(int)visitFields[0].x + reviewX, (int)visitFields[0].y + reviewY].LastPosition = new Vector2(visitFields[0].x, visitFields[0].y);
            }
        }

        enum Direct { D, U, L, R };
        public List<Vector2> FindRoad(List<Vector2> way, Vector2 startPosition)
        {
            List<Vector2> road = new List<Vector2>();
            Vector2 start = startPosition;
            Vector2 finish;
            Direct direction;

            for (int i = 0; i < way.Count-1; i++)
            {
                if (way[i].x < way[i + 1].x)
                {
                    direction = Direct.D;
                }
                else
                {
                    if (way[i].x > way[i + 1].x)
                    {
                        direction = Direct.U;
                    }
                    else
                    {
                        if (way[i].y < way[i + 1].y)
                        {
                            direction = Direct.R;
                        }
                        else
                        {
                            direction = Direct.L;
                        }
                    }
                }

                Chunk currentChunk = ChunksMap[(int)way[i].x][(int)way[i].y];
                Chunk nextChunk = ChunksMap[(int)way[i + 1].x][(int)way[i + 1].y];

                List<Vector2> temporaryWay = new List<Vector2>();

                switch (direction)
                {
                    case Direct.D:
                        {
                            int find = (int)start.y;
                            if (currentChunk.bitMap[9][find] != 0 && nextChunk.bitMap[0][find] != 0)
                            {
                                find = -1;
                                bool isFind = false;

                                while (!isFind)
                                {
                                    find++;
                                    if (currentChunk.bitMap[9][find] == 0 && nextChunk.bitMap[0][find] == 0)
                                    {
                                        isFind = true;
                                    }
                                }
                            }

                            finish = new Vector2(9, find);

                            temporaryWay = FindWayInChunk(start, finish, new Vector2((int)way[i].x, (int)way[i].y));

                            road = TranslateWay(temporaryWay, road, way[i]);

                            start = new Vector2(0, find);

                            break;
                        }

                    case Direct.U:
                        {
                            int find = (int)start.y;
                            if (currentChunk.bitMap[0][find] != 0 && nextChunk.bitMap[9][find] != 0)
                            {
                                find = -1;
                                bool isFind = false;

                                while (!isFind)
                                {
                                    find++;
                                    if (currentChunk.bitMap[0][find] == 0 && nextChunk.bitMap[9][find] == 0)
                                    {
                                        isFind = true;
                                    }
                                }
                            }

                            finish = new Vector2(0, find);

                            temporaryWay = FindWayInChunk(start, finish, new Vector2((int)way[i].x, (int)way[i].y));

                            road = TranslateWay(temporaryWay, road, way[i]);

                            start = new Vector2(9, find);

                            break;
                        }

                    case Direct.R:
                        {
                            int find = (int)start.x;
                            if (currentChunk.bitMap[find][9] != 0 && nextChunk.bitMap[find][0] != 0)
                            {
                                find = -1;
                                bool isFind = false;

                                while (!isFind)
                                {
                                    find++;
                                    if (currentChunk.bitMap[find][9] == 0 && nextChunk.bitMap[find][0] == 0)
                                    {
                                        isFind = true;
                                    }
                                }
                            }

                            finish = new Vector2(find, 9);

                            temporaryWay = FindWayInChunk(start, finish, new Vector2((int)way[i].x, (int)way[i].y));

                            road = TranslateWay(temporaryWay, road, way[i]);

                            start = new Vector2(find, 0);

                            break;
                        }

                    case Direct.L:
                        {
                            int find = (int)start.x;
                            if (currentChunk.bitMap[find][0] != 0 && nextChunk.bitMap[find][9] != 0)
                            {
                                find = -1;
                                bool isFind = false;

                                while (!isFind)
                                {
                                    find++;
                                    if (currentChunk.bitMap[find][0] == 0 && nextChunk.bitMap[find][9] == 0)
                                    {
                                        isFind = true;
                                    }
                                }
                            }

                            finish = new Vector2(find, 0);

                            temporaryWay = FindWayInChunk(start, finish, new Vector2((int)way[i].x, (int)way[i].y));

                            road = TranslateWay(temporaryWay, road, way[i]);

                            start = new Vector2(find, 9);

                            break;
                        }
                }
            }

            start.x = start.x + way[way.Count - 1].x * 10 + 0.5f;
            start.y = start.y + way[way.Count - 1].y * 10 + 0.5f;
            road.Add(start);

            return road;
        }

        public List<Vector2> TranslateWay(List<Vector2> temporaryWay, List<Vector2> road, Vector2 way)
        {
            foreach (var chunk in temporaryWay)
            {
                Vector2 translateChunk = chunk;

                translateChunk.x = chunk.x + way.x * 10 + 0.5f;
                translateChunk.y = chunk.y + way.y * 10 + 0.5f;

                road.Add(translateChunk);
            }

            return road;
        }
    }

    public class PathFinder : MonoBehaviour
    {
        public MapWays mapWay = new MapWays();

    }
}