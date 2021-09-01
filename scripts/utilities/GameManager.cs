using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : Node
{
    private Godot.Collections.Array characters;
    private InterfaceManager interfaceManager;
    public override void _Ready()
    {
        interfaceManager = (InterfaceManager)GetNode("InterfaceManager");
        characters = GetTree().GetNodesInGroup("character");
        foreach (Node character in characters)
        {
            character.Connect("OnMouseOver", this, nameof(OpenCharacterDialog));
            character.Connect("OnMouseExit", this, nameof(CloseCharacterDialog));
            character.Connect("OnCharacterClick", this, nameof(HandleClickEvent));
        }
    }

    private void OpenCharacterDialog(Character source) {
        interfaceManager.HandleDialogue(source);
    }
    private void CloseCharacterDialog() {
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
