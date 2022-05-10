using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Menu {
	
    public class WinScreenStars : Control
    {
        public static WinScreenStars Instance { get; private set; }

        private WinScreenStars ():base() {}

        [Export] protected NodePath[] starPaths;

        protected List<TextureRect> stars = new List<TextureRect>();
        protected List<TextureRect> starsFilling = new List<TextureRect>();
        protected List<Tween> starsTween = new List<Tween>();
        protected List<CPUParticles2D> starsParticles = new List<CPUParticles2D>();

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(WinScreenStars)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;
        }

        public void Init()
        {
            stars = new List<TextureRect>();
            starsFilling = new List<TextureRect>();
            starsTween = new List<Tween>();
            starsParticles = new List<CPUParticles2D>();
            foreach (NodePath path in starPaths)
            {
                stars.Add((TextureRect)GetNode(path));
                starsFilling.Add((TextureRect)GetNode(path).GetNode("Fill"));
                starsTween.Add((Tween)GetNode(path).GetNode("Tween"));
                starsParticles.Add((CPUParticles2D)GetNode(path).GetNode("Particle"));
            }

            Color lTransparent = new Color(1, 1, 1, 0);
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                stars[i].SelfModulate = lTransparent;
                starsFilling[i].SelfModulate = lTransparent;
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }

        public void Stars(int pScore)
        {
            Color lTransparent = new Color(1, 1, 1, 0);
            Color lFilled = new Color(1, 1, 1, 1);
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                starsTween[i].InterpolateProperty(stars[i], "self_modulate", lTransparent, lFilled, 0.5f);
                starsTween[i].InterpolateProperty(stars[i], "rect_position", stars[i].RectPosition + Vector2.Down * 30, stars[i].RectPosition, 0.5f, Tween.TransitionType.Quad, Tween.EaseType.InOut);
                stars[i].SelfModulate = lTransparent;
                starsFilling[i].SelfModulate = lTransparent;
                starsTween[i].Start();
            }

            for (int i = 0; i < pScore; i++)
            {
                float lDelay = starsTween[i].GetRuntime() + i * 0.5f + 0.5f;
                starsTween[i].InterpolateProperty(starsFilling[i], "self_modulate", lTransparent, lFilled, 0.1f, delay: lDelay);
                starsTween[i].InterpolateProperty(stars[i], "rect_scale", Vector2.One * 1.5f, Vector2.One, 0.3f, Tween.TransitionType.Back, Tween.EaseType.Out, lDelay);
                starsTween[i].InterpolateCallback(this, lDelay, nameof(PlayParticles), i);
                starsTween[i].Start();
            }
        }

        protected void PlayParticles(int pIndex)
        {
            starsParticles[pIndex].Emitting = true;
        }
    }
}