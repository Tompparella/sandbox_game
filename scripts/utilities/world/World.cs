using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class World : Node {
    private Godot.Collections.Array factions;
    public void Initialize(SceneTree sceneTree) {
        GD.Print("World initialization started...");
        Godot.Collections.Array settlements = sceneTree.GetNodesInGroup(Constants.SETTLEMENT_GROUP);
        Godot.Collections.Array caravans = sceneTree.GetNodesInGroup(Constants.CARAVAN_GROUP);
        factions = sceneTree.GetNodesInGroup(Constants.FACTION_GROUP);

        foreach (Faction faction in factions)
        {
            List<Settlement> factionSettlements = new List<Settlement>();
            foreach (Settlement settlement in settlements)
            {
                if (settlement.settlementInfo.settlementFaction == faction.factionInfo) {
                    settlement.Connect("ConnectNewCaravan", this, nameof(ConnectNewCaravan));
                    factionSettlements.Add(settlement);
                }
            }
            faction.Initialize(factionSettlements);
        }
        foreach (Npc caravan in caravans)
        {
            caravan.Connect("GetProfitableTrader", this, nameof(GetProfitableTrader));
        }
        GD.Print("World initialization complete!");
    }
    private void GetProfitableTrader(Npc caravan) {
        /*
        Right now selecting a settlement to trade in for the caravan works so that one settlement is
        chosen from each faction by the estimated potential profit. Finally, the settlement that is
        closest to the caravan is chosen as the place to commence trade in.

        TODO!! Right now, in testing all settlements are chosen per faction, since there's only one
        faction available at the moment. Simply comment the code that chooses two settlements and
        uncomment the lines that choose one.
        */
        IEnumerable<Item> caravanItems = caravan.tradeInventory.GetFilteredItems();
        List<Settlement> promisingSettlements = new List<Settlement>();
        foreach (Faction faction in factions)
        {
            if (caravan.IsEnemy(faction.GetFactionName())) {
                continue;
            }
            //promisingSettlements.Add(faction.GetSettlements().OrderByDescending(x => EstimateDemandInSettlement(x.GetItemDemand(), caravanItems)).FirstOrDefault()); // The actual line
            promisingSettlements.AddRange(faction.GetSettlements().OrderByDescending(x => EstimateDemandInSettlement(x.GetItemDemand(), caravanItems))); // Testing line
        }
        promisingSettlements.OrderBy(x => caravan.Position.DistanceTo(x.Position));
        foreach (Settlement settlement in promisingSettlements)
        {
            Character trader = settlement.settlementTradestalls.Where(x => x.GetWorkers().Any())?.Select(x => x.GetWorkers()[0]).FirstOrDefault();
            if (trader != null && trader != caravan.GetInteractive()) {
                caravan.nearbyTraders.Clear();
                caravan.AddNearbyTrader(trader);
                GD.Print(string.Format("Caravan '{0}' forwards its trade mission to trader '{1}' in settlement '{2}'", caravan.entityName, trader.entityName, settlement.GetSettlementName()));
                /*
                //caravan.hasTraded = false;
                caravan.SetInteractive();
                caravan.ClearSurroundings();
                */
                return;
            }
        }
        // At this point, the caravan hasn't found a suitable trader. Sent it back to a depot.
        ReturnCaravanToDepot(caravan);
        
    }
    private int EstimateDemandInSettlement(Dictionary<string, int> itemDemand, IEnumerable<Item> items) {
        int estimate = 0;
        foreach (KeyValuePair<string,int> kvp in itemDemand)
        {
            switch (kvp.Key)
            {
                case "Food":        // DEF_FOODNAME FROM CONSTANTS
                    estimate += items.Count(x => x is ConsumableItem cItem && cItem.nutritionValue > 0) * kvp.Value;
                    break;
                case "Commodity":   // DEF_COMMODITYNAME FROM CONSTANTS
                    estimate += items.Count(x => x is ConsumableItem cItem && cItem.commodityValue > 0) * kvp.Value;
                    break;
                default:
                    estimate += items.Count(x => x.itemName.Equals(kvp.Key)) * kvp.Value;
                    break;
            }
        }
        return estimate;
    }
    private void ConnectNewCaravan(Npc caravan) {
        if (!caravan.IsConnected("GetProfitableTrader", this, nameof(GetProfitableTrader))) {
            caravan.Connect("GetProfitableTrader", this, nameof(GetProfitableTrader));
        }
        if (!caravan.IsConnected("ReturnCaravanToDepot", this, nameof(ReturnCaravanToDepot))) {
            caravan.Connect("ReturnCaravanToDepot", this, nameof(ReturnCaravanToDepot));
        }
    }
    private void ReturnCaravanToDepot(Npc caravan) {
        if (caravan.IsConnected("GetProfitableTrader", this, nameof(GetProfitableTrader))) {
            caravan.Disconnect("GetProfitableTrader", this, nameof(GetProfitableTrader));
        }
        if (caravan.IsConnected("ReturnCaravanToDepot", this, nameof(ReturnCaravanToDepot))) {
            caravan.Disconnect("ReturnCaravanToDepot", this, nameof(ReturnCaravanToDepot));
        }
        if (caravan.IsInGroup(Constants.CARAVAN_GROUP)) {
            caravan.RemoveFromGroup(Constants.CARAVAN_GROUP);
        }
        foreach (Faction faction in factions)
        {
            if (faction.GetFactionName().Equals(caravan.GetFaction())) {
                caravan.AddVitalSurrounding(faction.GetClosestTradeDepot(caravan.Position));
            }
        }
        GD.Print(string.Format("Caravan '{0}' ends its trade mission and returns to closest trade depot", caravan.entityName));
    }
}