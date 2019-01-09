using System.Collections;
using System.Collections.Generic;  //Array list и многое другое
using System;
using UnityEngine;
using GraphLineClass;
using System.IO; //запись в файл
using System.Text;//форматы записи текста: Unicode, ASCII and etc;

public class MapGenerator : MonoBehaviour
{

    public int Length;
    public int Height;
    public byte chanceTrees; //шанс спавна дерева - через сколько блоков спавнить дерево
    public byte chanceStones; //шанс спавна камней
    public byte CityProcent;
    private string[,] Map;
    private System.Random rnd = new System.Random(); //во избежании ошибок создадим экземпляр 1 раз (для потокв нужно что-то другое)
    void Start()
    {
        Generate();
    }

    public bool Diagnostic()
    {
        bool Log = true;
        if (Length < 1 | Height < 1)
        {
            Debug.Log("Ошибка размеров карты");
            Log = false;
        }
        if (CityProcent < 0 | CityProcent > 100)
        {
            Debug.Log("Ошибка Процентов площади леса");
            Log = false;
        }
        if (chanceTrees < 0 | chanceTrees > 100)
        {
            Debug.Log("Ошибка Процентов шанса спавна деревьев");
            Log = false;
        }
        if (chanceStones < 0 | chanceStones > 100)
        {
            Debug.Log("Ошибка Процентов шанса спавна камней");
            Log = false;
        }
        if (chanceStones + chanceTrees > 100)
        {
            Debug.Log("Ошибка в уравнении: 0<=chanceStones+chanceTrees<=100");
            Log = false;
        }

        return Log;
    }

    private void Generate()
    {
        if (Diagnostic())
        {
            MapCreate();   //Сразу создаем карту
        }
    }

    private void MapCreate()
    {
        int ChunkValue = Length * Height;
        //Debug.Log(ChunkValue);
        int RoadAndCityValue = (int)Math.Floor((ChunkValue / 100d * CityProcent)); //Количество чанков, выделенных для строений
        //Debug.Log(RoadAndCityValue);
        int Orientation = rnd.Next(0, 1000);  //0 - X (горизонталь), 1- Y (Вертикаль)
        Orientation = Orientation % 2;
        //Debug.Log(Orientation);
        if (RoadAndCityValue > 1)
        {
            MakeCity(Length, Height, Orientation, ref RoadAndCityValue);
        }
    }

