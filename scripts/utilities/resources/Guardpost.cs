using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Guardpost : Resources
{
    public override void _Ready()
    {
        actions = Constants.DEF_WORKACTIONS;
        dialogue = new ResourceDialogue(this, Constants.GUARDPOST_DESCRIPTION, actions);

        defaultTexture = Constants.OVEN_TEXTURE;
        defaultPortrait = Constants.OVEN_PORTRAIT;
        defaultName = Constants.OVEN_NAME;
        defaultDescription = Constants.GUARDPOST_DESCRIPTION;

        exhaustedDescription = Constants.TRADESTALL_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }

    public override void GiveResource(Character worker) {
        if (!isExhausted) {
            worker.inventory.currency++;
        }
    }
/*
    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
*/
}