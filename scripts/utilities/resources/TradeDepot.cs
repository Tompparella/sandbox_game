using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TradeDepot : Resources
{
    [Signal]
    public delegate void ConnectNewCaravan(Npc caravan);
    public override void _Ready()
    {
        workerProfession = Constants.CARAVAN_PROFESSION;
        actions = Constants.DEF_WORKACTIONS;
        dialogue = new ResourceDialogue(this, Constants.TRADEDEPOT_DESCRIPTION, actions);

        maxWorkers = 3; // Biggest settlements can have even three caravans per trade depot.

        defaultTexture = Constants.OVEN_TEXTURE;
        defaultPortrait = Constants.OVEN_PORTRAIT;
        defaultName = Constants.OVEN_NAME;
        defaultDescription = Constants.TRADEDEPOT_DESCRIPTION;

        exhaustedDescription = Constants.TRADEDEPOT_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        //createDebugInstance();

        base._Ready();
    }

    public void BeginTradeMission(TradeStall tradeStall) {
        Npc caravan = workers.FirstOrDefault() as Npc;
        Character trader = tradeStall.GetWorkers().FirstOrDefault();
        if (trader != null && caravan != null) {
            //trader.Connect("Refresh", this, nameof(EndTradeMission));
            caravan.AddToGroup(Constants.CARAVAN_GROUP);
            caravan.nearbyTraders.Clear();
            caravan.AddNearbyTrader(trader);
            caravan.hasTraded = false;
            caravan.SetInteractive();
            EmitSignal(nameof(ConnectNewCaravan), caravan);
            GD.Print(string.Format("Caravan '{0}' sent on a trade mission to trader '{1}'", caravan.entityName, trader.entityName));
        }
    }
    /*
    private void EndTradeMission(Character trader) {
        // Handling here for when the trade mission ends, and the caravan returns to the depot.
        Character caravan = ongoingTradeMissions.
        caravan.neededItems.Clear();
        if (ongoingTradeMissions.ContainsKey(caravan)) {
            ongoingTradeMissions.Remove(caravan);
        }
        if (caravan is Npc npcCaravan) {
            npcCaravan.AddVitalSurrounding(this);
        }
    }
*/
/*
    public override void RemoveWorker(Character worker) {
        worker.RemoveFromGroup(Constants.CARAVAN_GROUP);
        worker.EmitSignal("Refresh", worker);
        base.RemoveWorker(worker);
    }
*/
    public override void GiveResource(Character worker) {
    }

    private void createDebugInstance()
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
        debugInstance.RectPosition += new Vector2(200, 0);  // This makes debug visible even if the trader is on top of the tradestall.
		AddChild(debugInstance);
		debugInstance.AddStat("Item Price Modifiers", this, "GetItemPriceModifiersString", true);
	}

/*
    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
*/
}