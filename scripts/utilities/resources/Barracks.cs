using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Barracks : Resources
{
    public override void _Ready()
    {

        actions = Constants.CRAFTACTIONS;
        dialogue = new ResourceDialogue(this, Constants.WOODCRAFT_DESCRIPTION, actions);

        defaultTexture = Constants.WOODCRAFT_TEXTURE;
        defaultPortrait = Constants.WOODCRAFT_PORTRAIT;
        defaultName = Constants.WOODCRAFT_NAME;
        defaultDescription = Constants.WOODCRAFT_DESCRIPTION;
        
        exhaustedDescription = Constants.OVEN_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }
}