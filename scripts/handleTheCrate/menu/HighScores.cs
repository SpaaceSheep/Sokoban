using Com.IsartDigital.HandleTheCrate.Sound;
using Com.IsartDigital.HandleTheCrate.Utils;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using GArray = Godot.Collections.Array;
using GDictionary = Godot.Collections.Dictionary;

namespace Com.IsartDigital.HandleTheCrate.Menu
{
	public class HighScores : NinePatchRect
	{
		public static HighScores Instance { get; private set; }

		private HighScores() : base() { }

		[Export] protected NodePath highScoresContainerPath;
		[Export] protected NodePath userScorePath;
		[Export] protected NodePath menuButtonPath;
		protected Button menuButton;

		protected const string START_PLAYER_BB = "[center][wave amp = 50][rainbow freq = 0.2 sat = 10 val = 20]";
		protected const string END_RAINBOW = "[/rainbow] :";
		protected const string END_PLAYER_BB = "[/wave][/center]";

		protected Array<RichTextLabel> scoreNodes = new Array<RichTextLabel>();

		public override void _Ready()
		{
			if (Instance != null)
			{
				Free();
				GD.Print($"{nameof(HighScores)} Instance already exist, destroying the last added.");
				return;
			}

			Instance = this;

			menuButton = (Button)GetNode(menuButtonPath);
			menuButton.Connect(MainMenu.PRESSED, this, nameof(ReturnToMenu));

			GArray children = GetNode(highScoresContainerPath).GetChildren();

			int lIteration = children.Count;
			for (int i = 0; i < lIteration; i++)
			{
				scoreNodes.Add((RichTextLabel)children[i]);
			}

			LoadAndDisplayHighScores();
		}

		private void ReturnToMenu()
		{
			SoundManager.Instance.ValidateBtn();
			Main.Instance.FadeOutFadeIn(this, nameof(ReturnToMenuGroupMethod));
		}

		private void ReturnToMenuGroupMethod()
        {
			QueueFree();
			MainMenu.Instance.Show();
		}

		private void LoadAndDisplayHighScores()
		{
			//I - open the profils
			GDictionary lAllProfils = JsonProfilSystem.usersProfils;

			GArray lAllNames = (GArray)lAllProfils.Keys;
			GArray lAllValues = (GArray)lAllProfils.Values;

			//II - get all names and scores
			string lName;
			int lScore = 0;
			List<GDictionary> lPlayerAllInfo = new List<GDictionary>();
			List<PlayerData> lNamesScores = new List<PlayerData>();

			int lIterationNb = lAllProfils.Count;
			for (int i = 0; i < lIterationNb; i++)
			{
				lName = lAllNames[i].ToString();

				lPlayerAllInfo.Add((GDictionary)lAllValues[i]);
				lScore = int.Parse(lPlayerAllInfo[i]["score"].ToString());

				lNamesScores.Add(new PlayerData(lName, lScore));
			}

			//III - sort from the highest to the lower score
			lNamesScores.Sort(Test);

			//IV - write result in richTextLabels
			lIterationNb = scoreNodes.Count;
			int lProfilsNb = lNamesScores.Count;
			string richTextContent;
			bool alreadyInLeaderboard = false;

			for (int j = 0; j < lIterationNb; j++)
			{
				if(j < lProfilsNb)
				{
					if (lNamesScores[j].name == Main.userProfil.name)
					{
						richTextContent = CurrentUserNameScore();

						alreadyInLeaderboard = true;
					}
					else
					{
						richTextContent = $"[center]{lNamesScores[j].name} : {lNamesScores[j].score}[/center]";
					}
				}
				else
				{
					richTextContent = "";
				}

				scoreNodes[j].BbcodeText = richTextContent;
			}

			//for the last richTextLabel
			RichTextLabel lTextScore = (RichTextLabel)GetNode(userScorePath);

			lTextScore.BbcodeText =  alreadyInLeaderboard ?  " " : CurrentUserNameScore();
		}

		protected int Test(PlayerData pS1, PlayerData pS2)
		{
			return pS2.score.CompareTo(pS1.score);
		}

		protected string CurrentUserNameScore()
		{
			return $"{START_PLAYER_BB} {Main.userProfil.name} {END_RAINBOW} {Main.userProfil.score} {END_PLAYER_BB}";
		}

		protected override void Dispose(bool pDisposing)
		{
			if (pDisposing && Instance == this)
				Instance = null;

			base.Dispose(pDisposing);
		}
	}

}
