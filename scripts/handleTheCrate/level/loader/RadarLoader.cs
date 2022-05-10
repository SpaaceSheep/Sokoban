using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.HandleTheCrate.utils.game;
using Com.IsartDigital.Utils;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.Level.Loader {
	
    public class RadarLoader : Control
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
        public PackedScene HandleConfigFacotry { get; protected set; }

        public static RadarLoader Instance { get; private set; }

        private RadarLoader ():base() {}

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(RadarLoader)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;
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
    }
}