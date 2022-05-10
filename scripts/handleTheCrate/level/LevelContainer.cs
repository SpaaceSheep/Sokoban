using Godot;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.Level;
using Com.IsartDigital.HandleTheCrate.Presenters;
using Com.IsartDigital.HandleTheCrate.Sound;
using System;
using System.Threading.Tasks;

namespace Com.IsartDigital.HandleTheCrate.Menu
{
	public class LevelContainer : Control
	{
		[Export(PropertyHint.Range, "0, 5, 0.1")] public float TargetApparition { get; private set; }
		[Export(PropertyHint.Range, "0, 10, 0.1")] public float TargetParticulesApparition { get; private set; }
		[Export(PropertyHint.Range, "0, 5, 0.1")] public float CrateApparition { get; private set; }
		[Export(PropertyHint.Range, "0, 5, 0.1")] public float HandlesApparition { get; private set; }
		[Export(PropertyHint.Range, "0, 5, 0.1")] public float PlayerApparition { get; private set; }
		[Export(PropertyHint.Range, "0, 5, 0.1")] public float SpeedApparition { get; private set; }

		protected const string PRESSED = "pressed";

		[Export] readonly NodePath quitButtonPath;
		[Export] readonly NodePath undoButtonPath;
		[Export] readonly NodePath redoButtonPath;
		[Export] readonly NodePath retryButtonPath;

		protected Button QuitButton { get; private set; }
		protected Button UndoButton { get; private set; }
		protected Button RedoButton { get; private set; }
		protected Button RetryButton { get; private set; }

		[Export] protected int CELL_SIZE = 50;
		[Export] readonly PackedScene isoScenePacked;
		[Export] readonly PackedScene radarScenePacked;

		[Export] readonly NodePath musicPlayerPath;
		protected AudioStreamPlayer musicPlayer;

		[Export] readonly NodePath scorePath;
		[Export] readonly NodePath scoreRefPath;

		protected Label scoreLabel;
		protected Label scoreRefLabel;

		protected const int BEST_VICTORY_POINT = 4;
		protected const int MAX_VICTORY_POINT = 3;
		protected const int PAR_VICTORY_POINT = 2;
		protected const int MIN_VICTORY_POINT = 1;

		Tween tween;

		public int Score { get; protected set; }
		public int scoreRef = 0;
		public int bestScoreRef = 0;
		private bool tutoVisible = true;

        public static LevelContainer Instance { get; private set; }

		private LevelContainer() : base() { }

		public override void _Ready()
		{
			if (Instance != null)
			{
				Free();
				GD.Print($"{nameof(LevelContainer)} Instance already exist, destroying the last added.");
				return;
			}

			Instance = this;


			scoreLabel = (Label)GetNode(scorePath);
			scoreRefLabel = (Label)GetNode(scoreRefPath);

			Score = 0;
			UpdateScore(Score);

			tween = (Tween)GetNode("Tween");

			InitButtons();
			TutoSettings();
			Main.Instance.inputsBlocked = true;
			if (SoundManager.Instance.isAudioOn == true)
            {
				musicPlayer = (AudioStreamPlayer)GetNode(musicPlayerPath);
				musicPlayer.Play();
			}
		}

		protected void InitButtons()
		{
			QuitButton = (Button)GetNode(quitButtonPath);
			UndoButton = (Button)GetNode(undoButtonPath);
			RedoButton = (Button)GetNode(redoButtonPath);
			RetryButton = (Button)GetNode(retryButtonPath);

			QuitButton.Connect(PRESSED, this, nameof(ReturnToMenu));
			UndoButton.Connect(PRESSED, this, nameof(Undo));
			RedoButton.Connect(PRESSED, this, nameof(Redo));
			RetryButton.Connect(PRESSED, this, nameof(Retry));

			HideButtons();   
		}
		protected void HideButtons()
		{
			QuitButton.Visible = false;
			UndoButton.Visible = false;
			RedoButton.Visible = false;
			RetryButton.Visible = false;
			scoreLabel.Visible = false;
			scoreRefLabel.Visible = false;
			RadarLoader.Instance.Hide();
		}

		public void CheckIfTuto()
		{
			if ((int)Main.userProfil.levelsScore[0] == 0 && Main.userProfil.unlockedLevels == 0)
			{
				TutoSettings();
			}
			else
			{
				StandardSettings();
				if(TutoNode.Instance != null) TutoNode.Instance.QueueFree();
			}
		}

