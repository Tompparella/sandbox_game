using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Camp : Refinery
{
    RandomNumberGenerator rand = new RandomNumberGenerator();

    private Timer patrolTimer = new Timer();
    protected int patrolTime = 60;             // Time stayed patrolling/hunting in seconds.
    protected bool patrolling = false;

    public override void _Ready()
    {
        craftableItems = new List<Item>() {
        };

        maxWorkers = 3;

        actions = Constants.DEF_WORKACTIONS;
        dialogue = new ResourceDialogue(this, Constants.CAMP_DESCRIPTION, actions);

        workRange = 20 * Constants.DEF_ATTACKRANGE; // Basically the hunting area.

        defaultTexture = Constants.OVEN_TEXTURE;
        defaultPortrait = Constants.OVEN_PORTRAIT;
        defaultName = Constants.OVEN_NAME;
        defaultDescription = Constants.CAMP_DESCRIPTION;

        exhaustedDescription = Constants.TRADESTALL_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        patrolTimer.OneShot = true;   // Instancing the tradeTimer
        patrolTimer.WaitTime = patrolTime;
        patrolTimer.Connect("timeout", this, nameof(PatrollingOff));
        AddChild(patrolTimer);

        createDebugInstance(); // Comment out for production.

        base._Ready();
    }


    public override bool AddWorker(Character worker) {
        if (worker is Npc && ((Npc)worker).hasTraded && !setWorkItemQueue(worker))
        {
            SetToPatrol();
        }
        workers.Add(worker);
        return true;
    }
    public override void RemoveWorker(Character worker) {
        patrolling = false;
        base.RemoveWorker(worker);
    }

    private void SetToPatrol() {
        // By default, the camper is set to patrol the area. If it finds resources in its area, it will attempt to process it into something.
        patrolling = true;
        patrolTimer.Start();
    }

    private void PatrollingOff() {
        patrolling = false;
    }

    public override void workAction(Character worker)
    {
        if (patrolling) {
            rand.Randomize();
            try
            {
                int radius = (int)Math.Sqrt(Math.Pow(workRange, 2) / 2);    // Workrange is the maximum hypotenuse
                worker.GetMovePath(worker.GlobalPosition, GlobalPosition + new Vector2(rand.RandiRange(-radius, radius), rand.RandiRange(-radius, radius)), worker); // Go for a patrol. This should be improved.
            }
            catch (System.Exception)
            {
                GD.Print("Error on Camp");
                throw;
            }
        } else {
            base.workAction(worker);
        }
    }

/*
    public void SupplySoldiers(int foodAmount, int commodityAmount) {
        List<Npc> soldiers = GetWorkers().Where(x => x is Npc).Cast<Npc>().ToList();
        bool supplyFood, supplyCommodities;
        foreach (Npc soldier in soldiers)
        {
            supplyFood = soldier.GetHungerValue() < Constants.DEF_MAXHUNGER / 2 && foodAmount > 0;
            supplyCommodities = soldier.GetCommoditiesValue() < Constants.DEF_MAXCOMMODITIES / 2 && commodityAmount > 0;

            if (supplyFood || supplyCommodities) {
                foodAmount = supplyFood ? foodAmount - 1 : foodAmount;
                commodityAmount = supplyCommodities ? commodityAmount - 1 : commodityAmount;
                soldier.CheckNeeds();
                soldier.SetInteractive();
            }

            if (foodAmount <= 0 && commodityAmount <= 0) {
                return;
            }
        }
    }
*/

    private float GetCamperDistance() {  // For debugging
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
		debugInstance.AddStat("Camper Distance", this, "GetCamperDistance", true);
		debugInstance.AddStat("Work Range", this, "workRange", false);
	}
/*
    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
*/
}