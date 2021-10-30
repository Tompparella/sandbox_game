using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TradeStall : Resources
{
    public override void _Ready()
    {
        actions = Constants.TRADEACTIONS;
        dialogue = new ResourceDialogue(this, Constants.LUMBER_DESCRIPTION, actions);

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