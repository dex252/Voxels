using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphLineClass
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
            if ((X1 - X2) == 0)
            {
                return "Vertical";
            }
            else
            {
                return "Horizontal";
            }
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
}
