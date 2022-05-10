using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.utils.game;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Com.IsartDigital.Utils.Game;
using Godot;

namespace Com.IsartDigital.HandleTheCrate.Presenters.Object
{

	public class CratePresenter : MobilePresenter
	{
		public CratePresenter() : base((CrateIsoView)IsoLoader.Instance.BoxFactory.Instance(), (CrateRadarView)RadarLoader.Instance.BoxFactory.Instance()) { }

		public override void Push(Vector2 pDirection)
		{
			if(GridModel.IsEmpty(position + pDirection))
			{
				GridPresenter.Move(this, pDirection);
			}
		}

		public override bool Pull(Vector2 pDirection)
		{
			if (!handles.Contains(pDirection)) return false;

			MoveView(position, position + pDirection);
			GridPresenter.Move(this, pDirection);
			return true;
		}

		public void DetachHandle(Vector2 pDirection)
        {
			if (!handles.Contains(pDirection)) return;

			handles.Remove(pDirection);
			((CrateIsoView)isoLink).Detach(pDirection);
			((CrateRadarView)radarLink).Detach(pDirection);
			
			HandlePresenter lObject = new HandlePresenter();
			GridModel.SetGridAt(position + pDirection, lObject, GridModel.HANDLE_LAYER);
			lObject.SetViewLater();
        }

        protected override void CreateHandleView(float pRad)
        {
			Vector2 lDirection = Vector2.Up.Rotated(pRad).Round();

            if (!((CrateIsoView)isoLink).handles.ContainsKey(lDirection))
            {
				((CrateIsoView)isoLink).handles.Add(lDirection, null);
				((CrateRadarView)radarLink).handles.Add(lDirection, null);
			}

			Node2D lConfiguration = (Node2D)IsoLoader.Instance.HandleConfigFacotry[GetHandleIndex(pRad)].Instance();
			isoLink.AddChild(lConfiguration);
			lConfiguration.Position = isoLink.GetHandleOffset() + lDirection * isoLink.GetHandleDistance();
			((CrateIsoView)isoLink).handles[lDirection] = lConfiguration;

            #region a modifier quand les assets seront la
            lConfiguration = (Node2D)RadarLoader.Instance.HandleConfigFacotry.Instance();
			((CrateRadarView)radarLink).handles[lDirection] = lConfiguration;
			radarLink.AddChild(lConfiguration);
			lConfiguration.Scale /= radarLink.Scale * 2f;
			lConfiguration.ZIndex = RadarManager.GetZIndex(position);
			lConfiguration.Rotation = pRad;
            #endregion
        }

		public void AttachAnim(Vector2 pDirection)
        {
			AttachedHandleIsoView lView = (AttachedHandleIsoView)((CrateIsoView)isoLink).handles[pDirection].GetNode("Handle");
			lView.parent = (CrateIsoView)isoLink;
			lView.Init(IsoManager.ModelToIsoView(pDirection));
		}

        public override string ToString()
		{
			return "B";
		}
	}
}