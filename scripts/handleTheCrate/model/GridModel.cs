using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.HandleTheCrate.Presenters;
using Com.IsartDigital.HandleTheCrate.Utils;
using Com.IsartDigital.Utils;
using Godot;

namespace Com.IsartDigital.HandleTheCrate.Model
{

    public static class GridModel
	{
		public const int LENGTH_X = 9;
		public const int LENGTH_Y = 9;
		public const int LENGTH_Z = 3;
		public const int HANDLE_LAYER = 2;
		public const int TARGET_LAYER = 1;

        private static Presenter[,,] grid = new Presenter[LENGTH_X, LENGTH_Y, LENGTH_Z];

		public static void ResetGrid()
        {
			grid = new Presenter[LENGTH_X, LENGTH_Y, LENGTH_Z];
        }

		//public pour detacher les poignees
		public static void SetGridAt(int pX, int pY, Presenter pValue, int pZ = 0)
		{
			if (pValue != null) pValue.position = new Vector2(pX, pY);

			grid[pY, pX, pZ] = pValue;
		}
		public static void SetGridAt(Vector2 pPosition, Presenter pValue, int pZ = 0)
		{
			if (pValue != null) pValue.position = pPosition;

			grid[(int)pPosition.y, (int)pPosition.x, pZ] = pValue;
		}

		public static bool IsEmpty(int pX, int pY)
		{
			return GetAt(pX, pY) == null;
		}
		public static bool IsEmpty(Vector2 pPosition)
		{
			return GetAt(pPosition) == null;
		}

		//recuperer l'objet sur une case
		public static Presenter GetAt(int pX, int pY, int pZ = 0)
		{
			if (!IsInBounds(pX, pY))
			{
				return null;
			}
			return grid[pY, pX, pZ];
		}
		public static Presenter GetAt(Vector2 pPosition, int pZ = 0)
		{
			if (!IsInBounds(pPosition))
			{
				return null;
			}
			return grid[(int)pPosition.y, (int)pPosition.x, pZ];
		}


		public static bool IsInBounds(int pX, int pY)
		{
			return IsInBounds(new Vector2(pX, pY));
		}
		public static bool IsInBounds(Vector2 pPosition)
		{
			return new Rect2(Vector2.Zero, new Vector2(LENGTH_X, LENGTH_Y)).HasPoint(pPosition);
		}


		public static void Print()
		{
			LevelInfo lLevel = SelectLevel.Instance.CurrentLevelModel;
			Presenter lMapObj;
			int lConfig = JsonParseHelper.OFFSET;

			for (int x = 0; x < LENGTH_X; x++)
			{
				for (int y = 0; y < LENGTH_Y; y++)
				{
					lMapObj = GetAt(x, y);
					lLevel.SetMap(x, y, lMapObj == null ? ' ' : (char)lMapObj.ToString()[0]);

					if (lMapObj is MobilePresenter) lConfig += ((MobilePresenter)lMapObj).HandleConfig;

					lMapObj = GetAt(x, y, TARGET_LAYER);
					if (lMapObj != null) lConfig += JsonParseHelper.TARGET_CODE;

					lMapObj = GetAt(x, y, HANDLE_LAYER);
					if (lMapObj != null) lConfig += JsonParseHelper.HANDLE_CODE;

					lLevel.SetOverlay(x, y, lConfig == JsonParseHelper.OFFSET ? ' ' : (char)lConfig);
					lConfig = JsonParseHelper.OFFSET;
				}
			}
			lLevel.Print();
		}
	}

}