using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.Sound;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Godot;

namespace Com.IsartDigital.HandleTheCrate.Presenters.Object
{

    public class HandlePresenter : Presenter
	{

        public HandlePresenter() : base((HandleIsoView)IsoLoader.Instance.HandleFactory.Instance(), (HandleRadarView)RadarLoader.Instance.HandleFactory.Instance()) { }

		public void Attach()
        {
			Vector2 lDirection = Vector2.Up;
            Presenter lObj;

            //verifier des 4 cotes si y'a une caisse
            for (int i = 0; i < 4; i++)
            {
                lObj = GridModel.GetAt(position + lDirection);

                //si y'a une caisse et qu'elle a pas de poignee sur cette direction
                if (lObj is CratePresenter && ((CratePresenter)lObj).AddHandle(-lDirection))
                {
                    GridPresenter.player.AttachHandle((CratePresenter)lObj, -lDirection);
                    GridPresenter.Destroy(this);
                    return;
                }

                lDirection = lDirection.Rotated(Mathf.Pi / 2).Round();
            }
        }

        public void Attach(CratePresenter pCrate)
        {
            Vector2 lDirection = position - pCrate.position;
            
            if (pCrate.AddHandle(lDirection))
            {
                //SoundManager.Instance.SoundPlay(SoundManager.Instance.soundHandle);
                GridPresenter.player.AttachHandle(pCrate, lDirection);
                GridPresenter.Destroy(this);
            }

        }
    }

}