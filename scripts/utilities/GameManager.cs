using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Node
{
	private Godot.Collections.Array characters;
	private Godot.Collections.Array resources;
	private Godot.Collections.Array settlements;	// This will be replaced with factions, that in turn keep count of settlements.
	private InterfaceManager interfaceManager;
	public override void _Ready()
	{
		Initialize();
	}

	async private void Initialize() {				// This is done because without waiting, the collisions will not be taken into account.

		GD.Print("Initializing GameManager...");
		SceneTree sceneTree = GetTree();
		await ToSignal(sceneTree.CreateTimer(0.01f), "timeout");

		// UI initiation
		interfaceManager = (InterfaceManager)GetNode("InterfaceManager");
		characters = sceneTree.GetNodesInGroup(Constants.CHARACTER_GROUP);
		foreach (Node character in characters)
		{
			character.Connect("OnMouseOver", this, nameof(OpenCharacterDialog));
			character.Connect("OnMouseExit", this, nameof(CloseDialogBox));
			character.Connect("OnCharacterClick", this, nameof(HandleClickEvent));
		}

		// Resources initiation
		resources = sceneTree.GetNodesInGroup(Constants.RESOURCES_GROUP);
		foreach (Node resource in resources)
		{
			resource.Connect("OnMouseOver", this, nameof(OpenResourceDialog));
			resource.Connect("OnMouseExit", this, nameof(CloseDialogBox));
		}

		// Factions initiation
		settlements = sceneTree.GetNodesInGroup(Constants.SETTLEMENT_GROUP); // Placeholder. Should be handled by faction.
		foreach (Settlement settlement in settlements)
		{
			settlement.Initiate();
		}
		GD.Print("GameManager Initialization Completed.");
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
