using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Radar {

	public class ObjectRadarView : Sprite
	{
		private const string TWEEN_PATH = "Tween";
		
		public Tween tween;

		Vector2 initScale;


		public override void _Ready()
		{
			initScale = Scale;
			tween = (Tween)GetNode(TWEEN_PATH);
		}

		public void MoveTo(Vector2 pPosition)
		{
			//tween.StopAll();
			tween.InterpolateProperty(this, "position", Position, pPosition, 0.2f, Tween.TransitionType.Quad, Tween.EaseType.Out);
			tween.Start();
		}

		public void Blocked()
		{
			tween.InterpolateProperty(this, "scale", initScale * 1.5f, initScale, 0.2f, Tween.TransitionType.Elastic, Tween.EaseType.Out);
			tween.Start();
		}
	}

}