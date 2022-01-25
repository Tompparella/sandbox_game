using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Faction : Node {
    [Export]
    public FactionInfo factionInfo = new FactionInfo();

    public void Initialize(List<Settlement> settlements) {
        GD.Print(string.Format("Initializing faction {0}", GetFactionName()));
		foreach (Settlement settlement in settlements)
		{
            AddSettlement(settlement);
			settlement.Initialize();
            settlement.Connect("FindAvailableTradeDepot", this, nameof(FindAvailableTradeDepot));
		}
        GD.Print(string.Format("Faction {0} initialization complete.", factionInfo.factionName));
    }
    private void AddSettlement(Settlement settlement) {
        if (!factionInfo.settlements.Contains(settlement)) {
            factionInfo.settlements.Add(settlement);
        }
    }
    private void FindAvailableTradeDepot(TradeStall tradeStall) {
        IEnumerable<Settlement> orderedSettlements = factionInfo.settlements.OrderBy(x => tradeStall.Position.DistanceTo(x.Position));
        foreach (Settlement settlement in orderedSettlements)
        {
            if (settlement.FindLocalTradeDepot(tradeStall)) {
                return;
            }
        }
    }
    ///<summary>Get a tradedepot from the closest settlement to position.</summary>
    public TradeDepot GetClosestTradeDepot(Vector2 position) {
        return GetSettlements().OrderBy(x => position.DistanceTo(x.Position)).Where(x => x.GetLocalTradeDepot() != null).Select(x => x.GetLocalTradeDepot()).FirstOrDefault();
    }
    public string GetFactionName() {
        return factionInfo.factionName;
    }
    public List<string> GetHostileFactions() {
        return factionInfo.hostileFactions;
    }
    public List<Settlement> GetSettlements() {
        return factionInfo.settlements;
    }
}