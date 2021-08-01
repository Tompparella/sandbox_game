using Godot;
using System;

public class InterfaceManager : CanvasLayer
{
    public static DialogueManager dialogueManager;
    private Camera2D camera;
	private Vector2 zoomAmount = new Vector2(0.25f,0.25f);
	private Vector2 minZoom = new Vector2(0.5f,0.5f);
	private Vector2 maxZoom = new Vector2(5,5);
	private Vector2 zeroVector = new Vector2(0,0);
    private Character player;

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ScrollIn")) {
			camera.Zoom -= camera.Zoom > minZoom ? zoomAmount : zeroVector;
			return;
		} else if (@event.IsActionPressed("ScrollOut")) {
			camera.Zoom += camera.Zoom < maxZoom ? zoomAmount : zeroVector;
			return;
		}
	}
    public override void _Process(float delta)
    {
        camera.Position = player.Position;   
    }
    public override void _Ready()
    {
        player = (Character)GetNode("../NavigationHandler/MapSort/Characters/Player");
        camera = (Camera2D)GetNode("MainCamera");
        dialogueManager = (DialogueManager)GetNode("DialogueManager");
    }

    public void HandleDialogue(Character source) {
        dialogueManager.ShowDialogueBox();
    }
}
