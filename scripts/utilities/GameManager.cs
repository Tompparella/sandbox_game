using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Node
{
	private InterfaceManager interfaceManager;
	private World world;
	public override void _Ready()
	{
		Initialize();
	}

	async private void Initialize() {

		GD.Print("Initializing GameManager...");
		SceneTree sceneTree = GetTree();
		await ToSignal(sceneTree.CreateTimer(0.01f), "timeout");	// This is done because without waiting, the collisions will not be taken into account.

		// UI initiation
		interfaceManager = (InterfaceManager)GetNode("InterfaceManager");
		Godot.Collections.Array characters = sceneTree.GetNodesInGroup(Constants.CHARACTER_GROUP);
		foreach (Node character in characters)
		{
			ConnectNewCharacter(character);
		}
		Godot.Collections.Array resources = sceneTree.GetNodesInGroup(Constants.RESOURCES_GROUP);
		foreach (Node resource in resources)
		{
			resource.Connect("OnMouseOver", this, nameof(OpenResourceDialog));
			resource.Connect("OnMouseExit", this, nameof(CloseDialogBox));
		}

		// World initiation (Factions, settlements, etc.)
		world = (World)GetNode("World");
		world.Initialize(sceneTree);

		// Spawner initiation
		Godot.Collections.Array spawners = sceneTree.GetNodesInGroup(Constants.SPAWNER_GROUP);
		foreach (Node spawner in spawners)
        {
            spawner.Connect("SpawnEntity", this, nameof(ConnectNewCharacter));
        }

		GD.Print("GameManager Initialization Completed.");
	}

	private void ConnectNewCharacter(Node character) {
		character.Connect("OnMouseOver", this, nameof(OpenCharacterDialog));
		character.Connect("OnMouseExit", this, nameof(CloseDialogBox));
		character.Connect("OnCharacterClick", this, nameof(HandleClickEvent));
	}

	private void OpenCharacterDialog(Character source) {
		interfaceManager.HandleDialogue(source: source);
	}
	private void OpenResourceDialog(Resources resource) {
		interfaceManager.HandleDialogue(resource: resource);
	}

	private void CloseDialogBox() {
		interfaceManager.HandleDialogue();
	}
	private void HandleClickEvent(Character source, InputEvent @event) {
		if (@event.IsActionPressed("R-Click")) {
			interfaceManager.OpenActionMenu(source);
		}
	}
	private void OpenActionMenu() {

	}
}
