using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.Utils
{
    public struct LevelInfo
    {
        public char[,] mapOverlay;
        public char[,] map;

        public char GetOverlay(int pX, int pY)
        {
            return mapOverlay[pY, pX];
        }

        public char GetMap(int pX, int pY)
        {
            return map[pY, pX];
        }

        public void SetOverlay(int pX, int pY, char pValue)
        {
            mapOverlay[pY, pX] = pValue;
        }
        public void SetOverlay(Vector2 pPosition, char pValue)
        {
            mapOverlay[(int)pPosition.y, (int)pPosition.x] = pValue;
        }

        public void SetMap(int pX, int pY, char pValue)
        {
            map[pY, pX] = pValue;
        }
        public void SetMap(Vector2 pPosition, char pValue)
        {
            map[(int)pPosition.y, (int)pPosition.x] = pValue;
        }

        public void Print()
        {
            string lMap = "";
            string lOverlay = "";

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    lMap += GetMap(x, y);
                    lOverlay += GetOverlay(x, y);
                }
                lMap += '\n';
                lOverlay += '\n';
            }

            GD.Print(lMap);
            //GD.Print(lOverlay);
        }
    }

}