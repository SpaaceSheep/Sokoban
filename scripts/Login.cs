using Com.IsartDigital.HandleTheCrate.Level;
using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.HandleTheCrate.Sound;
using Com.IsartDigital.HandleTheCrate.Utils;
using Godot;
using System;
using System.Threading.Tasks;

namespace Com.IsartDigital.HandleTheCrate
{

    public class Login : Control
    {
        [Export] protected PackedScene menu;
        [Export] protected readonly NodePath userNamePath;
        [Export] protected readonly NodePath loginButtonPath;

        protected LineEdit UserNameNode { get; private set; }
        protected Button Loginbtn { get; private set; }
        public static Login Instance { get; private set; }

        private Login() : base() { }

        public override void _Ready()
        {
            if (Instance != null)
            {
                Free();
                GD.Print($"{nameof(Login)} Instance already exist, destroying the last added.");
                return;
            }
            Instance = this;

            Loginbtn = (Button)GetNode(loginButtonPath);
            Loginbtn.Connect(MainMenu.PRESSED, this, nameof(ToLogin));

            UserNameNode = (LineEdit)GetNode(userNamePath);
            TranslationServer.SetLocale("en"); //let it there to translate levelButton text

        }
        protected void ToLogin()
        {
            SoundManager.Instance.ValidateBtn();
            Main.userProfil = JsonProfilSystem.CheckSaveFile(UserNameNode.Text);

            Main.Instance.FadeOutFadeIn(this, nameof(InstantiateMenu));
   
        }

        public void InstantiateMenu()
        {
            GetParent().AddChild(menu.Instance());
            QueueFree();
        }


        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this)
                Instance = null;

            base.Dispose(pDisposing);
        }
    }
}