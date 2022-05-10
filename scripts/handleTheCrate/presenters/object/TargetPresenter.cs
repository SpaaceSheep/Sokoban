using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.utils.game;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Com.IsartDigital.Utils.Game;

namespace Com.IsartDigital.HandleTheCrate.Presenters.Object
{

    public class TargetPresenter : Presenter
	{
		public TargetPresenter() : base((TargetIsoView)IsoLoader.Instance.TargetFactory.Instance(), (TargetRadarView)RadarLoader.Instance.TargetFactory.Instance()) { }

		public int Filled()
        {
			return GridModel.GetAt(position) is CratePresenter ? 1 : 0;
        }

        public override void SetView()
        {
            base.SetView();
            isoLink.ZIndex = IsoManager.GetZIndex(position) - 1;
            radarLink.ZIndex = RadarManager.GetZIndex(position) - 1;
        }
    }
}