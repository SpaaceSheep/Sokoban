using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Iso
{

	public class WallIsoView : ObjectIsoView
	{
        public override Vector2 GetHandleOffset()
        {
            return new Vector2(0, 50);
        }

        public override float GetHandleDistance()
        {
            return 15f;
        }
    }

}