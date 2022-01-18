using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Settlement : Area2D {
    /*
    Settlement holds information about all important nodes within a town/city/fortress.
    It sends signals to barracks, tradestalls etc. to control its soldiers, prices, etc.
    */

    private List<Barracks> settlementBarracks = new List<Barracks>();
    private List<TradeStall> settlementTradestalls = new List<TradeStall>();
    [Export]
    public SettlementInfo settlementInfo = new SettlementInfo();

    public void Initialize()  // Called from gamemanager to initiate the factions information.
    {
        Godot.Collections.Array areaEntities = GetOverlappingAreas();

        List<Guardpost> guardPosts = new List<Guardpost>();     // We'll find the settlement guardposts here and assign them to barracks evenly.
        List<Resources> resources = new List<Resources>();
        List<Npc> npcs = new List<Npc>();

        foreach (Area2D area in areaEntities)
        {
            switch (area)
            {
                case Barracks barracks:
                    settlementBarracks.Add(barracks);
                    GD.Print(string.Format("{0} added to settlement info", barracks.entityName));
                    break;
                case TradeStall tradeStall:
                    settlementTradestalls.Add(tradeStall);
                    GD.Print(string.Format("{0} added to settlement info", tradeStall.entityName));
                    break;
                case Guardpost guardPost:
                    guardPosts.Add(guardPost);
                    GD.Print(string.Format("{0} found.", guardPost.entityName));
                    break;
                case Resources resource:
                    resources.Add(resource);
                    GD.Print(string.Format("{0} found as resource", resource.entityName));
                    break;
                case Npc npc:
                    // Sketchy shit
                    if (string.IsNullOrEmpty(npc.GetProfession())) {
                        npcs.Add(npc);
                    }
                    break;
                default:
                    break;
            }
            if (area is Character character && string.IsNullOrEmpty(character.GetFaction())) {
                character.SetFaction(settlementInfo.settlementFaction);
            }
        }
        SetNpcJobs(npcs, resources);
        AssignGuardPostsToBarracks(guardPosts);
    }

    private void AssignGuardPostsToBarracks(List<Guardpost> guardPosts) {
        if (guardPosts.Any() && settlementBarracks.Any()) {
            int guardPostsPerBarracks = guardPosts.Count() / settlementBarracks.Count();
            foreach (Barracks barracks in settlementBarracks)
            {
                for (int i = 0; i < guardPostsPerBarracks; i++) {
                    if (guardPosts.Any()) {
                        Guardpost currentPost = guardPosts.First();
                        barracks.AddGuardPost(currentPost);
                        guardPosts.RemoveAt(0);
                    }
                }
                if (barracks.Equals(settlementBarracks.Last()) && guardPosts.Any()) {
                    guardPosts.ForEach(x => barracks.AddGuardPost(x));    // If there's still unassigned guardposts, assign those to the last barracks in the list.
                }
                barracks.Initialize();
            }
        }
    }

    // Used for development. Will be reworked in the future.
    private void SetNpcJobs(List<Npc> npcs, List<Resources> resources) {
        foreach (Resources resource in resources)
        {
            Npc currentNpc = npcs.FirstOrDefault();
            if (currentNpc == null) {
                return;
            }
            string newProfession = "";

            switch (resource)
            {
                case WheatField field:
                    newProfession = Constants.FARMER_PROFESSION;
                    break;
                case Lumber lumber:
                    newProfession = Constants.LUMBERJACK_PROFESSION;
                    break;
                case Deposit deposit:
                    newProfession = Constants.MINER_PROFESSION;
                    break;
                case Oven oven:
                    newProfession = Constants.MINER_PROFESSION;
                    break;
                case Woodcraft craft:
                    newProfession = Constants.CRAFTSMAN_PROFESSION;
                    break;
                case Blacksmith smith:
                    newProfession = Constants.BLACKSMITH_PROFESSION;
                    break;
                case TradeStall trade:
                    newProfession = Constants.TRADER_PROFESSION;
                    break;
                default:
                    break;
            }
            currentNpc.SetProfession(newProfession);
            currentNpc.surroundingResources.Clear();
            currentNpc.surroundingResources.Add(resource);
            currentNpc.SetInteractive();
            npcs.RemoveAt(0);
        }
        if (npcs.Any()) {
            SetNpcJobs(npcs, resources);
        }
    }

    // Debugging
    private string GetFactionString() {
        return settlementInfo.settlementFaction.factionName;
    }
    private string GetHostileFactionsString() {
        return string.Join(", " ,settlementInfo.settlementFaction.hostileFactions);
    }

    private void createDebugInstance()  // Keep track of faction, etc. during development.
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
		AddChild(debugInstance);
		debugInstance.AddStat("Faction", this, "GetFactionString", true);
		debugInstance.AddStat("Hostile Factions", this, "GetHostileFactionsString", true);
	}
}