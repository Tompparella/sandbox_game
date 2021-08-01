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
            character.Connect("OnCharacterClick", this, nameof(OpenCharacterDialog));
        }
    }

    private void OpenCharacterDialog(Character source) {
        interfaceManager.HandleDialogue(source);
    }
}
