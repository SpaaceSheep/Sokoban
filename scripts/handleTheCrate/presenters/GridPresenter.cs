using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.Presenters.Object;
using Com.IsartDigital.HandleTheCrate.Utils;
using Com.IsartDigital.HandleTheCrate.View;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.Utils;
using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Presenters
{
    public static class GridPresenter
	{
		//le player pour pouvoir le manipuler
		public static PlayerPresenter player;
		private static List<MobilePresenter> previouslyHighlighted = new List<MobilePresenter>();
		private static List<Presenter> previouslyTransparent = new List<Presenter>();

		//nombre de cibles pour la condition de victoire
		private static int targetCount;
		
		public static void LoadLevel()
		{
			previouslyHighlighted = new List<MobilePresenter>();
			previouslyTransparent = new List<Presenter>();
			//creer une nouvelle grille
			GridModel.ResetGrid();

			//utiliser l'information du level pour creer les presenter et remplir la grille
			LevelInfo lLevel = SelectLevel.Instance.CurrentLevel;
			Presenter lObject = null;
			char lObjectChar;
			int lObjectCode;
			Dictionary<int, WallParentPresenter> lWallParents = new Dictionary<int, WallParentPresenter>(); 
			targetCount = 0;

			for (int x = 0; x < GridModel.LENGTH_X; x++)
			{
				for (int y = 0; y < GridModel.LENGTH_Y; y++)
				{
					#region MAP OBJECTS (player, crates & walls)
					//MAP
					//caractere a cet emplacement de la map
					lObjectChar = lLevel.GetMap(x, y);

					//creer une boite si c'est b
					if (lObjectChar == JsonParseHelper.CRATE_CODE) lObject = new CratePresenter();
					//creer joueur si c'est P, et le stocker
					else if (lObjectChar == JsonParseHelper.PLAYER_CODE) lObject = player = new PlayerPresenter();
					//si c'est pas vide, c'est un mur (les murs sont symbolises par des chiffres de 1 a 9)
					else if (lObjectChar != ' ')
					{ 
						//recuperer ce chiffre
						int lWallIndex = int.Parse(lObjectChar.ToString());

						//creer le parent des murs numero X s'il existe pas, le stocker dans un dico
						if (!lWallParents.ContainsKey(lWallIndex)) lWallParents.Add(lWallIndex, new WallParentPresenter());

						//donner l'index au parent, creer le mur et le lier a son parent
						lWallParents[lWallIndex].index = lWallIndex;
						lObject = new WallPresenter(lWallParents[lWallIndex]);
					}

					//mettre l'objet sur la grille, creer le lien graphique
					GridModel.SetGridAt(x, y, lObject);
					if (lObject != null)
					{
						lObject.SetView();
						if (Main.userProfil.unlockedLevels == 0 && lObject is CratePresenter)
							CameraEffects.Instance.ZoomAt(lObject.isoLink, 1.5f);
					}
					#endregion

					#region OVERLAY OBJECTS (targets & handles)
					//OVERLAY
					//caractere a cet emplacement dans l'overlay (poignees et cibles)
					lObjectChar = lLevel.GetOverlay(x, y);
					//recuperer le code ascii et enlever le decalage de 40 (pour eviter les caracteres speciaux)
					lObjectCode = (int)lObjectChar - JsonParseHelper.OFFSET;

					//creer cible
					if (lObjectCode >= JsonParseHelper.TARGET_CODE)
					{
						lObject = new TargetPresenter();
						lObjectCode -= JsonParseHelper.TARGET_CODE;
						//cibles sur la couche 1 de la grille
						GridModel.SetGridAt(x, y, lObject, 1);
						lObject.SetView();
						//changer le nombre total de cibles
						++targetCount;

						if (Main.userProfil.unlockedLevels == 0)
						{
							CameraEffects.Instance.ZoomAt(lObject.isoLink, 3f);
							CameraEffects.Instance.Reset(4.5f);
						}
					}
					//creer poignee
					if (lObjectCode >= JsonParseHelper.HANDLE_CODE)
					{
						lObject = new HandlePresenter();
						lObjectCode -= JsonParseHelper.HANDLE_CODE;
						//poignees sur la couche 2 de la grille
						GridModel.SetGridAt(x, y, lObject, 2);
						lObject.SetView();
					}
                    #endregion

                    #region FIXED HANDLES
                    //creer poignee fixee (le code restant correspond a un code de poignee attachee)
					//il y a forcement une poignee si lobjectcode > 0
                    if (lObjectCode > 0)
					{
						((MobilePresenter)lObject).SetupHandles(lObjectCode);
					}
                    #endregion

                    lObject = null;
				}
			}
			UpdateOverlay();
			GridModel.Print();
		}

        #region PLAYER MOVE METHODS
        public static void MovePlayer(Vector2 pDirection)
		{
			//bouger, check collisions etc
			player.Move(pDirection);
			//attacher les poignees, verifier la victoire
			UpdateOverlay();
			//GridModel.Print();

			LevelContainer.Instance.ShowRadarAndButtons();
		}
		public static void UndoMovePlayer(UndoRedoAction pAction)
		{
			player.UndoMove(pAction);
			//attacher les poignees, verifier la victoire
			LevelContainer.Instance.Decrement();
			UpdateOverlay();
			//GridModel.Print();
		}
		public static void RedoMovePlayer(UndoRedoAction pAction)
		{
			player.RedoMove(pAction);
			//attacher les poignees, verifier la victoire
			LevelContainer.Instance.Increment();
			UpdateOverlay();
			//GridModel.Print();
		}
        #endregion

        #region GENERIC OBJECTS MOVE METHODS
        //fonction pour bouger un objet [1 surcharge]
        //bouger l'objet de la case X vers la case Y
        public static void Move(Vector2 pPosition, Vector2 pDirection)
		{
			Vector2 lNewPos = pPosition + pDirection;

			if (!GridModel.IsInBounds(lNewPos)) return;

			GridModel.SetGridAt(lNewPos, GridModel.GetAt(pPosition));
			GridModel.SetGridAt(pPosition, null);
		}
		//bouger l'objet A sur la case X
		public static void Move(Presenter pObject, Vector2 pDirection)
		{
			Vector2 lNewPos = pObject.position + pDirection;

			if (!GridModel.IsInBounds(lNewPos)) return;

			GridModel.SetGridAt(pObject.position, null);
			GridModel.SetGridAt(lNewPos, pObject);
		}
        #endregion

        //detruire un objet de la grille (les poignees notemment)
        public static void Destroy(Presenter pObject, int pLayer = 2)
        {
			pObject.isoLink.QueueFree();
			pObject.radarLink.QueueFree();
			GridModel.SetGridAt(pObject.position, null, pLayer);
        }

		public static void UpdateOverlay()
        {
			Presenter lOverlayObj;
			Presenter lObj;
			int lTargetsFilled = 0;
			List<Presenter> transparents = new List<Presenter>();

			for (int x = 0; x < GridModel.LENGTH_X; x++)
			{
				for (int y = 0; y < GridModel.LENGTH_Y; y++)
				{
					//recuperer objet couche 2 (poignee)
					lOverlayObj = GridModel.GetAt(x, y, GridModel.HANDLE_LAYER);
					//case pas vide ? essayer d'attacher la poignee
					if (lOverlayObj != null)
					{
						lObj = GridModel.GetAt(x, y);
						if (lObj != null)
                        {
							transparents.Add(lObj);
                        }
						((HandlePresenter)lOverlayObj).Attach();
					}
					//recuperer objet couche 1 (cible)
					lOverlayObj = GridModel.GetAt(x, y, GridModel.TARGET_LAYER);
					//case pas vide ? rajouter 1 au compteur si la cible est remplie
					if (lOverlayObj != null)
					{
						lObj = GridModel.GetAt(x, y);
						if (lObj != null)
						{
							transparents.Add(lObj);
						}
						lTargetsFilled += ((TargetPresenter)lOverlayObj).Filled();
					}
				}
			}

            foreach (MobilePresenter lPresenter in previouslyHighlighted)
            {
				if((lPresenter.position - player.position).Length() != 1) lPresenter.Highlight(0f);
            }
			foreach (Presenter lPresenter in previouslyTransparent)
			{
				if(!transparents.Contains(lPresenter)) lPresenter.SetTransparency(1);
			}

			previouslyHighlighted = new List<MobilePresenter>();
			previouslyTransparent = new List<Presenter>();

			Vector2 lDirection = Vector2.Up;
            for (int i = 0; i < 4; i++)
            {
				lOverlayObj = GridModel.GetAt(player.position + lDirection);
				if (lOverlayObj is MobilePresenter)
				{
					if (((MobilePresenter)lOverlayObj).handles.Contains(-lDirection))
					{
						((MobilePresenter)lOverlayObj).Highlight();
						previouslyHighlighted.Add((MobilePresenter)lOverlayObj);
					}
				}

				lDirection = lDirection.Rotated(Mathf.Pi / 2).Round();
			}
			foreach(Presenter current in transparents)
            {
				current.SetTransparency(0.7f);
				previouslyTransparent.Add(current);
            }


			//si toutes les cibles sont remplies
			if(lTargetsFilled == targetCount)
            {
				LevelContainer.Instance.HideRadarAndButtons();
				int lScore = LevelContainer.Instance.GetVictoryPoints();
				WinScreen.Instance.Appear(lScore);
				Main.UpdateProfil(lScore);
				Main.Instance.keyInputsBlocked = true;
			}
		}

		private static void Attach(CratePresenter pCrate)
        {
			Vector2 lDirection = Vector2.Up;
			HandlePresenter lObject;

			for (int i = 0; i < 4; i++)
			{
				lObject = (HandlePresenter)GridModel.GetAt(pCrate.position + lDirection, GridModel.HANDLE_LAYER);
				
				if(lObject != null)
                {
					lObject.Attach(pCrate);
                }

				lDirection = lDirection.Rotated(Mathf.Pi / 2).Round();
			}
		}
	}
}