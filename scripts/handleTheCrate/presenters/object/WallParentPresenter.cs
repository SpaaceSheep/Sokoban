using Com.IsartDigital.HandleTheCrate.Model;
using Godot;
using System.Collections.Generic;

namespace Com.IsartDigital.HandleTheCrate.Presenters.Object
{

	public class WallParentPresenter
	{
		protected List<WallPresenter> walls = new List<WallPresenter>();

		public int index;

		public void AddWall(WallPresenter pWall)
		{
			walls.Add(pWall);
		}

		public bool MoveTowards(Vector2 pDirection)
		{
			if (CanMoveTowards(pDirection))
			{
				foreach(WallPresenter lWall in walls)
				{
					lWall.MoveView(lWall.position, lWall.position + pDirection);
					GridPresenter.Move(lWall, pDirection);
				}

				//certains murs se sont faits ecraser par le mouvement des autres, les faire bouger sur place pour qu'ils se remettent sur la grille
				foreach (WallPresenter lWall in walls)
				{
					GridPresenter.Move(lWall, Vector2.Zero);
				}
				return true;
			}
			return false;
		}

		public void Highlight(float pValue)
		{
			foreach(WallPresenter wall in walls)
			{
				ShaderMaterial lMat = (ShaderMaterial)wall.isoLink.Material;
				wall.isoLink.tween.InterpolateProperty(lMat, "shader_param/offset", lMat.GetShaderParam("offset"), pValue, 0.3f);
				wall.isoLink.tween.Start();

				lMat = (ShaderMaterial)wall.radarLink.Material;
				wall.radarLink.tween.InterpolateProperty(lMat, "shader_param/offset", lMat.GetShaderParam("offset"), pValue, 0.3f);
				wall.radarLink.tween.Start();
			}
		}

		//check si aucun mur est bloque
		protected bool CanMoveTowards(Vector2 pDirection)
		{
			Vector2 lNextPos;
			foreach(WallPresenter lWall in walls)
			{
				lNextPos = lWall.position + pDirection;

				if (!GridModel.IsInBounds(lNextPos)) return false;

				//si qqch bloque
				if(!GridModel.IsEmpty(lNextPos))
				{
					if (!(GridModel.GetAt(lNextPos) is WallPresenter) || !walls.Contains((WallPresenter)GridModel.GetAt(lNextPos)))
					{
						GridModel.GetAt(lNextPos).MoveView(lNextPos, lNextPos);
                        foreach (WallPresenter wall in walls)
                        {
							wall.MoveView(wall.position, wall.position);
                        }
						return false;
					}
				}
			}

			return true;
		}

	}
}