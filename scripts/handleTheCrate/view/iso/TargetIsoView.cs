using Com.IsartDigital.HandleTheCrate.Menu;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Iso
{
	public class TargetIsoView : ObjectIsoView
	{
        [Export] NodePath particulesPath;
        CPUParticles2D particules;

        public override void _Ready()
        {
            particules = (CPUParticles2D)GetNode(particulesPath);
            particules.Emitting = false;    
            base._Ready();
        }

        public override float GetInitDelay()
        {
            tween.InterpolateProperty(particules, "emitting", false, true, LevelContainer.Instance.TargetParticulesApparition);
            return base.GetInitDelay() + LevelContainer.Instance.TargetApparition;
        }

    }
}