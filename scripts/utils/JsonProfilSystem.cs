using Godot;
using GDictionary = Godot.Collections.Dictionary;

namespace Com.IsartDigital.HandleTheCrate.Utils
{
    public static class JsonProfilSystem
    {
        const string JSONSAVE_PATH = "user://saveTeamCannelle.save";
        const string GUEST = "Guest";
        const string EMPTY_NAME = "";
        const string SAVE_DONE = "Save done";

        public static GDictionary usersProfils = new GDictionary(); //called als by HighScores

        public static Profil CheckSaveFile(string pPlayer)
        {
            File lSaveFile = new File();

            //create a save file if not existing, else copy data stored 
            if (!lSaveFile.FileExists(JSONSAVE_PATH))
            {
                GD.Print("JsonProfilSystem : new save file created");

                lSaveFile.Open(JSONSAVE_PATH, File.ModeFlags.Write);

                Profil newProfil = CheckIfGuestProfil(pPlayer);

                usersProfils.Add(newProfil.name, newProfil.GetInfos());

                lSaveFile.StoreLine(JSON.Print(usersProfils, ""));

                lSaveFile.Close();

                return newProfil;
            }
            else
            {
                GD.Print("JsonProfilSystem : save file already existing");

                lSaveFile = JsonLoader.OpenJsonFile(JSONSAVE_PATH, File.ModeFlags.ReadWrite);
                usersProfils = JsonLoader.GetJsonResult(lSaveFile);
                lSaveFile.Close();

                return CheckProfil(pPlayer);
            }
        }

        public static Profil CheckProfil(string pPlayer)
        {
            if (pPlayer == EMPTY_NAME) pPlayer = GUEST;

            //I - check if the user name already exists or not
            if (usersProfils.Contains(pPlayer))
            {
                GD.Print("This profil already exists");

                if (pPlayer == GUEST)
                {
                    //II - if no login entered by user, reset "Guest" profil

                    GD.Print("Guest profil dedected, temporary profil created");
                    Profil lNewProfil = new Profil(GUEST);
                    SaveProfil(lNewProfil);

                    return lNewProfil;
                }
                else
                {
                    //II - get the user profil 
                    GDictionary lPlayerData = (GDictionary)usersProfils[pPlayer];

                    //III - transfert all user data in a new Profil struct
                    return new Profil(pPlayer,
                        lPlayerData["currentLevel"],
                        lPlayerData["levelUnlockedIndex"],
                        lPlayerData["levelsScore"],
                        lPlayerData["score"]);
                }
            }
            else
            {
                //I create new profil, save it and return it
                Profil newProfil = CheckIfGuestProfil(pPlayer);
                SaveProfil(newProfil);

                return newProfil;
            }
        }

        public static Profil CheckIfGuestProfil(string pPlayer)
        {
            if (pPlayer != GUEST && pPlayer != EMPTY_NAME && pPlayer != "guest")
            {
                GD.Print("A new profil has been created");
                return new Profil(pPlayer);
            }
            else
            {
                GD.Print("Guest profil dedected, temporary profil created");
                return new Profil(GUEST);
            }
        }

        public static void SaveProfil(Profil pPlayer)
        {
            //I - replace old profil verison by the new one (allow to reset guest profil and update profil already existing)
            usersProfils.Remove(pPlayer.name);
            usersProfils.Add(pPlayer.name,pPlayer.GetInfos());
 
            //II - Open the JsonFile in write mode
            File lSaveFile = JsonLoader.OpenJsonFile(JSONSAVE_PATH, File.ModeFlags.Write);

            //III - store the new data
            lSaveFile.StoreLine(JSON.Print(usersProfils));

            //IV - close new JsonFile
            lSaveFile.Close();

            GD.Print(SAVE_DONE);
        }
    }
}