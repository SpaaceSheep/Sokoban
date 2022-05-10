using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.Presenters;
using Com.IsartDigital.HandleTheCrate.Sound;
using Com.IsartDigital.HandleTheCrate.utils.game;
using Com.IsartDigital.HandleTheCrate.Utils;
using Com.IsartDigital.Utils.Game;
using Godot;
using System;
using System.Threading.Tasks;

namespace Com.IsartDigital.HandleTheCrate.Menu
{
    public class SelectLevel : Control
    {
        protected const string PRESSED = "pressed";
        protected const string levelNodePath = "/root/Main/GameContainer/Level";

        [Export] protected NodePath goBackBtnPath;
        protected Button goBackBtn;
        [Export] protected NodePath unlockLevelPath;
        public Button unlocklevelBtn;

        public static SelectLevel Instance { get; private set; }

        public LevelInfo CurrentLevel { get; private set; }
        public LevelInfo CurrentLevelModel { get; private set; }
        [Export] protected int[] levelsScoreReferences;
        [Export] protected int[] levelsBestScoreReferences;

        [Export] protected PackedScene levelContainerPackedScene;
        Node levelContainerScene;
        public static int MaxLevelIndex { get; private set; }
        protected static Godot.Collections.Array levelDesignArray;

        private SelectLevel() : base() { }

        public override void _Ready()
        {
            if (Instance != null)
            {
                Free();
                GD.Print($"{nameof(SelectLevel)} Instance already exist, destroying the last added.");
                return;
            }

            Instance = this;
            Hide();

            goBackBtn = (Button)GetNode(goBackBtnPath);
            goBackBtn.Connect(PRESSED, this, nameof(ToGoBack));

            unlocklevelBtn = (Button)GetNode(unlockLevelPath);
            unlocklevelBtn.Connect(PRESSED, this, nameof(UnlockAllLevels));

            levelDesignArray = JsonLoader.GetLevelDesignArray("leveldesign.json");
            MaxLevelIndex = levelDesignArray.Count-1;
            IsoManager.Init(137, 65);
            RadarManager.Init(70, 70);
        }

        public void UnlockAllLevels()
        {
            SoundManager.Instance.SoundPlay(SoundManager.Instance.btnUnlockLevel);
            Main.userProfil.unlockedLevels = MaxLevelIndex;
            Main.SaveProfil();
            GD.Print("SelectLevel: all levels are unlocked");
        }

        public void LoadCurrentLevel()
        {
            SoundManager.Instance.SoundPlay(SoundManager.Instance.btnLevel);
            int lLevel = Main.userProfil.currentLevel;

            LoadLevel(lLevel);   
        }

        public void LoadLevel(int pLevelID)
        {
            if(pLevelID <= Main.userProfil.unlockedLevels)
            {
                Main.Instance.FadeOutFadeIn(this, nameof(InstantiateLevel), pLevelID);  
            }
            else
            {
                GD.Print("level locked");
            }
        }

        protected void InstantiateLevel(int pLevelID)
        {
            levelContainerScene = levelContainerPackedScene.Instance();
            GetNode(levelNodePath).AddChild(levelContainerScene);

            int levelIDClamp = Mathf.Clamp(pLevelID, 0, MaxLevelIndex);
            CurrentLevel = JsonLoader.GetCharGrid(levelDesignArray, levelIDClamp);
            Main.userProfil.currentLevel = pLevelID;
            Main.SaveProfil();
            CurrentLevelModel = JsonLoader.GetCharGrid(levelDesignArray, levelIDClamp);
            LevelContainer.Instance.CheckIfTuto();

            Hide();
            MainMenu.Instance.Hide();
            LevelContainer.Instance.Show();
            LevelContainer.Instance.DisplayNewScoreRef(levelsScoreReferences[pLevelID]);
            LevelContainer.Instance.DisplayNewBestScoreRef(levelsBestScoreReferences[pLevelID]);
            GridPresenter.LoadLevel();
            SoundManager.Instance.pathPlayerMusic.Stop();
        }

        public void ChangeLevel(int pLevelID)
        {
            if (pLevelID <= Main.userProfil.unlockedLevels)
            {
                int levelIDClamp = Mathf.Clamp(pLevelID, 0, MaxLevelIndex);
                CurrentLevel = JsonLoader.GetCharGrid(levelDesignArray, levelIDClamp);
                Main.userProfil.currentLevel = pLevelID;
                Main.SaveProfil();
                CurrentLevelModel = JsonLoader.GetCharGrid(levelDesignArray, levelIDClamp);
                LevelContainer.Instance.CheckIfTuto();
                LevelContainer.Instance.DisplayNewScoreRef(levelsScoreReferences[pLevelID]);
            }
        }

        protected void ToGoBack()
        {
            SoundManager.Instance.ValidateBtn();
            Main.Instance.FadeOutFadeIn(this, "hide");
        }
        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this)
                Instance = null;

            base.Dispose(pDisposing);
        }
    }
}
