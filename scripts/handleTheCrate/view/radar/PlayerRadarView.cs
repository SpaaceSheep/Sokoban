using Com.IsartDigital.HandleTheCrate.Model;
using Com.IsartDigital.HandleTheCrate.Presenters;
using Godot;
using System;

namespace Com.IsartDigital.HandleTheCrate.View.Radar
{

	public class PlayerRadarView : ObjectRadarView
	{
        protected const string PRESSED = "pressed";

        [Export]
        protected NodePath buttonPath;
        protected Button moveButton;

        [Export]
        protected NodePath feetPath;
        protected Node2D feet;

        public override void _Ready()
        {
            base._Ready();
            moveButton = (Button)GetNode(buttonPath);
            moveButton.Connect(PRESSED, this, nameof(MouseMove));
            feet = (Node2D)GetNode(feetPath);
        }

        protected void MouseMove()
        {
            Vector2 lMouseDirection = feet.GetLocalMousePosition();
            lMouseDirection = lMouseDirection.Rotated(Mathf.Pi / 2).Normalized();

            Vector2 lMoveDirection = Vector2.Up.Rotated(Mathf.Round(Mathf.Atan2(lMouseDirection.y, lMouseDirection.x) / Mathf.Pi * 2) * Mathf.Pi / 2).Round();

            GridPresenter.MovePlayer(lMoveDirection);
        }

    }

}