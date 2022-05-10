using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Godot;

namespace Com.IsartDigital.HandleTheCrate.Presenters.Object
{

    public class WallPresenter : MobilePresenter
	{
		public WallParentPresenter parent;

		public WallPresenter(WallParentPresenter pParent) : base((WallIsoView)IsoLoader.Instance.WallFactory.Instance(), (WallRadarView)RadarLoader.Instance.WallFactory.Instance())
		{
			parent = pParent;
			parent.AddWall(this);
		}

        public override void Highlight(float pValue = 0.5f)
        {
			parent.Highlight(pValue);
        }


        public override bool Pull(Vector2 pDirection)
		{
			if (!handles.Contains(pDirection)) return false;

			return parent.MoveTowards(pDirection);
		}

        public override void ForceMove(Vector2 pDirection)
        {
			parent.MoveTowards(pDirection);
		}

        public override string ToString()
        {
			return parent.index.ToString();
        }
    }

}