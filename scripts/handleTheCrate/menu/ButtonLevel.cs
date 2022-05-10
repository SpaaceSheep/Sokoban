using Com.IsartDigital.HandleTheCrate.Utils;
using Godot;

namespace Com.IsartDigital.HandleTheCrate.Menu
{

    public class ButtonLevel : Button
	{
		public static int GloballevelIndex { get; protected set; }
		protected int levelIndex;
		protected string LevelPrefix = "LEVEL_X";

		public override void _Ready()
		{
			levelIndex = GloballevelIndex;
			GloballevelIndex++;
			TranslateText();

			Connect("pressed", this, nameof(OnClick));
			Owner.Connect("ChangeLanguage", this, nameof(TranslateText));
			Disabled = true;
		}

		public void TranslateText()
        {
			Text = $"{Tr(LevelPrefix)} {levelIndex}";
		}

		public void OnClick()
		{
			SelectLevel.Instance.LoadLevel(levelIndex);
		}

		public static void ResetGobalLevelIndex()
        {
			GloballevelIndex = 0; //need to start again from 0 when user profil is changed
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
			if (levelIndex <= Main.userProfil.unlockedLevels)
            {
				Disabled = false;
            }
        }
    }
}
