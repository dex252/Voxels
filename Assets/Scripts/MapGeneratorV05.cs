using System.Collections.Generic;  //Array list и многое другое
using System;
using UnityEngine;
using System.IO; //запись в файл

namespace MapGeneratorNameSpace
{
    public class GraphLine : MonoBehaviour
    {
        public int X1;
        public int X2;
        public int Y1;
        public int Y2; //для условия магистралей
        public GraphLine(int X1, int Y1, int X2, int Y2)
        {
            this.X1 = X1;
            this.X2 = X2;
            this.Y1 = Y1;
            this.Y2 = Y2;
        }

        public string Orientation()
        {
            return (X1 == X2) ? "Vertical" : "Horizontal";
        }
    }

    public class Position : MonoBehaviour
    {
        public int X;
        public int Y;
        public Position(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    [Serializable]
    public class GenerateParams
    {
        [SerializeField] private int length;
        [SerializeField] private int height;
        [SerializeField] private byte chanceTrees; //шанс спавна дерева - через сколько блоков спавнить дерево
        [SerializeField] private byte chanceStones; //шанс спавна камней
        [SerializeField] private byte cityPercent;
        [SerializeField] private byte countOfIterations;

        public GenerateParams()
        {
            this.length = 20;
            this.height = 20;
            this.chanceTrees = 15;
            this.chanceStones = 15;
            this.cityPercent = 70;
            this.countOfIterations = 10;
        }

        public GenerateParams(int length, int height, byte chanceTrees, byte chanceStones, byte cityPercent, byte valueOfIteration)
        {
            this.length = length;
            this.height = height;
            this.chanceTrees = chanceTrees;
            this.chanceStones = chanceStones;
            this.cityPercent = cityPercent;
            this.countOfIterations = valueOfIteration;
        }

        public GenerateParams(GenerateParams paramObj)
        {
            this.length = paramObj.length;
            this.height = paramObj.height;
            this.chanceTrees = paramObj.chanceTrees;
            this.chanceStones = paramObj.chanceStones;
            this.cityPercent = paramObj.cityPercent;
            this.countOfIterations = paramObj.countOfIterations;
        }



        public int Length
        {
            get
            {
                return length; //=function getLength()
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public byte ChanceTrees
        {
            get
            {
                return chanceTrees;
            }
        }

        public byte ChanceStones
        {
            get
            {
                return chanceStones;
            }
        }

        public byte CityPercent
        {
            get
            {
                return cityPercent;
            }
        }

        public byte ValueOfIteration
        {
            get
            {
                return countOfIterations;
            }
        }
    }

    public class MapGeneratorV05 : MonoBehaviour
    {
        public List<List<String>> map= new List<List<string>>();
        private System.Random rnd = new System.Random(); //во избежании ошибок создадим экземпляр 1 раз (для потокв нужно что-то другое)
        [SerializeField] private GenerateParams generateParams;

        public GenerateParams GenerateParams
        {
            get
            {
                return generateParams;
            }
            set
            {
                generateParams = value;
            }
        }

        public List<List<string>> Map
        {
            get
            {
                return map;
            }
        }

        public void Start()   ////////------------
        {
            Generate();
        }               ////////-------------

        public bool CheckInput()
        {
            bool log = true;
            if (GenerateParams.Length < 1 || GenerateParams.Height < 1)
            {
                Debug.Log("Ошибка размеров карты");
                log = false;
            }
            if (GenerateParams.CityPercent < 0 || GenerateParams.CityPercent > 100)
            {
                Debug.Log("Ошибка Процентов площади леса");
                log = false;
            }
            if (GenerateParams.ChanceTrees < 0 || GenerateParams.ChanceTrees > 100)
            {
                Debug.Log("Ошибка Процентов шанса спавна деревьев");
                log = false;
            }
            if (GenerateParams.ChanceStones < 0 || GenerateParams.ChanceStones > 100)
            {
                Debug.Log("Ошибка Процентов шанса спавна камней");
                log = false;
            }
            if (GenerateParams.ChanceStones + GenerateParams.ChanceTrees > 100)
            {
                Debug.Log("Нарушено неравенство: 0<=chanceStones+chanceTrees<=100");
                log = false;
            }

            return log;
        }

        public void Generate()
        {
            if (CheckInput())
            {
                MapCreate();   //Сразу создаем карту
            }
        }

        private void MapCreate()
        {
            int chunkValue = GenerateParams.Length * GenerateParams.Height;
            int roadAndCityValue = (int)Math.Floor((chunkValue / 100d * GenerateParams.CityPercent)); //Количество чанков, выделенных для                                                                         
            int orientation = rnd.Next(0, 1000);  //0 - X (горизонталь), 1- Y (Вертикаль)
            orientation = orientation % 2;

            if (roadAndCityValue > 1)
            {
                MakeCity(GenerateParams.Length, GenerateParams.Height, orientation, ref roadAndCityValue);
            }
        }

        private void MakeCity(int length, int height, int orientation, ref int BuildResourse)
        //создание битового двумерного массива по одному из ориентиров, дорог + строений, перевод в двумерный стринговый массив
        {
            Debug.Log(length);
            Debug.Log(height);
            Debug.Log(orientation);
            Debug.Log(BuildResourse);

            byte[,] chunkWay;     //битовый перевернутый или нет масси вдля размещения города
            int lengthLocal; //Локальная ширина
            int heightLocal; //Локальная высота

            if (orientation == 1)
            {
                lengthLocal = length;
                heightLocal = height;
            }
            else
            {
                lengthLocal = height;
                heightLocal = length;
            }

            chunkWay = new byte[lengthLocal, heightLocal];

            for (int x = 0; x < lengthLocal; x++) //обнуление массива
            {
                for (int y = 0; y < heightLocal; y++)
                {
                    chunkWay[x, y] = 0;
                }
            }

            int iteration = 0;
            int magMassPoint = -1;

            GraphLine[] magistralMass = new GraphLine[10]; //массив пар магистралей

            //индекс последнего элемента массива пары магистрали
            while (iteration < GenerateParams.ValueOfIteration)
            {
                int xRegion = AreaRandom(4, lengthLocal - 1); //каждые 4 блока
                int yRegion = AreaRandom(2, heightLocal - 1); // каждые 2 блока

                SetRegionOfChunks(xRegion, yRegion, lengthLocal, heightLocal, ref BuildResourse, chunkWay);

                if (SecurityStatus(BuildResourse))
                {
                    Debug.Log("Окончание цикла"); break; //выход из while, если нет доступных блоков
                }

                bool leftMagistral = true;   //Работа с магистралью
                bool rightMagistral = true;

                for (int index = 0; index <= magMassPoint; index++) //проверка свободной области
                {
                    if (Math.Abs(xRegion - magistralMass[index].X1) < 50
                        || Math.Abs(xRegion - magistralMass[index].X2) < 50) //если в радиусе 50 чанок существует магистраль, то её не записывают
                    {
                        leftMagistral = false;
                    }
                    if (Math.Abs(xRegion + 16 - magistralMass[index].X1) < 50
                        || Math.Abs(xRegion + 16 - magistralMass[index].X2) < 50) //если в радиусе 50 чанок существует магистраль, то её не записывают
                    {
                        rightMagistral = false;
                    }
                }

                if (leftMagistral || rightMagistral)
                {
                    magMassPoint++;

                    if (leftMagistral)
                    {
                        magistralMass[magMassPoint] = new GraphLine(xRegion, 0, 0, 0);
                        CreateMagistral(magistralMass[magMassPoint].X1, lengthLocal - 1, heightLocal - 1, ref BuildResourse, chunkWay);
                        //создание левой магистрали
                    }
                    else
                    {
                        magistralMass[magMassPoint] = new GraphLine(-100, 0, 0, 0);
                    }

                    if (rightMagistral)
                    {
                        magistralMass[magMassPoint].X2 = xRegion + 16;
                        CreateMagistral(magistralMass[magMassPoint].X2, lengthLocal - 1, heightLocal - 1, ref BuildResourse, chunkWay);
                    }
                    else
                    {
                        magistralMass[magMassPoint].X2 = -100;
                    }
                }

                if (SecurityStatus(BuildResourse)) break;

                SetRegionOfChunks(xRegion + 16, yRegion, lengthLocal, heightLocal, ref BuildResourse, chunkWay);

                if (SecurityStatus(BuildResourse)) break;

                SetRegionOfChunks(xRegion - 16, yRegion, lengthLocal, heightLocal, ref BuildResourse, chunkWay);

                if (SecurityStatus(BuildResourse)) break;

                SetRegionOfChunks(xRegion, yRegion + 10, lengthLocal, heightLocal, ref BuildResourse, chunkWay);

                if (SecurityStatus(BuildResourse)) break;

                SetRegionOfChunks(xRegion, yRegion - 10, lengthLocal, heightLocal, ref BuildResourse, chunkWay);
                iteration++;
            }

            if (!SecurityStatus(BuildResourse))
            {//остальсь блоки для расширения вширь. Нельзя прекратить метод. Можно запихнуть в другую функцию
                int valueOfBlocks = 1; //количество блоков, которые используются в данный момент --------
                int xPositionOfCornel;
                int yPositionOfCornel;

                List<Position> blockPosition = new List<Position>(); //лист специальново типа для хранения координат

                sbyte horizontalValue;
                sbyte verticalValue;

                if (rnd.Next(0, 1000) % 2 == 1) //справа начинаем
                {
                    horizontalValue = -1;
                    xPositionOfCornel = (lengthLocal - 1) / 16;
                    xPositionOfCornel = xPositionOfCornel * 16; //избавились от нецелой части
                }
                else //слева начинаем
                {
                    horizontalValue = 1;
                    xPositionOfCornel = 0;
                }

                if (rnd.Next(0, 1000) % 2 == 1) //снизу начинаем
                {
                    verticalValue = -1;
                    yPositionOfCornel = (heightLocal - 1) / 10;
                    yPositionOfCornel = yPositionOfCornel * 10; //избавились от нецелой части
                }
                else //сверху начинаем
                {
                    verticalValue = 1;
                    yPositionOfCornel = 0;
                }

                blockPosition.Add(new Position(xPositionOfCornel, yPositionOfCornel)); //позиция первого спавна

                while (blockPosition.Count > 0) //пока не опустел будет спавниться и продолжаться итерации
                {

                    int newValueOfBlocks = 0; //сколько добавилось блоков

                    if (SecurityStatus(BuildResourse)) break;

                    for (int Index = 0; Index < valueOfBlocks; Index++) //сколько раз нужно пройтись по списку
                    {
                        Debug.Log(blockPosition[Index].X);
                        Debug.Log(blockPosition[Index].Y);

                        SetRegionOfChunks(blockPosition[Index].X, blockPosition[Index].Y, lengthLocal, heightLocal, ref BuildResourse, chunkWay);

                        if (blockPosition[Index].X + (16 * horizontalValue) >= 0 &&
                            blockPosition[Index].X + (16 * horizontalValue) < lengthLocal) //попадает в промежуток по горизонтали
                        {
                            blockPosition.Add(new Position(blockPosition[Index].X + (16 * horizontalValue),
                                                                            blockPosition[Index].Y)); //добавили влево или вправо
                            newValueOfBlocks++;
                        }

                        if (blockPosition[Index].Y + (10 * verticalValue) >= 0 && 
                            blockPosition[Index].Y + (10 * verticalValue) < heightLocal && 
                            Index== valueOfBlocks-1)//попадает в промежуток по вертикали
                        {
                            blockPosition.Add(new Position(blockPosition[Index].X, blockPosition[Index].Y + (10 * verticalValue)));
                            newValueOfBlocks++;
                        }
                    }

                    for (int Index = 0; Index < valueOfBlocks; Index++)
                    {
                        blockPosition.RemoveAt(0);
                    }

                    valueOfBlocks = newValueOfBlocks;
                }
            }

            Debug.Log(BuildResourse);

            byte periodTrees = (byte)(100 / GenerateParams.ChanceTrees);
            byte periodStones = (byte)(100 / GenerateParams.ChanceStones);

            ChanceRealizator(periodStones, 5, lengthLocal - 1, heightLocal - 1, chunkWay);
            ChanceRealizator(periodTrees, 6, lengthLocal - 1, heightLocal - 1, chunkWay);

            //StreamWriter sr = new StreamWriter("C://Test2.txt");

            //for (int Index = 0; Index < heightLocal; Index++)
            //{
            //    for (int Index2 = 0; Index2 < lengthLocal; Index2++)
            //    {
            //        sr.Write(chunkWay[Index2, Index]);
            //    }
            //    sr.WriteLine();
            //}
            //sr.Close();

            WriteMap(lengthLocal - 1, heightLocal - 1, chunkWay, orientation);
        }

        private void SetRegionOfChunks(int xRegion, int yRegion, int lengthLocal, int heightLocal, ref int buildResourse, byte[,] chunkWayMass) //для строения жилых кварталов
        {
            for (int IndexY = 0; IndexY < 5; IndexY++) //5 в высоту наборов чанков
            {
                for (int IndexX = 0; IndexX < 4; IndexX++) //4 в длину
                {
                    SpawnChunk(xRegion + IndexX * 4, yRegion + IndexY * 2, lengthLocal - 1, heightLocal - 1, ref buildResourse, chunkWayMass); //спавнится в зависимости от периода и рандомного местоположения
                }
            }
        }

        private bool SecurityStatus(int buildResourse)  
        {
            if (buildResourse < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int AreaRandom(byte period, int maxLength)  //выводит рандомное число из отрезка от 0 до MaxLength, которое имеет переодичность (1 3 5 -> период 2). Сдвиг не учитывается
        {
            int maxRandomValue = maxLength / period;
            int randomValue = rnd.Next(0, maxRandomValue);

            return (period * randomValue);
        }

        private void SpawnChunk(int x, int y, int maxX, int maxY, ref int buildResourse, byte[,] chunkMass) //спавнит область 5*3
        {
            LineSpawn(x, y, maxX, maxY, 2, 3, chunkMass, ref buildResourse);

            if (SecurityStatus(buildResourse))
            {
                return;
            }//обрывает выполнение метода если кончились строительные блоки

            LineSpawn(x, y + 1, maxX, maxY, 3, 4, chunkMass, ref buildResourse);

            if (SecurityStatus(buildResourse))
            {
                return;
            }//обрывает выполнение метода если кончились строительные блоки

            LineSpawn(x, y + 2, maxX, maxY, 2, 3, chunkMass, ref buildResourse);
        }

        private void LineSpawn(int x, int y, int maxX, int maxY, byte typeTip,
            byte typeHandle, byte[,] mass, ref int buildResourse) //спавнит линию типа (23332 или 24442)
        { 
            for (int index = 0; index < 5; index++)
            {
                if (PrivatArea(x + index, y, maxX, maxY, ref buildResourse, mass) == 2)
                {
                    if (index > 0 && index < 4)
                    {
                        mass[x + index, y] = typeHandle; //населенные дороги - чанки с дорогой и домиками
                    }
                    else
                    {
                        mass[x + index, y] = typeTip; //просто дороги без домиков 1-для магистралей (2 -могут отличаться от 1 !!! на будущее)
                    }
                    buildResourse -= 1;
                }
            }
        }

        private byte PrivatArea(int x, int y, int maxX, int maxY, 
            ref int buildResourse, byte[,] chunkMass) //смотрит на то, можно ли вообще ставить в текущий блок массива
        { 
            if (x > maxX || x < 0 || y > maxY || y < 0)
            {
                return 0;
            }

            if (buildResourse < 1)
            {
                return 0;
            }

            if (chunkMass[x, y] != 0) {
                return 1;
            };

            return 2; //если все проверки прошел
        }

        private void CreateMagistral(int xRegion, int maxX, int maxY, ref int buildResourse, byte[,] chunkMass)         //создаем магистраль
        { 
            if (xRegion < 0 || xRegion > maxX)
            {
                return;
            } //если выходит за границы массива

            for (int index = 0; index <= maxY; index++)
            {
                if (PrivatArea(xRegion, index, maxX, maxY, ref buildResourse, chunkMass) == 2)
                {
                    chunkMass[xRegion, index] = 1;
                    buildResourse--;
                }
                if (PrivatArea(xRegion, index, maxX, maxY, ref buildResourse, chunkMass) == 1) //замена существующего блока
                {
                    chunkMass[xRegion, index] = 1;
                }
            }
        }

        private void ChanceRealizator(byte period, byte chunkByte, 
            int maxLength, int maxHeight, byte[,] chunkMass) //спавнит байтовые айди на массиве с заданным шагом и заданными айдишниками
        {
            int indexY = 0;
            byte localPeriod = (byte)rnd.Next(1, period); //при достижении LocalPeriod=Period происходит спавн

            while (indexY >= 0 && indexY < maxHeight)
            {
                for (int indexX = 0; indexX < maxLength; indexX++)
                {
                    if (chunkMass[indexX, indexY] == 0)
                    {
                        if (period == localPeriod)
                        {
                            chunkMass[indexX, indexY] = chunkByte;
                            localPeriod = 1;
                        }
                        else
                        {
                            localPeriod++;
                        }
                    }
                }
                indexY += period;
            }
        }

        private void WriteMap(int maxX, int maxY, byte[,] chunkMass, int orientation)
        {
            byte[,] chunkMass2= (orientation == 1)? new byte[maxY + 1, maxX + 1]: new byte[maxX + 1, maxY + 1];

            for (int x=0; x<=chunkMass2.GetLength(0)-1;x++) //транспонировать матрицу
            {
                for (int y=0; y<= chunkMass2.GetLength(1)-1; y++)
                {
                    if (orientation == 1)
                    {
                        chunkMass2[x, y] = chunkMass[y, x];
                    }
                    else
                    {
                        chunkMass2[x, y] = chunkMass[x, y];
                    }
                }
            }

            string[,] mapString = (orientation == 1) ? new string[maxY+1, maxX+1] : new string[maxX+1,maxY+1];

            for (int indexY = 0; indexY <= mapString.GetLength(1)-1; indexY++)
            {
                for (int indexX = 0; indexX <= mapString.GetLength(0)-1; indexX++)
                {
                    switch (chunkMass2[indexX, indexY])
                    {
                        case (5):
                            mapString[indexX, indexY] = "Chunk_10_0_0_0_0";
                            break;
                        case (6):
                            mapString[indexX, indexY] = "Chunk_13_1_1_1_1";
                            break;
                        case (0):
                            mapString[indexX, indexY] = "Chunk_00_1_1_1_1";
                            break;
                        default:
                            WriyeRoadMap("Chunk", chunkMass2[indexX, indexY], indexX, indexY, mapString.GetLength(0)-1, mapString.GetLength(1)-1, chunkMass2, mapString);
                            break;
                    }

                }
            }

            for (int indeX= map.Count-1; indeX >= 0; indeX--)
            {
                for (int indeY = 0; indeY < map[indeX].Count; indeY++)
                {
                    map[indeX].RemoveAt(0);
                }
                map.RemoveAt(indeX);
            }
            for (int indeX = 0; indeX < mapString.GetLength(0); indeX++)
            {
                map.Add(new List<string>());
                for (int indeY = 0; indeY < mapString.GetLength(1); indeY++)
                {
                    map[indeX].Add(mapString[indeX, indeY]); //--------------------
                }
            }

        }

        private void WriyeRoadMap(string prefix, byte byteId, int indexX, int indexY,
             int maxX, int maxY, byte[,] chunkMass, string[,] massString)
        {
            int id = 1;  //id чанка, который пойдет в название файла

            switch (byteId)
            {
                case (1):  //тип чанка (Forest road)
                    id = 1;
                    break;
                case (2): // City road without buikdings
                    id = 2;
                    break;
                case (3): //City road with buildings
                    id = rnd.Next(2, 2);
                    break;
                case (4): //асфальт
                    id = rnd.Next(4, 4);  //!!!
                    break;
            }

            byte up = 0;
            byte down = 0;
            byte left = 0;
            byte right = 0; //направления

            if (indexY > 0)
            {
                if (chunkMass[indexX, indexY - 1] > 0 && chunkMass[indexX, indexY - 1] < 4)
                {
                    //up = 1;
                    down = 1;
                }
            }

            if (indexY < maxY)
            {
                if (chunkMass[indexX, indexY + 1] > 0 && chunkMass[indexX, indexY + 1] < 4)
                {
                    //down = 1;
                    up = 1;
                }
            }

            if (indexX < maxX)
            {
                if (chunkMass[indexX + 1, indexY] > 0 && chunkMass[indexX + 1, indexY] < 4)
                {
                    right = 1;
                }
            }

            if (indexX > 0)
            {
                if (chunkMass[indexX - 1, indexY] > 0 && chunkMass[indexX - 1, indexY] < 4)
                {
                    left = 1;
                }
            }

            if (byteId==4)
            {
                left = 1;
                right = 1;
                down = 1;
                up = 1;
            }

            if (id==2 && down==1 && up==1 && right==0 && left == 0 || id == 2 && down == 0 && up == 0 && right == 1 && left == 1)
            {
                int value = rnd.Next(0, 10);
                value = value / 3;
                if (value == 1)
                {
                    id = 5;
                }
                if (value == 2)
                {
                    id = 6;
                }
            }

            String file = $"{prefix}_0{id}_{left}_{right}_{down}_{up}"; //id - значение от 0 до 9 !!!
            massString[indexX, indexY] = file;
        }

        public void convertMagistralName() //only after generate a map //for Vyacheslav II
        {
            for (int indexX=0; indexX<map.Count;indexX++)
            {
                for(int indexY=0; indexY< map[indexX].Count; indexY++)
                {
                    string chunkName = String.Copy(map[indexX][indexY]);
                    chunkName = chunkName.Remove(0,6);
                    chunkName = chunkName.Remove(2, 8);
                    int indexOfChunk = Convert.ToInt32(chunkName);
                    if (indexOfChunk == 1)
                    {
                        Debug.Log(map[indexX][indexY]);
                        map[indexX][indexY] = "Chunk_01_1_1_1_1";
                    }
                }
            }
        }
    }
}

