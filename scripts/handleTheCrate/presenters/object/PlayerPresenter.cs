using Com.IsartDigital.HandleTheCrate.Level.Loader;
using Com.IsartDigital.HandleTheCrate.Menu;
using Com.IsartDigital.HandleTheCrate.Menu.Loader;
using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.Sound;
using Com.IsartDigital.HandleTheCrate.View.Iso;
using Com.IsartDigital.HandleTheCrate.View.Radar;
using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Presenters.Object
{
	//a changer
    public class UndoRedoAction
    {
		public Vector2 playerDirection;
		public MobilePresenter pushedObject;
		public MobilePresenter pulledObject;

		public List<Vector2> pushedHandle;
		public List<Vector2> pulledHandle;

		public UndoRedoAction(Vector2 pPlayerDir, MobilePresenter pPushed, MobilePresenter pPulled)
        {
			playerDirection = pPlayerDir;
			pushedObject = pPushed;
			pulledObject = pPulled;

			pushedHandle = new List<Vector2>();
			pulledHandle = new List<Vector2>();
		}
	}

	public class PlayerPresenter : Presenter
	{
		protected Stack<UndoRedoAction> undo = new Stack<UndoRedoAction>();
		protected Stack<UndoRedoAction> redo = new Stack<UndoRedoAction>();


		public PlayerPresenter() : base(
			(PlayerIsoView)IsoLoader.Instance.PlayerFactory.Instance(), 
			(PlayerRadarView)RadarLoader.Instance.PlayerFactory.Instance()
			) 
		{ }

		public void Move(Vector2 pDirection)
        {
			Vector2 lInitialPos = position;

			UndoRedoAction lAction = new UndoRedoAction(pDirection, null, null);
			Vector2 lNextPos = position + pDirection;

			//case suivante pas vide ? essayer de la pousser (ca peut echouer!)
			if (!GridModel.IsEmpty(lNextPos))
			{
				Presenter lPresenter = GridModel.GetAt(lNextPos);

				if (lPresenter is MobilePresenter)
				{
					((MobilePresenter)lPresenter).Push(pDirection);
					lPresenter.MoveView(lNextPos, lPresenter.position);
					lAction.pushedObject = (MobilePresenter)lPresenter;
					SoundManager.Instance.SoundMovePlayer(SoundManager.Instance.soundCreate);
				}
				else { SoundManager.Instance.SoundMovePlayer(SoundManager.Instance.soundMovePlayer); }
			}

			//case suivante vide ? bouger la bas
			if (GridModel.IsEmpty(lNextPos))
			{
				Vector2 lPos = position;
				GridPresenter.Move(position, pDirection);
				MoveView(lPos, position);

				if(GridModel.IsInBounds(lNextPos)) LevelContainer.Instance.Increment();

				//2 cases derriere moi pas vide (le joueur vient de bouger) ? essayer de la tirer
				lNextPos = position + pDirection * -2;

				if (!GridModel.IsEmpty(lNextPos))
				{
					Presenter lPresenter = GridModel.GetAt(lNextPos);

					if (lPresenter is MobilePresenter && ((MobilePresenter)lPresenter).Pull(pDirection))
					{
						lAction.pulledObject = (MobilePresenter)lPresenter;
						SoundManager.Instance.SoundMovePlayer(SoundManager.Instance.soundCreate);
					}
				}
				else { SoundManager.Instance.SoundMovePlayer(SoundManager.Instance.soundMovePlayer); }
			}
            else
            {
				MoveView(position, position);
            }

			if (position != lInitialPos)
			{
				undo.Push(lAction);
				redo.Clear();
			}
		}

		public void RedoMove(UndoRedoAction pAction)
        {
			Vector2 lNextPos = position + pAction.playerDirection;

			if(pAction.pushedObject != null)
            {
				pAction.pushedObject.Push(pAction.playerDirection);
				pAction.pushedObject.MoveView(lNextPos, pAction.pushedObject.position);
            }

			Vector2 lPos = position;
			GridPresenter.Move(position, pAction.playerDirection);
			MoveView(lPos, position);

			if (pAction.pulledObject != null)
			{
				pAction.pulledObject.Pull(pAction.playerDirection);
				pAction.pulledObject.MoveView(lNextPos, pAction.pulledObject.position);
			}
		}

		public void UndoMove(UndoRedoAction pAction)
		{
			Vector2 lNextPos = position - pAction.playerDirection;

			if (pAction.pulledObject != null)
			{
				//detacher les poignees qui se sont fixees a ce tour
				if(pAction.pulledHandle.Count != 0)
                {
					for (int i = pAction.pulledHandle.Count - 1; i >= 0; i--)
					{
						((CratePresenter)pAction.pulledObject).DetachHandle(pAction.pulledHandle[i]);
					}
				}

				pAction.pulledObject.ForceMove(-pAction.playerDirection);
				pAction.pulledObject.MoveView(lNextPos, pAction.pulledObject.position);
			}

			Vector2 lPos = position;
			GridPresenter.Move(position, -pAction.playerDirection);
			MoveView(lPos, position);

			if (pAction.pushedObject != null)
			{
				//detacher les poignees qui se sont fixees a ce tour
				if (pAction.pushedHandle.Count != 0)
                {
                    for (int i = pAction.pushedHandle.Count - 1; i >= 0; i--)
                    {
						((CratePresenter)pAction.pushedObject).DetachHandle(pAction.pushedHandle[i]);
					} 
                }

				pAction.pushedObject.ForceMove(-pAction.playerDirection);
				pAction.pushedObject.MoveView(lNextPos, pAction.pushedObject.position);
			}
		}

		public void AttachHandle(MobilePresenter pCrate, Vector2 pDirection)
        {
			UndoRedoAction lAction = undo.Peek();

			if(pCrate == lAction.pushedObject)
            {
				lAction.pushedHandle.Add(pDirection);
            }
            else
            {
				lAction.pulledHandle.Add(pDirection);
            }
        }

		public void Undo()
		{
			if(undo.Count > 0)
            {
				GridPresenter.UndoMovePlayer(undo.Peek());
				redo.Push(undo.Pop());
			}
		}

		public void Redo()
		{
			if (redo.Count > 0)
			{
				GridPresenter.RedoMovePlayer(redo.Peek());
				undo.Push(redo.Pop());
			}
		}


		public override string ToString()
        {
			return "P";
        }
    }

}