		private void TutoSettings()
		{
			CrateApparition = 1.25f;
			TargetApparition = 2.5f;
			HandlesApparition = 4f;
			PlayerApparition = 4.5f;
			SpeedApparition = 1;
			TargetParticulesApparition = 7f;
		}

		private void StandardSettings()
		{
			CrateApparition = 0.5f;
			TargetApparition = CrateApparition + 0.5f;
			HandlesApparition = TargetApparition + 0.5f;
			PlayerApparition = HandlesApparition + 0.5f;
			SpeedApparition = 1;
			TargetParticulesApparition = PlayerApparition + 2f;
		}

		public void DisplayNewScoreRef(int pScoreRef)
		{
			scoreRef = pScoreRef;
			scoreRefLabel.Text = "Par : " + scoreRef;
		}
		public void DisplayNewBestScoreRef(int pBestScoreRef)
		{
			bestScoreRef = pBestScoreRef;
		}

		public void UpdateScore(int pScore)
		{
			scoreLabel.Text = "Steps : " + pScore;
		}

		public void Increment()
		{
			UpdateScore(++Score);
		}

		public void Decrement()
		{
			UpdateScore(--Score);
		}

		public void ReturnToMenu()
		{
			TimeSpan ldelay = TimeSpan.FromSeconds( Main.Instance.FadeOutFadeIn(this, nameof(ReturnToMenuGroupMethod)) );
		}

		protected void ReturnToMenuGroupMethod()
        {
			MainMenu.Instance.Show();
			SoundManager.Instance.MusicPlay();
			QueueFree();
		}

		protected void Undo()
		{
			GridPresenter.player.Undo();

			GridModel.Print();
		}

		protected void Redo()
		{
			GridPresenter.player.Redo();

			GridModel.Print();
		}

		protected void Retry()
		{
			tutoVisible = true;
			SoundManager.Instance.SoundPlay(SoundManager.Instance.btnRetry);
			IsoLoader.Instance.GetParent().Free();
			AddChild(isoScenePacked.Instance());

			RadarLoader.Instance.GetParent().Free();
			AddChild(radarScenePacked.Instance());

			Score = 0;
			UpdateScore(Score);
			GridPresenter.LoadLevel();
		}

		public void Next()
		{
			tutoVisible = true;
			SoundManager.Instance.SoundPlay(SoundManager.Instance.btnLevel);
			IsoLoader.Instance.GetParent().Free();
			AddChild(isoScenePacked.Instance());

			RadarLoader.Instance.GetParent().Free();
			AddChild(radarScenePacked.Instance());

			Score = 0;
			UpdateScore(Score);
			int lLevel = Main.userProfil.currentLevel;
			SelectLevel.Instance.ChangeLevel(lLevel);
			HideButtons();
			GridPresenter.LoadLevel();
		}


		public int GetVictoryPoints()
		{
			if (Score == bestScoreRef) return BEST_VICTORY_POINT;
			else if (Score < scoreRef && Score > bestScoreRef) return MAX_VICTORY_POINT;
			else if (Score == scoreRef) return PAR_VICTORY_POINT;
			else return MIN_VICTORY_POINT;
		}

		public override void _UnhandledKeyInput(InputEventKey @event)
		{
			base._UnhandledKeyInput(@event);

			if (Main.Instance.keyInputsBlocked) return;

			if (@event is InputEventKey eventKey)
			{
				if (eventKey.Pressed && !eventKey.Echo)
				{
					if (eventKey.Scancode == (int)KeyList.Q || eventKey.Scancode == (int)KeyList.Left)
					{
						GridPresenter.MovePlayer(Vector2.Left);
					}
					else if (eventKey.Scancode == (int)KeyList.D || eventKey.Scancode == (int)KeyList.Right)
					{
						GridPresenter.MovePlayer(Vector2.Right);
					}
					if (eventKey.Scancode == (int)KeyList.Z && eventKey.Control)
					{
						if (eventKey.Shift) Redo();
						else Undo();
					}
					else if (eventKey.Scancode == (int)KeyList.Z || eventKey.Scancode == (int)KeyList.Up)
					{
						GridPresenter.MovePlayer(Vector2.Up);
					}
					else if (eventKey.Scancode == (int)KeyList.S || eventKey.Scancode == (int)KeyList.Down)
					{
						GridPresenter.MovePlayer(Vector2.Down);
					}

					if (eventKey.Scancode == (int)KeyList.R)
					{
						Retry();
					}
				}
			}
		}

