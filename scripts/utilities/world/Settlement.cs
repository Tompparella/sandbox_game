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
    private PackedScene packedNpc = (PackedScene)GD.Load(Constants.NPC);
    private Node charactersNode;
    [Export]
    public SettlementInfo settlementInfo = new SettlementInfo();
    [Signal]
    public delegate void SpawnEntity(MovingEntity entity);

    public void Initialize()  // Called from gamemanager to initiate the factions information.
    {
        Godot.Collections.Array areaEntities = GetOverlappingAreas();

        List<Guardpost> guardPosts = new List<Guardpost>();     // We'll find the settlement guardposts here and assign them to barracks evenly.
        List<Resources> resources = new List<Resources>();      // No use at the moment, but the available jobs could be calculated from available resources.
        List<Npc> npcs = new List<Npc>();

        charactersNode = GetNode("../../../NavigationHandler/MapSort/Characters");

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
            if (area is Character character) {
                HandleCharacter(character);
            }
        }
        SetNpcJobs(npcs);
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
    private void NotifySoldiers(Character attacker) {
        settlementBarracks?.ForEach(x => x.AlertSoldiers(attacker));
    }
    private void OnVillagerDied(Character villager) {
        if (settlementInfo.jobsAvailable.ContainsKey(villager.GetProfession())) {
            settlementInfo.jobsAvailable[villager.GetProfession()] += 1;
        }
        SpawnVillager();    // To keep jobs occupied
    }
    private void SpawnVillager() {
		Npc npcInstance = (Npc)packedNpc.Instance().Duplicate();
        npcInstance.Position = Position;
        npcInstance.entityName = "Spawned Villager";
		charactersNode.AddChild(npcInstance);
        EmitSignal(nameof(SpawnEntity), npcInstance);
        GD.Print("Spawned a villager");
        HandleCharacter(npcInstance);
        SetNpcJobs(new List<Npc>() { npcInstance });    // Give the spawned Npc a profession.
    }
    private void HandleCharacter(Character character) {
        if (string.IsNullOrEmpty(character.GetFaction())) {
            character.SetFaction(settlementInfo.settlementFaction);
        }
        if (character.GetFaction().Equals(GetFactionString())) {
            character.Connect("Dead", this, nameof(OnVillagerDied));
            character.Connect("UnderAttack", this, nameof(NotifySoldiers));
            settlementInfo.WorkerAdded(character.GetProfession());
        }
    }

    // Used for development. Will be reworked in the future.
    private void SetNpcJobs(List<Npc> npcs) {

        Dictionary<string,int> jobsAvailable = settlementInfo.jobsAvailable;

        foreach (KeyValuePair<string,int> kvp in jobsAvailable.ToArray())
        {
            for (int i = 0; i < kvp.Value; i++) {
                Npc currentNpc = npcs.FirstOrDefault();
                if (currentNpc == null) {
                    return;
                }
                currentNpc.SetProfession(kvp.Key);  // This needs to be done to update the npc's surrounding resources and enter new workstate.
                currentNpc.SetInteractive();
                //GD.Print(jobsAvailable[kvp.Key]);
                settlementInfo.WorkerAdded(kvp.Key);
                npcs.RemoveAt(0);
                GD.Print(string.Format("Settlement: Handed job {0} to Npc: {1}", kvp.Key, currentNpc.entityName));
            }
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


/*
            switch (kvp.Key)
            {
                case Constants.TRADER_PROFESSION:
                    newResources = resources.Where(x => x is TradeStall).ToList();
                    break;
                case Constants.FARMER_PROFESSION:
                    newResources = resources.Where(x => x is WheatField).ToList();
                    break;
                case Constants.BAKER_PROFESSION:
                    newResources = resources.Where(x => x is Oven).ToList();
                    break;
                case Constants.LUMBERJACK_PROFESSION:
                    newResources = resources.Where(x => x is Lumber).ToList();
                    break;
                case Constants.CRAFTSMAN_PROFESSION:
                    newResources = resources.Where(x => x is Woodcraft).ToList();
                    break;
                case Constants.MINER_PROFESSION:
                    newResources = resources.Where(x => x is Deposit).ToList();
                    break;
                case Constants.BLACKSMITH_PROFESSION:
                    newResources = resources.Where(x => x is Blacksmith).ToList();
                    break;
                case Constants.LOGISTICSOFFICER_PROFESSION:
                    newResources = resources.Where(x => x is Barracks).ToList();
                    break;
                case Constants.SOLDIER_PROFESSION:
                    newResources = resources.Where(x => x is Guardpost).ToList();
                    break;
                default:
                    GD.Print(string.Format("Settlement: Error handling job: {0}", kvp.Key));
                    break;
            }
            */