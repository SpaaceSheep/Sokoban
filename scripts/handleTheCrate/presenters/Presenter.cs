using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.utils.game;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Com.IsartDigital.Utils.Game;
using Godot;

namespace Com.IsartDigital.HandleTheCrate.Presenters
{
    public abstract class Presenter
    {        
        public Vector2 position;

        public ObjectIsoView isoLink;
        public ObjectRadarView radarLink;

        //on utilise le constructeur pour recuperer les view, override pour passer les instances
        public Presenter(ObjectIsoView pIsoLink, ObjectRadarView pRadarLink)
        {
            isoLink = pIsoLink;
            radarLink = pRadarLink;
        }

        public override string ToString()
        {
            return " ";
        }

        public virtual void SetTransparency(float pAlpha = 1)
        {
            Color lColor = isoLink.SelfModulate;
            
            if (lColor.a == pAlpha) return;

            lColor.a = pAlpha;
            isoLink.tween.InterpolateProperty(isoLink, "self_modulate", isoLink.SelfModulate, lColor, 0.3f);
            isoLink.tween.Start();
            isoLink.SelfModulate = lColor;

            radarLink.tween.InterpolateProperty(radarLink, "self_modulate", radarLink.SelfModulate, lColor, 0.3f);
            radarLink.tween.Start();
            radarLink.SelfModulate = lColor;
        }

        //creer les objets iso et radar, les addchild et les positionner
        public virtual void SetView()
        {
            IsoLoader.Instance.GetObjectParent().AddChild(isoLink);
            isoLink.Position = IsoManager.ModelToIsoView(position);
            isoLink.ZIndex = IsoManager.GetZIndex(position);
            isoLink.Appear();

            RadarLoader.Instance.GetObjectParent().AddChild(radarLink);
            radarLink.Position = RadarManager.ModelToRadarView(position);
            radarLink.ZIndex = RadarManager.GetZIndex(position);
        }
        public virtual void SetViewLater()
        {
            IsoLoader.Instance.GetObjectParent().AddChild(isoLink);
            isoLink.Position = IsoManager.ModelToIsoView(position);
            isoLink.ZIndex = IsoManager.GetZIndex(position);

            RadarLoader.Instance.GetObjectParent().AddChild(radarLink);
            radarLink.Position = RadarManager.ModelToRadarView(position);
            radarLink.ZIndex = RadarManager.GetZIndex(position);
        }

        //bouger la view de la case X a Y
        public void MoveView(Vector2 pFrom, Vector2 pTo)
        {
            if(pFrom == pTo)
            {
                isoLink.Blocked();
                radarLink.Blocked();
            }
            else
            {
                isoLink.MoveTo(IsoManager.ModelToIsoView(pTo));
                isoLink.ZIndex = IsoManager.GetZIndex(pTo);
                radarLink.MoveTo(RadarManager.ModelToRadarView(pTo));
                radarLink.ZIndex = RadarManager.GetZIndex(pTo);
            }
        }
    }
}

