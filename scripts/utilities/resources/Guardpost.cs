using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Guardpost : Resources
{
    RandomNumberGenerator rand = new RandomNumberGenerator();
    public override void _Ready()
    {
        actions = Constants.DEF_WORKACTIONS;
        dialogue = new ResourceDialogue(this, Constants.GUARDPOST_DESCRIPTION, actions);

        requiredActions = (int)Math.Round(requiredActions * 0.25);
        workRange = 10 * Constants.DEF_ATTACKRANGE; // Basically the patrolling area.

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

    public override void workAction(Character worker)
    {
        rand.Randomize();
        try
        {
            int radius = (int)workRange;
            worker.GetMovePath(worker.GlobalPosition, GlobalPosition + new Vector2(rand.RandiRange(-radius, radius), rand.RandiRange(-radius, radius)), worker); // Go for a patrol. This should be improved.
        }
        catch (System.Exception)
        {
            GD.Print("Error on Guardpost");
            throw;
        }
        base.workAction(worker);
    }

    public override void GiveResource(Character worker) { // Something exciting here, such as granting exp instead of currency.
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