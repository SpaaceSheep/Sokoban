using Com.IsartDigital.HandleTheCrate.Sound;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Iso
{

	public class AttachedHandleIsoView : ObjectIsoView
	{
		protected Vector2 initPos;
		[Export] PackedScene particleFactory;
		public CrateIsoView parent;

		public void Init(Vector2 pInitPos)
		{
			tween = (Tween)GetNode("Tween");
			initPos = Position;
			Main.Instance.inputsBlocked = true;

			tween.InterpolateProperty(this, "position", pInitPos, initPos, 0.4f, Tween.TransitionType.Cubic, Tween.EaseType.In);
			tween.InterpolateCallback(this, tween.GetRuntime() - 0.1f, nameof(CreateParticles));
			tween.InterpolateCallback(this, tween.GetRuntime() + 0.1f, nameof(EnableInputs));
			tween.Start();
		}

		protected void CreateParticles()
        {
			CPUParticles2D lParticles = (CPUParticles2D)particleFactory.Instance();
			AddChild(lParticles);
			lParticles.Emitting = true;
			SoundManager.Instance.SoundPlay(SoundManager.Instance.soundHandle);
			parent.Blocked(0.5f);
		}

		protected void EnableInputs()
        {
			Main.Instance.inputsBlocked = false;
        }
	}

}