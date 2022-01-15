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

        createDebugInstance(); // Comment out for production.

        base._Ready();
    }

    public override void workAction(Character worker)
    {
        rand.Randomize();
        try
        {
            int radius = (int)Math.Sqrt(Math.Pow(workRange, 2) / 2);    // Workrange is the maximum hypotenuse
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
        worker.CheckNeeds();
    }

    private float GetGuardDistance() {  // For debugging
        if (workers.Any()) {
            return workers[0].Position.DistanceTo(Position);
        }
        return -1;
    }

    private void createDebugInstance()  // Keep track of distances etc. during development.
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
		AddChild(debugInstance);
		debugInstance.AddStat("Guard Distance", this, "GetGuardDistance", true);
		debugInstance.AddStat("Work Range", this, "workRange", false);
	}
/*
    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
*/
}