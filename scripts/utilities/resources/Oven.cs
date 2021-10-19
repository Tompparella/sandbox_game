using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Oven : Refinery
{
    public override void _Ready()
    {
        craftableItems = new List<Item>() {
            (Item)GD.Load(Constants.BREADITEM),
            (Item)GD.Load(Constants.FLOURITEM),
        };

        actions = Constants.OVENACTIONS;
        dialogue = new ResourceDialogue(this, Constants.OVEN_DESCRIPTION, actions);

        exhaustedDescription = Constants.OVEN_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }
}