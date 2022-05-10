using Com.IsartDigital.HandleTheCrate.Menu;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.Sound {
	
    public class SoundManager : Node
    {
        [Export] NodePath playerSound;
        [Export] NodePath playerMusic;
        [Export] NodePath movePlayer;

        private AudioStreamPlayer pathPlayerSound;
        public AudioStreamPlayer pathPlayerMusic;
        private AudioStreamPlayer pathMovePlayer;

        [Export] public AudioStream soundMovePlayer;
        [Export] public AudioStream soundCreate;

        [Export] public AudioStream soundHandle;
        [Export] public AudioStream soundBlocked;
        [Export] public AudioStream soundWin;

        [Export] private AudioStream btnValidate;
        [Export] public AudioStream btnRetry;
        [Export] public AudioStream btnLevel;
        [Export] public AudioStream btnUnlockLevel;
        [Export] public AudioStream btnUndo;
        [Export] public AudioStream btnRedo;

        [Export] private NodePath tweeMusicPath;
        private Tween tweeMusic;

        public bool isAudioOn = true;

        public static SoundManager Instance { get; private set; }

        private SoundManager ():base() {}

        public override void _Ready()
        {
            if (Instance != null){  
                Free();
                GD.Print($"{nameof(SoundManager)} Instance already exist, destroying the last added.");
                return;
            }
            
            Instance = this;

            pathPlayerSound = (AudioStreamPlayer)GetNode(playerSound);
            pathPlayerMusic = (AudioStreamPlayer)GetNode(playerMusic);
            pathMovePlayer = (AudioStreamPlayer)GetNode(movePlayer);
            tweeMusic = (Tween)GetNode(tweeMusicPath);
        }

        public void SoundMovePlayer(AudioStream pAudioName)
        {
            if (isAudioOn == true)
            {
                pathMovePlayer.Stream = pAudioName;
                pathMovePlayer.Play();
            }
        }

        public void ValidateBtn()
        {
            if (isAudioOn == true)
            {
                pathPlayerSound.Stream = btnValidate;
                pathPlayerSound.Play();
            }
        }

        public void MusicPlay()
        {
            if (isAudioOn == true)
            {
                pathPlayerMusic.Play();
            } else
            {
                pathPlayerMusic.Stop();
            }
        }

        public void SoundPlay(AudioStream pAudioName)
        {
            if (isAudioOn == true)
            {
                pathPlayerSound.Stream = pAudioName;
                pathPlayerSound.Play();
            }
        }

        public void MusicFadeOut(AudioStreamPlayer pMusic)
        {
            tweeMusic.InterpolateProperty((AudioStreamPlayer)pMusic, "volume_db", 0, -80, 10/*duration a changer*/, Tween.TransitionType.Quart);
            tweeMusic.Start();
        }

        protected override void Dispose(bool pDisposing)
        {
            if (pDisposing && Instance == this) 
                Instance = null;

            base.Dispose(pDisposing);
        }
    }
}