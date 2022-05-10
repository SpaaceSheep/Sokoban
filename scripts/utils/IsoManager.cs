using Godot;
using System;

namespace Com.IsartDigital.Utils.Game
{
	public static class IsoManager
	{
		private static float halfWidth;
		private static float halfHeight;

		public static void Init(uint pTileWidth, uint pTileHeight)
		{
			halfWidth = pTileWidth / 2f;
			halfHeight = pTileHeight / 2f;
		}
	
		public static Vector2 ModelToIsoView(Vector2 pPoint)
		{
			return new Vector2(
				(pPoint.x - pPoint.y) * halfWidth,
				(pPoint.x + pPoint.y) * halfHeight
			);
		}
	
		public static Vector2 IsoViewToModel(Vector2 pPoint)
		{
			return new Vector2(
				Mathf.Round((pPoint.y / halfHeight + pPoint.x / halfWidth) / 2),
				Mathf.Round((pPoint.y / halfHeight - pPoint.x / halfWidth) / 2)
			);
		}

		public static int GetZIndex(Vector2 pPoint)
		{
			return Mathf.RoundToInt(pPoint.x + pPoint.y);
		}
	}
}