using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Faction : Node {
    [Export]
    public FactionInfo factionInfo = new FactionInfo();

    public void Initialize(List<Settlement> settlements) {
        GD.Print(string.Format("Initializing faction {0}", factionInfo.factionName));
		foreach (Settlement settlement in settlements)
		{
            AddSettlement(settlement);
			settlement.Initialize();
		}
        GD.Print(string.Format("Faction {0} initialization complete.", factionInfo.factionName));
    }
    private void AddSettlement(Settlement settlement) {
        if (!factionInfo.settlements.Contains(settlement)) {
            factionInfo.settlements.Add(settlement);
        }
    }
}