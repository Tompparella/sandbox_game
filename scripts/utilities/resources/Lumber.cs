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
        entityName = Constants.TREE_NAME;
        inventory = (Inventory)ResourceLoader.Load(Constants.LUMBER_INVENTORY).Duplicate();

        defaultTexture = Constants.TREE_TEXTURE;
        defaultPortrait = Constants.TREE_PORTRAIT;
        defaultName = entityName;
        defaultDescription = Constants.LUMBER_DESCRIPTION;
        defaultInventory = Constants.LUMBER_INVENTORY;

        exhaustedDescription = Constants.TREETRUNK_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }
}