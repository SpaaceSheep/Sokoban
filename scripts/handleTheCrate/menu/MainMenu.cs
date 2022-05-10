using Com.IsartDigital.HandleTheCrate.Sound;
using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Menu
{
    public class MainMenu : Control
    {
        public const string PRESSED = "pressed";
        protected const string WELCOME = "[center]Welcome: [/center]";

        [Signal] delegate void ChangeLanguage();

        [Export] protected NodePath selectLevelPath;
        [Export] protected NodePath openHelpPath;
        [Export] protected NodePath welcomeTextPath;
        [Export] protected List<NodePath> boxContainerPath;
        [Export] protected Texture soundOnTexture;
        [Export] protected Texture soundOffTexture;
        [Export] protected Texture laFrance;
        [Export] protected Texture lesAnglois;
        [Export] protected PackedScene highScoresScene;
        [Export] protected NodePath creditBtnPath;
        [Export] protected NodePath creditScnPath;

        protected Button Playbtn { get; private set; }
        protected Button Levelsbtn { get; private set; }
        protected Button Quitbtn { get; private set; }
        protected TextureButton Soundbtn { get; private set; }
        protected TextureButton Languagebtn { get; private set; }
        protected Button HighScores { get; private set; }
        protected Button Helpbtn { get; private set; }
        protected Button CreditBtn { get; private set; }
        protected Control CreditScene { get; private set; }

        protected const string EN = "en";
	    protected const string FR = "fr";

        protected RichTextLabel WelcomeText { get; private set; }

        public static MainMenu Instance { get; private set; }
        private MainMenu() : base() { }

        public override void _Ready()
        {
            if (Instance != null)
            {
                Free();
                GD.Print($"{nameof(MainMenu)} Instance already exist, destroying the last added.");
                return;
            }

            Instance = this;

            Playbtn = (Button)GetNode(boxContainerPath[0]);
            Playbtn.Connect(PRESSED, this, nameof(ToPlay));

            Levelsbtn = (Button)GetNode(boxContainerPath[1]);
            Levelsbtn.Connect(PRESSED, this, nameof(ToSelect));

            HighScores = (Button)GetNode(boxContainerPath[2]);
            HighScores.Connect(PRESSED, this, nameof(ToHighScores));

            Helpbtn = (Button)GetNode(boxContainerPath[3]);
            Helpbtn.Connect(PRESSED, this, nameof(ToHelp));

            CreditBtn = (Button)GetNode(creditBtnPath);
            CreditBtn.Connect(PRESSED, this, nameof(ToCredit));
            CreditScene = (Control)GetNode(creditScnPath);

            Soundbtn = (TextureButton)GetNode(boxContainerPath[4]);
            Soundbtn.Connect(PRESSED, this, nameof(ToSound));

            Languagebtn = (TextureButton)GetNode(boxContainerPath[5]);
            Languagebtn.Connect(PRESSED, this, nameof(ToLanguage));

            WelcomeText = (RichTextLabel)GetNode(welcomeTextPath);

            Main.InitLevelsScore(SelectLevel.MaxLevelIndex+1);//only here because SelectLevel is initialized, important to let the +1
            WelcomeUserMessage();

        }

        private void WelcomeUserMessage()
        {
            WelcomeText.BbcodeText = $"[center][wave amp = 50] {WelcomeText.BbcodeText}[color=aqua]{Main.userProfil.name}[/color][/wave][/center]";
        }

        protected void ToPlay()
        {
            SelectLevel.Instance.LoadCurrentLevel();
        }

        protected void ToSelect()
        {
            SoundManager.Instance.ValidateBtn();
            Control levelselection = GetNode<Control>(selectLevelPath);
            Main.Instance.FadeOutFadeIn(levelselection, "show");
        }

        protected void ToQuit()
        {
            SoundManager.Instance.ValidateBtn();
            GetTree().Quit();
        }

        protected void ToHelp()
        {
            //PanelContainer openhelp = GetNode<PanelContainer>(openHelpPath);
            //openhelp.Show();
            SoundManager.Instance.ValidateBtn();
            ToLogIn();
        }

        protected void ToCredit()
        {
            Main.Instance.FadeOutFadeIn(CreditScene, "show");
        }



        protected void ToHighScores()
        {
            SoundManager.Instance.ValidateBtn();
            Main.Instance.FadeOutFadeIn(this, nameof(ToHighScoresGroupMethod));
        }
        private void ToHighScoresGroupMethod()
        {
            AddChild(highScoresScene.Instance());
        }

        protected void ToLogIn()
        {
            SoundManager.Instance.ValidateBtn();
            Main.Instance.FadeOutFadeIn(this, nameof(ToLogInGroupMethod));
        }

        private void ToLogInGroupMethod()
        {
            Main.Instance.InstantiateLogIn();
            QueueFree();
        }

        protected const string ON = "ON";
        protected const string OFF = "OFF";

        protected void ToSound()
        {
            SoundManager.Instance.ValidateBtn();
            if (Soundbtn.TextureNormal == soundOnTexture)
            {
                Soundbtn.TextureNormal = soundOffTexture;
                SoundManager.Instance.isAudioOn = false;
            }
            else
            {
                Soundbtn.TextureNormal = soundOnTexture;
                SoundManager.Instance.isAudioOn = true;
            }
            SoundManager.Instance.MusicPlay();
        }

        protected void ToLanguage()
        {
            SoundManager.Instance.ValidateBtn();
            if (TranslationServer.GetLocale() == EN)
            {
                Languagebtn.TextureNormal = laFrance;
                TranslationServer.SetLocale(FR);
            }
            else
            {
                Languagebtn.TextureNormal = lesAnglois;
                TranslationServer.SetLocale(EN);
            }

            EmitSignal(nameof(ChangeLanguage));
        }

        protected override void Dispose(bool pDisposing)
        {
            ButtonLevel.ResetGobalLevelIndex();

            if (pDisposing && Instance == this)
                Instance = null;


            base.Dispose(pDisposing);
        }
    }
}
