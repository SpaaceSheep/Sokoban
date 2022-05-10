using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.Presenters.Object;
using Com.IsartDigital.HandleTheCrate.utils.game;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Com.IsartDigital.Utils;
using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Presenters
{

    public abstract class MobilePresenter : Presenter
	{
		public List<Vector2> handles = new List<Vector2>();
		public int HandleConfig { protected set; get; }

		public MobilePresenter(ObjectIsoView pIsoView, ObjectRadarView pRadarView) : base(pIsoView, pRadarView) { }

		public virtual bool Pull(Vector2 pDirection)
		{
			return false;
		}

		public virtual void Push(Vector2 pDirection)
		{

		}

		public virtual void Highlight(float pValue = 0.2f)
        {
			ShaderMaterial lMat = (ShaderMaterial)isoLink.Material;

			if ((float)lMat.GetShaderParam("offset") == pValue) return;

			isoLink.tween.InterpolateProperty(lMat, "shader_param/offset", lMat.GetShaderParam("offset"), pValue, 0.3f);
			isoLink.tween.Start();
			lMat.SetShaderParam("offset", pValue);

			lMat = (ShaderMaterial)radarLink.Material;
			radarLink.tween.InterpolateProperty(lMat, "shader_param/offset", lMat.GetShaderParam("offset"), pValue, 0.3f);
			radarLink.tween.Start();
			lMat.SetShaderParam("offset", pValue);
		}
		public virtual void ForceMove(Vector2 pDirection)
		{
			Push(pDirection);
		}

		//ajout de handle (retourne un bool pour savoir si l'operation a reussie)
		public bool AddHandle(Vector2 pDirection)
		{
			pDirection = pDirection.Round();

			if (!handles.Contains(pDirection))
			{
				handles.Add(pDirection);
				HandleConfig = GetConfig();
				CreateHandleView(Mathf.Atan2(pDirection.y, pDirection.x) + JsonParseHelper.HALF_PI);
				if(this is CratePresenter) ((CratePresenter)this).AttachAnim(pDirection);

				return true;
			}
			return false;
		}

		public void SetupHandles(int pConfig)
		{
			handles = JsonParseHelper.ConfigToVector2(pConfig);

			//update la view
			foreach (float lRad in JsonParseHelper.ConfigToRads(pConfig))
			{
				CreateHandleView(lRad);
			}
		}

		//utiliser vector2 au lieu de rad quand les assets seront implementes!
		protected virtual void CreateHandleView(float pRad)
        {
			Vector2 lDirection = Vector2.Up.Rotated(pRad).Round();
			
			Node2D lConfiguration = (Node2D)IsoLoader.Instance.HandleConfigFacotry[GetHandleIndex(pRad)].Instance();
			isoLink.AddChild(lConfiguration);
			lConfiguration.Position = isoLink.GetHandleOffset() + lDirection * isoLink.GetHandleDistance();

			#region a modifier quand les assets seront la
			lConfiguration = (Node2D)RadarLoader.Instance.HandleConfigFacotry.Instance();
			radarLink.AddChild(lConfiguration);
			lConfiguration.Scale /= radarLink.Scale * 2f;
			lConfiguration.ZIndex = RadarManager.GetZIndex(position);
			lConfiguration.Rotation = pRad;
            #endregion
        }

        protected int GetHandleIndex(float pRad)
        {
			Vector2 lDir = Vector2.Up;
			Vector2 lConfig = Vector2.Up.Rotated(pRad).Round();
            for (int i = 0; i < 4; i++)
            {
				if(lDir == lConfig)
                {
					return i;
                }
				lDir = lDir.Rotated(Mathf.Pi / 2).Round();
            }
			return 0;
        }

		protected int GetConfig()
        {
			//config decrite par une suite de booleens en binaire (0010) donne la config avec rien en haut, poignee a droite, rien en bas, rien a gauche
			Vector2 lDirection = Vector2.Up;
			int lResult = 0;

			for (int i = 0; i < 4; i++)
			{
                if (handles.Contains(lDirection))
                {
					lResult += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
				lDirection = lDirection.Rotated(Mathf.Pi / 2).Round();
			}
			return lResult;
		}
	}

}