using Com.IsartDigital.HandleTheCrate.Level;
using Com.IsartDigital.Utils;
using Com.IsartDigital.Utils.Game;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.Menu.Loader {
	
    public class IsoLoader : Control
    {
        [Export]
        protected NodePath objectParent;
        
        [Export]
        public PackedScene MovableWallFactory { get; protected set; }
        [Export]
        public PackedScene WallFactory { get; protected set; }
        [Export]
        public PackedScene BoxFactory { get; protected set; }
        [Export]
        public PackedScene PlayerFactory { get; protected set; }
        [Export]
        public PackedScene HandleFactory { get; protected set; }
        [Export]
        public PackedScene TargetFactory { get; protected set; }
        [Export]
        public PackedScene[] HandleConfigFacotry { get; protected set; }

        protected Tween tween;

        public NodePath ObjectParent { get { return objectParent; } }

        public static IsoLoader Instance { get; private set; }

        private IsoLoader ():base() {}

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(IsoLoader)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;

            tween = (Tween)GetNode("Tween");
            Init();
        }

        public Node GetObjectParent()
        {
            return GetNode(objectParent);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }

        protected void Init()
        {
            Sprite lBackground = (Sprite)GetNode("Background");
            float lPos = lBackground.Position.y + 200;
            tween.InterpolateProperty(lBackground, "position:y", lPos, lBackground.Position.y, 0.5f, Tween.TransitionType.Quad, Tween.EaseType.Out);
            tween.InterpolateProperty(lBackground, "self_modulate:a", 0, 1f, 0.5f);
            tween.Start();
        }
    }
}