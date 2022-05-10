using Com.IsartDigital.HandleTheCrate.Menu;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Iso
{

	public class HandleIsoView : ObjectIsoView
	{
        public override float GetInitDelay()
        {
            return base.GetInitDelay() + LevelContainer.Instance.HandlesApparition;
        }
    }
}