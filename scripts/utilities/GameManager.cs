using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Node
{
    private Godot.Collections.Array characters;
    private Godot.Collections.Array resources;
    private InterfaceManager interfaceManager;
    public override void _Ready()
    {
        interfaceManager = (InterfaceManager)GetNode("InterfaceManager");
        characters = GetTree().GetNodesInGroup("character");
        foreach (Node character in characters)
        {
            character.Connect("OnMouseOver", this, nameof(OpenCharacterDialog));
            character.Connect("OnMouseExit", this, nameof(CloseDialogBox));
            character.Connect("OnCharacterClick", this, nameof(HandleClickEvent));
        }

        resources = GetTree().GetNodesInGroup("resource");
        foreach (Node resource in resources)
        {
            resource.Connect("OnMouseOver", this, nameof(OpenResourceDialog));
            resource.Connect("OnMouseExit", this, nameof(CloseDialogBox));
        }
    }

    private void OpenCharacterDialog(Character source) {
        interfaceManager.HandleDialogue(source: source);
    }
    private void OpenResourceDialog(Resource resource) {
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