    private void MakeCity(int Length, int Height, int Orientation, ref int BuildResourse)  //создание битового двумерного массива по одному из ориентиров, дорог + строений, перевод в двумерный стринговый массив
    {
        byte[,] ChunkWay;     //битовый перевернутый или нет масси вдля размещения города
        int LengthLocal; //Локальная ширина
        int HeightLocal; //Локальная высота
        if (Orientation == 1)
        {
            LengthLocal = Length;
            HeightLocal = Height;
        }
        else
        {
            LengthLocal = Height;
            HeightLocal = Length;
        }
        ChunkWay = new byte[LengthLocal, HeightLocal];
        for (int Index1 = 0; Index1 < LengthLocal; Index1++) //обнуление массива
        {
            for (int Index2 = 0; Index2 < HeightLocal; Index2++)
            {
                ChunkWay[Index1, Index2] = 0;
            }
        }
        int iteration = 1;
        GraphLineClass.GraphLine[] MagistralMass = new GraphLineClass.GraphLine[10]; //массив пар магистралей
        int MagMassPoint = -1; //индекс последнего элемента массива пары магистрали
        while (iteration < 11)//&& !BuildSecurity)
        {
            int XRegion = AreaRandom(4, LengthLocal - 1); //каждые 4 блока
            int YRegion = AreaRandom(2, HeightLocal - 1); // каждые 2 блока
            setRegionOfChunks(XRegion, YRegion, LengthLocal, HeightLocal, ref BuildResourse, ChunkWay);
            if (SecurityStatus(BuildResourse)) { Debug.Log("Окончание цикла"); break; } //выход из while, если нет доступных блоков
            bool LeftMagistral = true;                                                                            //Работа с магистралью
            bool RightMagistral = true;
            for (int index = 0; index <= MagMassPoint; index++) //проверка свободной области
            {
                if (Math.Abs(XRegion - MagistralMass[index].X1) < 50 | Math.Abs(XRegion - MagistralMass[index].X2) < 50) //если в радиусе 50 чанок существует магистраль, то её не записывают
                {
                    LeftMagistral = false;
                }
                if (Math.Abs(XRegion + 16 - MagistralMass[index].X1) < 50 | Math.Abs(XRegion + 16 - MagistralMass[index].X2) < 50) //если в радиусе 50 чанок существует магистраль, то её не записывают
                {
                    RightMagistral = false;
                }
            }
            if (LeftMagistral | RightMagistral)
            {
                MagMassPoint++;
                if (LeftMagistral)
                {
                    MagistralMass[MagMassPoint] = new GraphLineClass.GraphLine(XRegion, 0, 0, 0);
                    createMagistral(MagistralMass[MagMassPoint].X1, LengthLocal - 1, HeightLocal-1,ref BuildResourse, ChunkWay); //создание левой магистрали
                } else {
                    MagistralMass[MagMassPoint] = new GraphLineClass.GraphLine(-100, 0, 0, 0);
                }

                if (RightMagistral)
                {
                    MagistralMass[MagMassPoint].X2 = XRegion + 16;
                    createMagistral(MagistralMass[MagMassPoint].X2, LengthLocal - 1, HeightLocal-1, ref BuildResourse, ChunkWay);
                }
                else
                {
                    MagistralMass[MagMassPoint].X2 = -100;
                }
            }
            if (SecurityStatus(BuildResourse)) { Debug.Log("Окончание цикла"); break; } //выход из while, если нет доступных блоков
            setRegionOfChunks(XRegion+16, YRegion, LengthLocal, HeightLocal, ref BuildResourse, ChunkWay);
            if (SecurityStatus(BuildResourse)) { Debug.Log("Окончание цикла"); break; } //выход из while, если нет доступных блоков
            setRegionOfChunks(XRegion-16, YRegion, LengthLocal, HeightLocal, ref BuildResourse, ChunkWay);
            if (SecurityStatus(BuildResourse)) { Debug.Log("Окончание цикла"); break; } //выход из while, если нет доступных блоков
            setRegionOfChunks(XRegion, YRegion+10, LengthLocal, HeightLocal, ref BuildResourse, ChunkWay);
            if (SecurityStatus(BuildResourse)) { Debug.Log("Окончание цикла"); break; } //выход из while, если нет доступных блоков
            setRegionOfChunks(XRegion, YRegion-10, LengthLocal, HeightLocal, ref BuildResourse, ChunkWay);
            iteration++;
        }
        if (!SecurityStatus(BuildResourse))
        {//остальсь блоки для расширения в ширь. Нельзя прекратить метод. Можно запихнуть в другую функцию
            int ValueOfBlocks = 1; //количество блоков, которые используются в данный момент --------
            List<GraphLineClass.Position> BlockPosition = new List<GraphLineClass.Position>(); //лист специальново типа для хранения координат
            sbyte HorizontalValue;
            sbyte VerticalValue;
            int XPositionOfCornel;
            int YPositionOfCornel;
            if (rnd.Next(0, 1000) % 2 == 1) //справа начинаем
            {
                HorizontalValue = -1;
                XPositionOfCornel = (LengthLocal - 1) / 4;
                XPositionOfCornel = XPositionOfCornel * 4; //избавились от нецелой части
            }
            else //слева начинаем
            {
                HorizontalValue = 1;
                XPositionOfCornel = 0;
            }
            if (rnd.Next(0, 1000) % 2 == 1) //снизу начинаем
            {
                VerticalValue = -1;
                YPositionOfCornel = (HeightLocal - 1) / 2;
                YPositionOfCornel = YPositionOfCornel * 2; //избавились от нецелой части
            }
            else //сверху начинаем
            {
                VerticalValue = 1;
                YPositionOfCornel = 0;
            }
            BlockPosition.Add(new GraphLineClass.Position(XPositionOfCornel, YPositionOfCornel)); //позиция первого спавна
            while (BlockPosition.Count > 0) //пока не опустел будет спавниться и продолжаться итерации
            {
                Debug.Log(BlockPosition.Count);
                int newValueOfBlocks = 0; //сколько добавилось блоков
                if (SecurityStatus(BuildResourse)) { break; } //преждевременный выход из цыкла
                for (int Index = 0; Index < ValueOfBlocks; Index++)
                {
                    setRegionOfChunks(BlockPosition[Index].X, BlockPosition[Index].Y, LengthLocal, HeightLocal, ref BuildResourse, ChunkWay);
                    if (BlockPosition[Index].X + (16 * HorizontalValue) >= 0 && BlockPosition[Index].X + (16 * HorizontalValue) < LengthLocal) //попадает в промежуток по горизонтали
                    {
                        BlockPosition.Add(new GraphLineClass.Position(BlockPosition[Index].X + (16 * HorizontalValue), BlockPosition[Index].Y)); //добавили влево или вправо
                        newValueOfBlocks++;
                    }
                    if (BlockPosition[Index].Y + (10 * VerticalValue) >= 0 && BlockPosition[Index].Y + (10 * VerticalValue) < HeightLocal)//попадает в промежуток по вертикали
                    {
                        BlockPosition.Add(new GraphLineClass.Position(BlockPosition[Index].X, BlockPosition[Index].Y + (10 * VerticalValue)));
                        newValueOfBlocks++;
                    }
                }
                for (int Index = 0; Index < ValueOfBlocks; Index++)
                {
                    BlockPosition.RemoveAt(0);
                }
                ValueOfBlocks = newValueOfBlocks;
            }
        }
        byte periodTrees = (byte)(100 / chanceTrees);
        byte periodStones = (byte)(100 / chanceStones);
        chanceRealizator(periodStones, 5, LengthLocal - 1, HeightLocal - 1, ChunkWay);
        chanceRealizator(periodTrees, 6, LengthLocal - 1, HeightLocal - 1, ChunkWay);
        Map = new string[Length, Height];
        WriteMap(LengthLocal-1,HeightLocal-1, ChunkWay, Orientation);
    }

