using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate {
	
    public class Help : Control
    {
        [Export] NodePath helpbtns;
        protected const string PRESSED = "pressed";
        protected Button backBtn { get; private set; }
        public static Help Instance { get; private set; }

        private Help ():base() {}

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(Help)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;
            backBtn = GetNode<Button>(helpbtns);
            backBtn.Connect(PRESSED, this, nameof(ToGoBack));
        }
        protected void ToGoBack()
        {
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