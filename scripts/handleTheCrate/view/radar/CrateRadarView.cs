using Godot;
using System;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.View.Radar
{

	public class CrateRadarView : ObjectRadarView
	{
		public Dictionary<Vector2, Node2D> handles = new Dictionary<Vector2, Node2D>();

		public void Detach(Vector2 pDirection)
		{
			handles[pDirection.Round()].QueueFree();
		}
	}

}