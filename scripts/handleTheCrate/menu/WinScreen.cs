using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.Menu {
	
    public class WinScreen : Control
    {
        [Export] protected NodePath quitBtnPath;
        [Export] protected NodePath nextBtnPath;
        protected Button quitBtn;
        protected Button nextBtn;
        protected TextureProgress titleFrame;
        protected TextureRect title;
        protected Tween tween;



        public static WinScreen Instance { get; private set; }

        private WinScreen ():base() {}

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(WinScreen)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;
            quitBtn = (Button)GetNode(quitBtnPath);
            nextBtn = (Button)GetNode(nextBtnPath);

            quitBtn.Connect("pressed", this, nameof(Quit));
            nextBtn.Connect("pressed", this, nameof(Next));
            tween = (Tween)GetNode("Tween");

            titleFrame = (TextureProgress)GetNode("Title");
            title = (TextureRect)titleFrame.GetNode("Text");
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }

        public void Appear(int pScore)
        {
            Show();
            quitBtn.Disabled = true;
            nextBtn.Disabled = true;

            TextureRect lBackground = (TextureRect)GetNode("Background");

            tween.InterpolateProperty(titleFrame, "value", 0, 1, 0.3f, Tween.TransitionType.Cubic, Tween.EaseType.Out);
            tween.InterpolateProperty(title, "self_modulate:a", 0, title.SelfModulate.a, 0.5f);
            tween.InterpolateProperty(lBackground, "self_modulate:a", 0, lBackground.SelfModulate.a, 0.5f);
            tween.InterpolateProperty(titleFrame, "rect_position:y", titleFrame.RectPosition.y + 100, titleFrame.RectPosition.y, 1f, Tween.TransitionType.Cubic, Tween.EaseType.InOut, tween.GetRuntime() + 0.2f);
            titleFrame.RectPosition += Vector2.Down * 100;

            tween.InterpolateCallback(WinScreenStars.Instance, tween.GetRuntime() + 0.2f, nameof(WinScreenStars.Instance.Stars), Mathf.Clamp(pScore,0,3));
            WinScreenStars.Instance.Init();

            float lDuration = tween.GetRuntime() + 0.5f + pScore * 0.5f;
            tween.InterpolateProperty(nextBtn, "rect_position:y", nextBtn.RectPosition.y + 50, nextBtn.RectPosition.y, 0.7f, Tween.TransitionType.Cubic, Tween.EaseType.Out, lDuration + 0.2f);
            tween.InterpolateProperty(nextBtn, "self_modulate:a", 0, nextBtn.SelfModulate.a, 0.4f, delay : lDuration + 0.2f);

            tween.InterpolateProperty(quitBtn, "rect_position:y", quitBtn.RectPosition.y + 50, quitBtn.RectPosition.y, 0.7f, Tween.TransitionType.Cubic, Tween.EaseType.Out, lDuration + 0.4f);
            tween.InterpolateProperty(quitBtn, "self_modulate:a", 0, quitBtn.SelfModulate.a, 0.4f, delay: lDuration + 0.4f);

            tween.InterpolateCallback(this, tween.GetRuntime(), nameof(EnableClick));

            Color lTransparent = new Color(1, 1, 1, 0);
            nextBtn.RectPosition += Vector2.Down * 50;
            quitBtn.RectPosition += Vector2.Down * 50;
            nextBtn.SelfModulate = lTransparent;
            quitBtn.SelfModulate = lTransparent;

            tween.Start();
        }

        public void Quit()
        {
            LevelContainer.Instance.ReturnToMenu();
        }

        public void Next()
        {
            Hide();
            LevelContainer.Instance.Next();
        }

        public void EnableClick()
        {
            quitBtn.Disabled = false;
            nextBtn.Disabled = false;
        }
    }
}