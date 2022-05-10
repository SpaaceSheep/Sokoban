using Com.IsartDigital.HandleTheCrate.Menu;
using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.View.Iso
{
	
    public class Popup_Tuto : RichTextLabel
    {

        public static int index = 0;
        string content;
        const string WAVE = "[wave amp=50 freq=2]";
        const string CRATE = WAVE + "[color=purple] crate [/color][/wave]";
        const string CRATES = WAVE + "[color=purple] crates [/color][/wave]";
        const string MAGNETS = WAVE + "[color=grey] magnets [/color][/wave]";
        const string MAGNET = WAVE + "[color=grey] magnet [/color][/wave]";
        Tween tween;

        public static Popup_Tuto Instance { get; private set; }

        private Popup_Tuto ():base()
        {
            tween = new Tween();
            AddChild(tween);
        }

        List<string> tutos = new List<string>
        {
            $"[center] Move the {CRATE} to the {WAVE}[color=green] beam light [/color][/wave] to complete the level [/center]",
            $"[center] {MAGNETS} are attracted by {CRATES}. They can't be removed [/center]",
            $"[center] A {MAGNET} stucks on the {CRATE} only if the side is free[/center]",
            $"[center] When the player is connected to a {MAGNET}, what is connected to the {MAGNET} follows the player as long as the link is not severed [/center]",
            $"[center] {CRATES} step over {MAGNETS} when their side is not free [/center]"
        };

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(Popup_Tuto)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;
            Hide();
        }

        public void ShowTuto()
        {
            Show();
            BbcodeText = tutos[index];
            PercentVisible = 0;
            Main.Instance.inputsBlocked = true; //block player moves
            tween.InterpolateProperty(this, "percent_visible", 0, 1, 1.5f);
            tween.InterpolateProperty(Main.Instance, "inputsBlocked", 1, 0, tween.GetRuntime() + 0.5f);
            tween.Start(); 
        }

        public void Close()
        {
            Hide();
            if (index == tutos.Count) QueueFree();
            else ++index;
        }

        protected override void Dispose(bool pDisposing)
        {
            index = 0;

            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }
    }
}