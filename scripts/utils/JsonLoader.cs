using Godot;
using Godot.Collections;

namespace Com.IsartDigital.HandleTheCrate.Utils
{
	
	public static class JsonLoader
	{
		private const string JSON_PATH = "res://game/level/levelDesign.json";
		private const int GRID_MAX_SIZE = 9;

		#region public methods

		public static Array GetLevelDesignArray(string pJsonFile)
		{
			//I - open jSonFile and convert it in a dictionnary to reach the key "LevelDesign"
			File lJsonFile = OpenJsonFile(JSON_PATH, File.ModeFlags.Read);
			Dictionary JsonFileResult = GetJsonResult(lJsonFile);

			//I - return levelDesign which is a Godot Array
			return JsonFileResult["levelDesign"] as Array;
		}

		public static LevelInfo GetCharGrid(Array pLevelDesign, int pLevelIndex)
		{
			Array[] lLevelInfoRaw = GetMapAndOverlayOfLevel(pLevelDesign, pLevelIndex);

			//I - Convert array of objects in array of string
			string[] lMap = ConvertArrayInStrings(lLevelInfoRaw[0]);
			string[] lOverlay = ConvertArrayInStrings(lLevelInfoRaw[1]);

			//II - Convert string[9] in char[9,9] to match grid cells
			LevelInfo levelInfo = new LevelInfo();
			levelInfo.map = ConvertStringsInChars(lMap);
			levelInfo.mapOverlay = ConvertStringsInChars(lOverlay);

			//III - return the one your interested in
			return levelInfo;
		}

		public static void DisplayCharElements(char[,] pChar)
		{
			GD.Print("The Char[,] has ", pChar.Length, " elements");

			for (int i = 0; i < GRID_MAX_SIZE; i++)
			{
				for (int j = 0; j < GRID_MAX_SIZE; j++)
				{
					GD.Print($"cell ({i}, {j}) {pChar[i, j]} - {(int)pChar[i, j]}");
				}
			}
		}

		public static File OpenJsonFile(string pPath, File.ModeFlags pMode)
		{
			File lJsonFile = new File();

			//I - Check if the file exist or exit the method
			if (lJsonFile.FileExists(pPath))
			{
				GD.Print("File: " + pPath + " Found");
			}
			else
			{
				GD.Print("File: " + pPath + " not Found \n Please, check path spelling");
				return null;
			}

			//II - open the file
			lJsonFile.Open(pPath, pMode);

			return lJsonFile;
		}

		public static Dictionary GetJsonResult(File pJsonFile)
		{
			//III - convert it in string
			string lJsonText = pJsonFile.GetAsText();

			//IV - Parse the string
			// type var because JSON.PARSE().Result return a either a Godot Dictionnary, a Godot Array or an Error
			var lJsonParse = JSON.Parse(lJsonText);

			Dictionary lJsonResult = lJsonParse.Result as Dictionary;

			return lJsonResult;
		}

		#endregion

		#region private methods

		/// <summary> Don't forget to write the extension .json , or the file won't be found</summary>


		private static Array[] GetMapAndOverlayOfLevel(Array pLevelDesign, int pLevelIndex)
		{
			//II - get data of the pLevelIndex key which are in a odot Dictionnary
			//(cast in another type doesn't work)
			Dictionary lLevels = pLevelDesign[pLevelIndex] as Dictionary;

			//III - The level has two data : the map and mapOverlay. They are Array.
			Array lLevel = lLevels["map"] as Array;
			Array lOverlay = lLevels["mapOverlay"] as Array;
			Array[] lAllArray = { lLevel, lOverlay };

			GD.Print($"Level {pLevelIndex} loaded");

			return lAllArray;
		}

		private static string[] ConvertArrayInStrings(Array pArray)
		{
			//III - convert Array in string[9], representing one string per grid row.
			string[] stringArray = new string[9];

			for (int i = 0; i < pArray.Count; i++)
			{
				stringArray[i] = pArray[i].ToString();
			}

			return stringArray;
		}

		private static char[,] ConvertStringsInChars(string[] pStrings)
		{
			char[] lChar;
			char[,] lCharArray = new char[9, 9];

			for (int i = 0; i < pStrings.Length; i++)
			{
				lChar = pStrings[i].ToCharArray();

				for (int j = 0; j < lChar.Length; j++)
				{
					lCharArray[i, j] = lChar[j];
				}
			}
			return lCharArray;
		}
        #endregion
    }

