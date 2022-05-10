using Com.IsartDigital.HandleTheCrate.Menu;
using Godot;
using System;
using GArray = Godot.Collections.Array;
using GDictionary = Godot.Collections.Dictionary;

namespace Com.IsartDigital.HandleTheCrate.Utils
{
    public class Profil
    {
        public string name;
        public int currentLevel;
        public int unlockedLevels;
        public int score;
        public GArray levelsScore;

        //constructor for new profils
        public Profil(string pName)
        {
            name = pName;
            currentLevel = 0;
            unlockedLevels = 0;
            levelsScore = new GArray();
            score = 0;
        }

        //constructor for existings files coming from jsonFile
        public Profil(string pName, object pCurrentLevel, object pLevelUnlockedIndex, object pScorePerLevel, object pScore)
        {
            name = pName;
            currentLevel = int.Parse(pCurrentLevel.ToString());
            unlockedLevels = int.Parse(pLevelUnlockedIndex.ToString());
            levelsScore = (GArray)pScorePerLevel;
            score = int.Parse(pScore.ToString());      
        }

        public void InitLevelsScore(int pSize) 
        {
            if (levelsScore.Count < pSize)
            {
                for (int i = 0; i < pSize; i++)
                {
                    levelsScore.Add(0);
                }
            }
        }

        public GDictionary GetInfos()
        {
            GDictionary lInfos = new GDictionary {
                {"currentLevel", currentLevel},
                {"levelUnlockedIndex", unlockedLevels},
                {"levelsScore", levelsScore},
                {"score", score }
            };

            return lInfos;
        }

        public void UpdateCurrentLvlAndUnlockedLvls()
        {
            int lMaxLvlIndex = SelectLevel.MaxLevelIndex;

            if(currentLevel < lMaxLvlIndex) currentLevel++;
            if(currentLevel == Mathf.Clamp(currentLevel, unlockedLevels, lMaxLvlIndex)) unlockedLevels = currentLevel;
        }

        public void UpdateBestLevelScore(int pLevelScore)
        {
            //if current level score if lower than level best score
            int lBestScore = Convert.ToInt32(levelsScore[currentLevel]);

            if (pLevelScore > lBestScore) levelsScore[currentLevel] = pLevelScore;
        }

        public void UpdateTotalScore()
        {
            int lIterationNb = levelsScore.Count;
            int lScore = 0;

            for (int i = 0; i < lIterationNb; i++)
            {
                lScore += Convert.ToInt32(levelsScore[i]);
            }

            score = lScore;
        }
    }

    public class PlayerData
    {
        public string name;
        public int score;

        public PlayerData(string pName, int pScore)
        {
            name = pName;
            score = pScore;
        }
    }
}