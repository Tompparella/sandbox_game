using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Npc : Character
{
    public List<Resources> surroundingResources { get; private set; } = new List<Resources>();
    public List<Npc> nearbyTraders { get; private set; } = new List<Npc>();
    public List<Item> neededItems { get; private set; } = new List<Item>();
    public List<Item> soldItems { get; private set; } = new List<Item>();

    [Export]
    public string profession { get; private set; }

    public bool hasTraded = true;
    public bool outOfWork = false; // If there's nothing for the Npc to do, idle.

    public void _OnSurroundingsEntered(Area2D area) {
        switch (profession)
        {
            case Constants.LUMBERJACK_PROFESSION:
                if (area is Lumber) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;

            case Constants.MINER_PROFESSION:
                if (area is Deposit) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;
            
            case Constants.FARMER_PROFESSION:
                if (area is WheatField) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;
            case Constants.TRADER_PROFESSION:
                if (area is TradeStall) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;
            case Constants.BAKER_PROFESSION:
                if (area is Oven) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;
                
            default:
                break;
        }
        if (area.IsInGroup("traders")) {
            nearbyTraders.Add((Npc)area);
        }
    }
    public bool WorkableResourcesExist() {
        return surroundingResources.Any() && !(surroundingResources[0] is Refinery);
    }
    public void _OnSurroundingsExited(Area2D area) {
        if (surroundingResources.Contains(area)) {
            if (area.IsConnected("OnRemoval", this, nameof(SurroundingRemoved))) {
                area.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
            }
            surroundingResources.Remove((Resources)area);
        }
    }
    private void SurroundingRemoved(Interactive resource) {
        resource.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
        surroundingResources.Remove((Resources)resource);
    }
    public bool GetNextWork() {
        if (inventory.IsFull()) {
            return false;
        }
        bool foundWork = surroundingResources.Any();
        Resources currentResource = null;
        float shortestDistance = float.MaxValue;
        int leastWorkers = int.MaxValue;
        foreach(Resources i in surroundingResources.ToList()) {
            if ((currentResource == null || i.GetWorkers().Count() <= leastWorkers)) {
                if (!IsInstanceValid(i) || i.GetExhausted()) {
                    surroundingResources.Remove(i);
                    continue;
                }
                if (Position.DistanceTo(i.Position) < shortestDistance) {
                    currentResource = i;
                    shortestDistance = Position.DistanceTo(currentResource.Position);
                    leastWorkers = i.GetWorkers().Count();
                }
            }
        }
        SetInteractive(currentResource);
        return foundWork;
    }

    public override bool checkBuyQueue(Dictionary<Item,int> items) {
        bool itemsAdded = false;
        foreach(KeyValuePair<Item, int> kvp in items) {
            int itemsInQueue = neededItems.Where(x => x == kvp.Key).Count();
            if (itemsInQueue < kvp.Value) {
                for (int i = itemsInQueue; i < kvp.Value; i++) {
                    neededItems.Add(kvp.Key);
                    GD.Print(string.Format("Item added to buy queue. {0}", kvp.Key.itemName));
                }
                itemsAdded = true;
            }
        }
        neededItems.ForEach(x => GD.Print(x.itemName));
        return itemsAdded;
    }
    public bool checkBuyQueue() {
        return neededItems.Any();
    }
    public List<Item> GetBuyQueue() {
        return neededItems;
    }
    public override List<Item> GetSellQueue() {
        return soldItems;
    }
    public override void AddToSellQueue(Item item) {
        soldItems.Add(item);
    }
    public override void PopFromSellQueue(Item item)
    {
        soldItems.Remove(item);
    }
    public bool GetTrader() {
        //GD.Print(string.Format("{0} found {1} traders.", this.Name, nearbyTraders.Count()));
        if (nearbyTraders.Any() && !hasTraded) {
            SelectTrader();
            return true;
        }
        return false;
    }

    private void SelectTrader() {
        Npc trader = null;
        float shortestDistance = float.MaxValue;
        foreach(Npc i in nearbyTraders) {
            if ((trader == null || Position.DistanceTo(i.Position) < shortestDistance)) {
                if (!IsInstanceValid(i)) {
                    nearbyTraders.Remove(i);
                    continue;
                }
                trader = i;
                shortestDistance = Position.DistanceTo(trader.Position);
            }
        }
        SetInteractive(trader);
    }

    private void createDebugInstance() {
        PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
        DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
        AddChild(debugInstance);
        debugInstance.AddStat("Profession", this, "profession", false);
        debugInstance.AddStat("Surrounding Resources", this, "surroundingResources", false);
        debugInstance.AddStat("Nearby Traders", this, "nearbyTraders", false);
        debugInstance.AddStat("Needed Items", this, "neededItems", false);
        debugInstance.AddStat("Sold Items", this, "soldItems", false);
        debugInstance.AddStat("Has Traded", this, "hasTraded", false);
        debugInstance.AddStat("Out Of Work", this, "outOfWork", false);
    }

    public override void _Ready()
    {
        if (profession == null) {
            Random r = new Random();
            int index = r.Next(Constants.PROFESSIONS.Count());
            profession = Constants.PROFESSIONS[index];
        }
        createDebugInstance();
        base._Ready();
    }
}