		public void HideRadarAndButtons()
		{
			tween.InterpolateProperty(QuitButton, "modulate:a", QuitButton.Modulate.a, 0, .5f);
			tween.InterpolateProperty(UndoButton, "modulate:a", UndoButton.Modulate.a, 0, .5f);
			tween.InterpolateProperty(RedoButton, "modulate:a", RedoButton.Modulate.a, 0, .5f);
			tween.InterpolateProperty(RetryButton, "modulate:a", RetryButton.Modulate.a , 0, .5f);
			tween.InterpolateProperty(scoreLabel, "modulate:a", scoreLabel.Modulate.a, 0,.5f);
			tween.InterpolateProperty(scoreRefLabel, "modulate:a", scoreRefLabel.Modulate.a, 0, .5f);
			tween.InterpolateProperty(RadarLoader.Instance, "modulate:a", RadarLoader.Instance.Modulate.a, 0, .5f);

			tween.Start();
		}
		public void ShowRadarAndButtons()
		{
            if (tutoVisible)
            {
				Sprite lTutorial = (Sprite)GridPresenter.player.isoLink.GetNode("MovementTutorial");
				((Tween)lTutorial.GetNode("Tween")).InterpolateProperty(lTutorial, "modulate", lTutorial.Modulate, new Color(1, 1, 1, 0), 0.5f);
				((Tween)lTutorial.GetNode("Tween")).Start();
				tutoVisible = false;
            }

			if (QuitButton.Visible) return;


			QuitButton.Visible = true;
			UndoButton.Visible = true;
			RedoButton.Visible = true;
			RetryButton.Visible = true;
			scoreLabel.Visible = true;
			scoreRefLabel.Visible = true;
			RadarLoader.Instance.Show();

			tween.InterpolateProperty(QuitButton, "modulate:a", 0, 1, .5f, delay: 0.3f);
			tween.InterpolateProperty(UndoButton, "modulate:a", 0, 1, .5f, delay: 0.3f);
			tween.InterpolateProperty(RedoButton, "modulate:a", 0, 1, .5f, delay: 0.3f);
			tween.InterpolateProperty(RetryButton, "modulate:a", 0, 1, .5f, delay: 0.3f);
			tween.InterpolateProperty(scoreLabel, "modulate:a", 0, 1, .5f, delay: 0.5f);
			tween.InterpolateProperty(scoreRefLabel, "modulate:a", 0, 1, .5f, delay: 0.5f);
			tween.InterpolateProperty(RadarLoader.Instance, "modulate:a", 0, RadarLoader.Instance.Modulate.a, .5f);

			tween.InterpolateProperty(QuitButton, "rect_position:y", QuitButton.RectPosition.y + 30, QuitButton.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay: 0.3f);
			tween.InterpolateProperty(UndoButton, "rect_position:y", UndoButton.RectPosition.y + 30, UndoButton.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay: 0.3f);
			tween.InterpolateProperty(RedoButton, "rect_position:y", RedoButton.RectPosition.y + 30, RedoButton.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay: 0.3f);
			tween.InterpolateProperty(RetryButton, "rect_position:y", RetryButton.RectPosition.y + 30, RetryButton.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay: 0.3f);
			tween.InterpolateProperty(scoreLabel, "rect_position:y", scoreLabel.RectPosition.y + 30, scoreLabel.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay: 0.5f);
			tween.InterpolateProperty(scoreRefLabel, "rect_position:y", scoreRefLabel.RectPosition.y + 30, scoreRefLabel.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay: 0.5f);
			tween.InterpolateProperty(RadarLoader.Instance, "rect_position:y", RadarLoader.Instance.RectPosition.y + 30, RadarLoader.Instance.RectPosition.y, .5f, Tween.TransitionType.Quad, Tween.EaseType.Out);
			tween.Start();

			Color lColor = new Color(1f, 1f, 1f, 0f);
			QuitButton.Modulate = lColor;
			UndoButton.Modulate = lColor;
			RedoButton.Modulate = lColor;
			RetryButton.Modulate = lColor;
			scoreLabel.Modulate = lColor;
			scoreRefLabel.Modulate = lColor;
		}

		protected override void Dispose(bool pDisposing)
		{
			if (pDisposing && Instance == this)
				Instance = null;

			base.Dispose(pDisposing);
		}
	}
}
