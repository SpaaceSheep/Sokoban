using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.utils.game
{
    public class RadarManager : Node
    {
        private static float halfWidth;
        private static float halfHeight;
        public static RadarManager Instance { get; private set; }

        public static void Init(uint pTileWidth, uint pTileHeight)
        {
            halfWidth = pTileWidth /2f;
            halfHeight = pTileHeight /2f;
        }

        public static Vector2 ModelToRadarView(Vector2 pPoint)
        {
            return new Vector2(
                (pPoint.x * halfWidth),
                (pPoint.y * halfHeight)
            );
        }

        public static Vector2 RadarViewToModel(Vector2 pPoint)
        {
            return new Vector2(
                Mathf.Round((pPoint.y / halfHeight + pPoint.x / halfWidth) /2),
                Mathf.Round((pPoint.y / halfHeight - pPoint.x / halfWidth) /2)
            );
        }

        public static int GetZIndex(Vector2 pPoint)
        {
            return Mathf.RoundToInt(pPoint.x + pPoint.y);
        }
    }
}