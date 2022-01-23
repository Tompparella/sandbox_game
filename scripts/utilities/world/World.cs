using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class World : Node {
    private Godot.Collections.Array factions;
    public void Initialize(SceneTree sceneTree) {
        GD.Print("World initialization started...");
        Godot.Collections.Array settlements = sceneTree.GetNodesInGroup(Constants.SETTLEMENT_GROUP);
        factions = sceneTree.GetNodesInGroup(Constants.FACTION_GROUP);

        foreach (Faction faction in factions)
        {
            List<Settlement> factionSettlements = new List<Settlement>();
            foreach (Settlement settlement in settlements)
            {
                if (settlement.settlementInfo.settlementFaction == faction.factionInfo) {
                    factionSettlements.Add(settlement);
                }
            }
            faction.Initialize(factionSettlements);
        }
        GD.Print("World initialization complete!");
    }
}