    public static class CopyOfJsonLoader
    {
        private const string JSON_PATH = "res://game/level/levelDesign.json";
        private const int GRID_MAX_SIZE = 9;

        #region public methods

        public static Array GetLevelDesignArray(string pJsonFile)
        {
            //I - open jSonFile and convert it in a dictionnary to reach the key "LevelDesign"
            File lJsonFile = OpenJsonFile(JSON_PATH, File.ModeFlags.Read);
            Dictionary JsonFileResult = GetJsonResult(lJsonFile);

            //I - return levelDesign which is a Godot Array
            return JsonFileResult["levelDesign"] as Array;
        }

        public static LevelInfo GetCharGrid(Array pLevelDesign, int pLevelIndex)
        {
            Array[] lLevelInfoRaw = GetMapAndOverlayOfLevel(pLevelDesign, pLevelIndex);

            //I - Convert array of objects in array of string
            string[] lMap = ConvertArrayInStrings(lLevelInfoRaw[0]);
            string[] lOverlay = ConvertArrayInStrings(lLevelInfoRaw[1]);

            //II - Convert string[9] in char[9,9] to match grid cells
            LevelInfo levelInfo = new LevelInfo();
            levelInfo.map = ConvertStringsInChars(lMap);
            levelInfo.mapOverlay = ConvertStringsInChars(lOverlay);

            //III - return the one your interested in
            return levelInfo;
        }

        public static void DisplayCharElements(char[,] pChar)
        {
            GD.Print("The Char[,] has ", pChar.Length, " elements");

            for (int i = 0; i < GRID_MAX_SIZE; i++)
            {
                for (int j = 0; j < GRID_MAX_SIZE; j++)
                {
                    GD.Print($"cell ({i}, {j}) {pChar[i, j]} - {(int)pChar[i, j]}");
                }
            }
        }

        public static File OpenJsonFile(string pPath, File.ModeFlags pMode)
        {
            File lJsonFile = new File();

            //I - Check if the file exist or exit the method
            if (lJsonFile.FileExists(pPath))
            {
                GD.Print("File: " + pPath + " Found");
            }
            else
            {
                GD.Print("File: " + pPath + " not Found \n Please, check path spelling");
                return null;
            }

            //II - open the file
            lJsonFile.Open(pPath, pMode);

            return lJsonFile;
        }

        public static Dictionary GetJsonResult(File pJsonFile)
        {
            //III - convert it in string
            string lJsonText = pJsonFile.GetAsText();

            //IV - Parse the string
            // type var because JSON.PARSE().Result return a either a Godot Dictionnary, a Godot Array or an Error
            var lJsonParse = JSON.Parse(lJsonText);

            Dictionary lJsonResult = lJsonParse.Result as Dictionary;

            return lJsonResult;
        }

        #endregion

        #region private methods

        /// <summary> Don't forget to write the extension .json , or the file won't be found</summary>


        private static Array[] GetMapAndOverlayOfLevel(Array pLevelDesign, int pLevelIndex)
        {
            //II - get data of the pLevelIndex key which are in a odot Dictionnary
            //(cast in another type doesn't work)
            Dictionary lLevels = pLevelDesign[pLevelIndex] as Dictionary;

            //III - The level has two data : the map and mapOverlay. They are Array.
            Array lLevel = lLevels["map"] as Array;
            Array lOverlay = lLevels["mapOverlay"] as Array;
            Array[] lAllArray = { lLevel, lOverlay };

            GD.Print($"Level {pLevelIndex} loaded");

            return lAllArray;
        }

        private static string[] ConvertArrayInStrings(Array pArray)
        {
            //III - convert Array in string[9], representing one string per grid row.
            string[] stringArray = new string[9];

            for (int i = 0; i < pArray.Count; i++)
            {
                stringArray[i] = pArray[i].ToString();
            }

            return stringArray;
        }

        private static char[,] ConvertStringsInChars(string[] pStrings)
        {
            char[] lChar;
            char[,] lCharArray = new char[9, 9];

            for (int i = 0; i < pStrings.Length; i++)
            {
                lChar = pStrings[i].ToCharArray();

                for (int j = 0; j < lChar.Length; j++)
                {
                    lCharArray[i, j] = lChar[j];
                }
            }
            return lCharArray;
        }
        #endregion
    }
}