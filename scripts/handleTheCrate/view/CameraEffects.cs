using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View {
	
    public class CameraEffects : Camera2D
    {
        public static CameraEffects Instance { get; private set; }

        private CameraEffects ():base() {}

       public Tween tween;

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(CameraEffects)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;

            tween = (Tween)GetNode("Tween");
            GlobalPosition = Vector2.Zero;
        }

        public void ZoomAt(ObjectIsoView pObject, float pDelay = 0)
        {
            Main.Instance.inputsBlocked = true;
            MakeCurrent();
            tween.InterpolateProperty(this, "global_position", GlobalPosition, pObject.GlobalPosition + Vector2.Up * 30f - (GetViewportRect().Size / 2f) * 0.3f, 1f, Tween.TransitionType.Quad, Tween.EaseType.InOut, delay:pDelay);
            tween.InterpolateProperty(this, "zoom", Zoom, Vector2.One * 0.3f, 1f, Tween.TransitionType.Quad, Tween.EaseType.InOut, delay: pDelay);
            tween.Start();
            
            GlobalPosition = pObject.GlobalPosition + Vector2.Up * 30f - (GetViewportRect().Size / 2f) * 0.3f;
            Zoom = Vector2.One * 0.3f;
        }

        public void Reset(float pDelay = 0)
        {
            tween.InterpolateProperty(this, "global_position", GlobalPosition, Vector2.Zero, 1f, Tween.TransitionType.Quad, Tween.EaseType.InOut, delay: pDelay);
            tween.InterpolateProperty(this, "zoom", Zoom, Vector2.One, 1f, Tween.TransitionType.Quad, Tween.EaseType.InOut, delay: pDelay);
            tween.InterpolateCallback(this, tween.GetRuntime(), nameof(Disable));
            tween.Start();

            GlobalPosition = Vector2.Zero;
            Zoom = Vector2.One;
        }
       
        public void Disable()
        {
            ClearCurrent();
            Main.Instance.inputsBlocked = false;
            Main.Instance.keyInputsBlocked = false;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }
    }
}