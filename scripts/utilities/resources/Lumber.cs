using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Lumber : Resources
{
    public override void _Ready()
    {
        actions = Constants.LUMBERACTIONS;
        dialogue = new ResourceDialogue(this, Constants.LUMBER_DESCRIPTION, actions);
        entityName = "Lumber";
        inventory = (Inventory)ResourceLoader.Load(Constants.LUMBER_INVENTORY).Duplicate();

        exhaustedDescription = Constants.TREETRUNK_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }
}