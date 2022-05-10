using Com.IsartDigital.HandleTheCrate.View.Iso;
using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Menu
{
	
    public class TutoNode : Control
    {
        public static TutoNode Instance { get; private set; }

        private TutoNode ():base() {}

        Popup_Tuto popup;
        List<Area2D> triggers = new List<Area2D>();

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(TutoNode)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;

            Area2D trigger = new Area2D();

            for (int i = 1; i < GetChildCount(); i++)
            {
                trigger = (Area2D)GetChild(i);
                trigger.Connect("body_entered", this, nameof(DisplayTuto));
                trigger.Connect("body_exited", this, nameof(HideTuto));
                triggers.Add(trigger);
            }

            popup = (Popup_Tuto)GetChild(0);
            if ((int)Main.userProfil.levelsScore[0] != 0 || Main.userProfil.currentLevel !=0) QueueFree(); 
        }

        private void DisplayTuto(PhysicsBody2D pPlayer)
        {
            popup.ShowTuto();
        }

        private void HideTuto(PhysicsBody2D pPlayer)
        {
            popup.Close();
            triggers[0].QueueFree();
            triggers.RemoveAt(0);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }
    }
}