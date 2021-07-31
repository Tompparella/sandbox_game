using Godot;
using System;

public class Player : Character
{
	private Camera2D camera;
	private Vector2 zoomAmount = new Vector2(0.25f,0.25f);
	private Vector2 minZoom = new Vector2(0.5f,0.5f);
	private Vector2 maxZoom = new Vector2(5,5);
	private Vector2 zeroVector = new Vector2(0,0);

	public override void _Ready()
    {
		camera = (Camera2D)GetNode("PlayerCamera");
    }

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ScrollIn")) {
			camera.Zoom -= camera.Zoom > minZoom ? zoomAmount : zeroVector;
			return;
		} else if (@event.IsActionPressed("ScrollOut")) {
			camera.Zoom += camera.Zoom < maxZoom ? zoomAmount : zeroVector;
			return;
		}
	}
}