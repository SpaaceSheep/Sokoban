using Com.IsartDigital.HandleTheCrate.Menu;
using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.View.Iso
{

	public class CrateIsoView : ObjectIsoView
	{
        public override float GetInitDelay()
        {
            return base.GetInitDelay() + LevelContainer.Instance.CrateApparition;
        }

        public Dictionary<Vector2, Node2D> handles = new Dictionary<Vector2, Node2D>();

		public void Detach(Vector2 pDirection)
        {
			handles[pDirection.Round()].QueueFree();
        }

    }
}