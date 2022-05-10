using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.Utils
{
    public static class JsonParseHelper
	{
		public const int OFFSET = 40;
		public const int HANDLE_CODE = 20;
		public const int TARGET_CODE = 40;
		public const float HALF_PI = Mathf.Pi * .5f;
		public const char PLAYER_CODE = 'P';
		public const char CRATE_CODE = 'B';

		public static List<Vector2> ConfigToVector2(int pConfig)
        {
			List<Vector2> lDirections = new List<Vector2>();
			Vector2 lDirection = Vector2.Up;

			for (int i = 0; i < 4; i++)
			{
				if ((pConfig >> i & 1) == 1)
				{
					lDirections.Add(lDirection);
				}
				lDirection = lDirection.Rotated(Mathf.Pi / 2).Round();
			}
			return lDirections;
        }

		public static List<float> ConfigToRads(int pConfig)
		{
			List<float> lDirections = new List<float>();
			Vector2 lDirection = Vector2.Up;

			for (int i = 0; i < 4; i++)
			{
				if ((pConfig >> i & 1) == 1) 
				{
					lDirections.Add(Mathf.Atan2(lDirection.y, lDirection.x) + JsonParseHelper.HALF_PI);
				}
				lDirection = lDirection.Rotated(Mathf.Pi / 2).Round();
			}
			return lDirections;
		}

		public static int Vector2ToInt(Vector2 pDirection)
        {
			pDirection = pDirection.Rotated(-HALF_PI);
			return Mathf.RoundToInt(Mathf.Pow(2, Mathf.Atan2(pDirection.y, pDirection.x) / HALF_PI));
        }
	}
}