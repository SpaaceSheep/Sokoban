using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.Utils.Game;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Iso {

	public class ObjectIsoView : Sprite
	{
		public virtual float GetInitDelay()
        {
			return ZIndex * 0.05f + 0.35f;

		}

		public Tween tween;
		Vector2 initScale;

		public virtual Vector2 GetHandleOffset()
        {
			return Vector2.Zero;
        }

		public virtual float GetHandleDistance()
        {
			return 0;
        }

        public override void _Ready()
        {
			initScale = Scale;
			tween = (Tween)GetNode("Tween");
        }

		public void MoveTo(Vector2 pPosition)
		{
			//tween.StopAll();
			tween.InterpolateProperty(this, "position", Position, pPosition, 0.5f, Tween.TransitionType.Elastic, Tween.EaseType.Out);
			tween.Start();
		}

		public void Blocked(float lDuration = 0.2f)
        {
			tween.InterpolateProperty(this, "scale", initScale * 1.5f, initScale, lDuration, Tween.TransitionType.Elastic, Tween.EaseType.Out);
			tween.Start();
		}

		public virtual void Appear()
        {
			//tween.StopAll();
			tween.InterpolateProperty(this, "scale", Vector2.Zero, Scale, 0.5f, Tween.TransitionType.Back, Tween.EaseType.Out, delay : GetInitDelay());
			tween.InterpolateProperty(this, "position:y", Position.y - 70, Position.y, 0.3f, Tween.TransitionType.Quad, Tween.EaseType.Out, delay : GetInitDelay());
			if (Material != null)
			{
				(Material as ShaderMaterial).SetShaderParam("progress", 1);
				tween.InterpolateProperty(Material, "shader_param/progress", 1, 0, LevelContainer.Instance.SpeedApparition, delay: GetInitDelay());
			}
			tween.Start();
			Scale = Vector2.Zero;
            Position += Vector2.Down * 70;

        }
	}

}