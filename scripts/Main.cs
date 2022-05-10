using Com.IsartDigital.HandleTheCrate.Sound;
using Com.IsartDigital.HandleTheCrate.Utils;
using Godot;
using System;
using GObject = Godot.Object;

namespace Com.IsartDigital.HandleTheCrate
{
    public class Main : Node
    {
        public static Profil userProfil;
         protected TextureRect vignette;
         public static Tween tween;

        [Export] protected float VignetteDuration;

        [Export] protected PackedScene menu;
        [Export] protected NodePath gameContainerMenuPath;
        [Export] protected PackedScene login;
        [Export] protected NodePath vignettePath;
        [Export] protected NodePath tweenPath;

        public bool inputsBlocked = false;
        public bool keyInputsBlocked = false;

        public static Main Instance { get; private set; }

        private Main() : base() { }

        public override void _Ready()
        {
            if (Instance != null)
            {
                Free();
                GD.Print($"{nameof(Main)} Instance already exist, destroying the last added.");
                return;
            }

            Instance = this;
            vignette = (TextureRect)GetNode(vignettePath);
            tween = (Tween)GetNode(tweenPath);

            VignetteFadeIn();
            InstantiateLogIn();
            SoundManager.Instance.pathPlayerMusic.Play();
            TranslationServer.SetLocale("en");
        }

        #region Vignette

        public void VignetteFadeIn()
        {
            tween.InterpolateProperty((ShaderMaterial)vignette.Material, "shader_param/vignette_opacity", 1f, 0, VignetteDuration);
            tween.Start();
        }

        public void VignetteFadeOut()
        {
            tween.InterpolateProperty((ShaderMaterial)vignette.Material, "shader_param/vignette_opacity", 1f, 0, VignetteDuration);
            tween.Start();
        }

        public float FadeOutFadeIn(GObject pObject = null, string pCallBack = null, int pLevel = -1)
        {
            vignette.Show();

            tween.InterpolateProperty((ShaderMaterial)vignette.Material, "shader_param/vignette_opacity", 0, 1f, VignetteDuration*0.8f);
            float lDelay = tween.GetRuntime();

            if(pLevel == -1) tween.InterpolateCallback(pObject, lDelay, pCallBack);
            else tween.InterpolateCallback(pObject, lDelay, pCallBack, arg1:pLevel);

            tween.InterpolateProperty((ShaderMaterial)vignette.Material, "shader_param/vignette_opacity", 1f, 0, VignetteDuration, delay: lDelay);
            tween.InterpolateCallback(vignette, tween.GetRuntime(), "hide");
            tween.Start();

            return tween.GetRuntime();
        }

        #endregion


        #region Init
        public void InstantiateLogIn()
        {
            GetNode(gameContainerMenuPath).AddChild(login.Instance());
        }

        public static void InitLevelsScore(int pSize)
        {
            userProfil.InitLevelsScore(pSize);
            SaveProfil();
        }
        #endregion

        #region Profil
        public static void UpdateProfil(int pLevelScore)
        {
            userProfil.UpdateBestLevelScore(pLevelScore); //update score level only if score is better
            userProfil.UpdateTotalScore();

            userProfil.UpdateCurrentLvlAndUnlockedLvls(); //update current level and levelUnlockedIndex
            
            SaveProfil();
        }

        public static void SaveProfil()
        {
            JsonProfilSystem.SaveProfil(userProfil);
        }
        #endregion
        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this)
                Instance = null;

            base.Dispose(pDisposing);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (inputsBlocked) GetTree().SetInputAsHandled();
        }
    }
}