    private void setRegionOfChunks(int XRegion, int YRegion, int LengthLocal, int HeightLocal, ref int BuildResourse, byte[,] ChunkWayMass) //для строения жилых кварталов
    {
        for (int IndexY = 0; IndexY < 5; IndexY++) //5 в высоту наборов чанков
        {
            for (int IndexX = 0; IndexX < 4; IndexX++) //4 в длину
            {
                SpawnChunk(XRegion + IndexX * 4, YRegion + IndexY * 2, LengthLocal - 1, HeightLocal - 1, ref BuildResourse, ChunkWayMass); //спавнится в зависимости от периода и рандомного местоположения
            }
        }
    }

    private bool SecurityStatus(int BuildResourse)  //
    {
        if (BuildResourse < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int AreaRandom(byte Period, int MaxLength)  //выводит рандомное число из отрезка от 0 до MaxLength, которое имеет переодичность (1 3 5 -> период 2). Сдвиг не учитывается
    {
        int MaxRandomValue = MaxLength / Period;
        //Debug.Log(MaxRandomValue);
        int RandomValue = rnd.Next(0, MaxRandomValue);
        //Debug.Log(MaxRandomValue*4);
        return (Period * RandomValue);
    }

    private void SpawnChunk(int X, int Y, int MaxX, int MaxY, ref int BuildResourse, byte[,] ChunkMass) //спавнит область 5*3
    {
        LineSpawn(X, Y, MaxX, MaxY, 2, 3, ChunkMass, ref BuildResourse);
        if (SecurityStatus(BuildResourse)) { return; }//обрывает выполнение метода если кончились строительные блоки
        LineSpawn(X, Y + 1, MaxX, MaxY, 3, 4, ChunkMass, ref BuildResourse);
        if (SecurityStatus(BuildResourse)) { return; }//обрывает выполнение метода если кончились строительные блоки
        LineSpawn(X, Y + 2, MaxX, MaxY, 2, 3, ChunkMass, ref BuildResourse);
    }

    private void LineSpawn(int X, int Y, int MaxX, int MaxY, byte typeTip, byte typeHandle, byte[,] Mass, ref int BuildResourse) { //спавнит линию типа (23332 или 24442)
        for (int Index = 0; Index < 5; Index++)
        {
            if (PrivatArea(X + Index, Y, MaxX, MaxY, ref BuildResourse, Mass)==2)
            {
                if (Index > 0 && Index < 4)
                {
                    Mass[X + Index, Y] = typeHandle; //населенные дороги - чанки с дорогой и домиками
                }
                else
                {
                    Mass[X + Index, Y] = typeTip; //просто дороги без домиков 1-для магистралей (2 -могут отличаться от 1 !!! на будущее)
                }
                BuildResourse -= 1;
            }
        }
    }

    private byte PrivatArea(int X, int Y, int MaxX, int MaxY, ref int BuildResourse, byte[,] ChunkMass)
    { //смотрит на то, можно ли вообще ставить в текущий блок массива
        if (X > MaxX | X < 0 | Y > MaxY | Y < 0) { return 0; }
        if (BuildResourse < 1) { return 0; }
        if (ChunkMass[X, Y] != 0) { return 1; };
        return 2; //если все проверки прошел

    }

    private void createMagistral(int XRegion, int MaxX, int MaxY, ref int BuildResourse, byte[,] ChunkMass){ //создаем магистраль
        if (XRegion < 0 | XRegion > MaxX) { return; } //если выходит за границы массива
        for(int index=0; index <= MaxY;index++)
        {
            if (PrivatArea(XRegion, index, MaxX, MaxY, ref BuildResourse, ChunkMass) == 2)
            {
                ChunkMass[XRegion, index] = 1;
                BuildResourse--;
            }
            if (PrivatArea(XRegion, index, MaxX, MaxY, ref BuildResourse, ChunkMass) == 1) //замена существующего блока
            {
                ChunkMass[XRegion, index] = 1;
            }
        }
    }

    private void chanceRealizator(byte period, byte ChunkByte, int MaxLength, int MaxHeight , byte[,] ChunkMass) //спавнит байтовые айди на массиве с заданным шагом и заданными айдишниками
    {
        byte localPeriod = (byte)rnd.Next(1, period); //при достижении LocalPeriod=Period происходит спавн
        int IndexY = 0;
        while (IndexY>=0 && IndexY< MaxHeight)
        {
            for (int IndexX=0; IndexX<MaxLength; IndexX++)
            {
                if (ChunkMass[IndexX, IndexY] == 0)
                {
                    if (period == localPeriod)
                    {
                        ChunkMass[IndexX, IndexY] = ChunkByte;
                        localPeriod = 1;
                    }
                    else
                    {
                        localPeriod++;
                    }
                }
            }
            IndexY += period; //можно по 1-й прибавлять
        }
    }

    private void WriteMap (int MaxX, int MaxY, byte[,] ChunkMass, int Orientation)
    {
        for (int IndexY=0; IndexY <= MaxY; IndexY++)
        {
            for (int IndexX=0; IndexX<= MaxX; IndexX++)
            {
                int swopIndexX = (Orientation == 1) ? IndexX : IndexY;
                int swopIndexY = (Orientation == 1) ? IndexY : IndexX;
                switch (ChunkMass[IndexX, IndexY])
                {
                    case (5):
                        Map[swopIndexX, swopIndexY] = "Chunk_10_0_0_0_0";
                        break;
                    case (6):
                        Map[swopIndexX, swopIndexY] = "Chunk_13_1_1_1_1";
                        break;
                    case (0):
                        Map[swopIndexX, swopIndexY] = "Chunk_00_1_1_1_1";
                        break;
                    default:
                        WriyeRoadMap("Chunk", ChunkMass[IndexX, IndexY], IndexX, IndexY, Orientation, MaxX, MaxY, ChunkMass);
                        break;
                }
                
            }
        }
    }

    private void WriyeRoadMap(string Prefix, byte byteId,int IndexX, int IndexY, int Orientation,int MaxX, int MaxY, byte[,] ChunkMass)
    {
        int id=1;
        switch (byteId)
        {
            case (3):
                id = rnd.Next(5,6);
                break;
            case (1):
                id = 3;
                break;
            case (2):
                id = 4;
                break;
            case (4):
                id = rnd.Next(7, 7);
                break;
        }
        byte Up=0;
        byte Down=0;
        byte Left=0;
        byte Right=0; //направления
        if (IndexY > 0)
        {
            if (ChunkMass[IndexX, IndexY - 1] > 0 && ChunkMass[IndexX, IndexY - 1] < 4)
            {
                Up = 1;
            }
        }
        if (IndexY < MaxY)
        {
            if (ChunkMass[IndexX, IndexY + 1] > 0 && ChunkMass[IndexX, IndexY + 1] < 4)
            {
                Down = 1;
            }
        }
        if (IndexX < MaxX)
        {
            if (ChunkMass[IndexX+1, IndexY] > 0 && ChunkMass[IndexX+1, IndexY] < 4)
            {
                Right = 1;
            }
        }

        if (IndexX > 0)
        {
            if (ChunkMass[IndexX - 1, IndexY] > 0 && ChunkMass[IndexX - 1, IndexY] < 4)
            {
                Left = 1;
            }
        }
        String File = $"{Prefix}_0{id}_{Left}_{Right}_{Down}_{Up}"; //id - значение от 0 до 9 !!!
        if (Orientation == 1)
        {
            Map[IndexX, IndexY] = File;
        }
        else
        {
            Map[IndexY, IndexX] = File;
        }
    }
}
