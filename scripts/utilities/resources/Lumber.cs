using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Lumber : Resources
{
    public override void _Ready()
    {
        actions = Constants.LUMBERACTIONS;
        dialogue = new ResourceDialogue(this, Constants.LUMBERDESCRIPTION, actions);
        entityName = "Lumber";

        exhaustedDescription = Constants.TREETRUNK_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }
}