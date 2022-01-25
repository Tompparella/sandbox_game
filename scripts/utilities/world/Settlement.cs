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
    public List<TradeStall> settlementTradestalls { get; private set; } = new List<TradeStall>();
    public List<TradeDepot> settlementTradedepots { get; private set; } = new List<TradeDepot>();
    private PackedScene packedNpc = (PackedScene)GD.Load(Constants.NPC);
    private Dictionary<string, int> itemDemandChange = new Dictionary<string, int>();
    private int priceUpdateTime = 120;
    private Timer updatePricesTimer = new Timer();
    private Node charactersNode;
    
    [Export]
    public SettlementInfo settlementInfo = new SettlementInfo();
    [Signal]
    public delegate void SpawnEntity(MovingEntity entity);
    [Signal]
    public delegate void UpdatePrices(Dictionary<string,int> itemDemand);
    [Signal]
    public delegate void FindAvailableTradeDepot(TradeStall tradeStall);
    [Signal]
    public delegate void ConnectNewCaravan(Npc caravan);

    public void Initialize()  // Called from gamemanager to initiate the factions information.
    {
        GD.Print(string.Format("Initializing settlement {0}...", settlementInfo.settlementName));
        Godot.Collections.Array areaEntities = GetOverlappingAreas();

        List<Guardpost> guardPosts = new List<Guardpost>();     // We'll find the settlement guardposts here and assign them to barracks evenly.
        //List<Resources> resources = new List<Resources>();    // No use at the moment, but the available jobs could be calculated from available resources.
        List<Npc> npcs = new List<Npc>();

        charactersNode = GetNode("../../../NavigationHandler/MapSort/Characters");

        foreach (Area2D area in areaEntities)
        {
            switch (area)
            {
                case Barracks barracks:
                    settlementBarracks.Add(barracks);
                    //GD.Print(string.Format("{0} added to settlement info", barracks.entityName));
                    break;
                case TradeStall tradeStall:
                    tradeStall.Connect("OnItemSold", this, nameof(OnTraderItemSold));
                    tradeStall.Connect("OnItemBought", this, nameof(OnTraderItemBought));
                    tradeStall.Connect("BeginTradeMission", this, nameof(BeginTradeMission));
                    Connect(nameof(UpdatePrices), tradeStall, "UpdatePrices");
                    settlementTradestalls.Add(tradeStall);
                    //GD.Print(string.Format("{0} added to settlement info", tradeStall.entityName));
                    break;
                case TradeDepot tradeDepot:
                    tradeDepot.Connect("ConnectNewCaravan", this, nameof(ForwardCaravanConnect));
                    settlementTradedepots.Add(tradeDepot);
                    break;
                case Guardpost guardPost:
                    guardPosts.Add(guardPost);
                    //GD.Print(string.Format("{0} found.", guardPost.entityName));
                    break;
                /*
                case Resources resource:
                    resources.Add(resource);
                    GD.Print(string.Format("{0} found as resource", resource.entityName));
                    break;
                */
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
        InitializeTimer();
        GD.Print(string.Format("Settlement {0} initialization complete!", settlementInfo.settlementName));
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
    private void InitializeTimer() {
		updatePricesTimer.WaitTime = priceUpdateTime;
		updatePricesTimer.Connect("timeout", this, nameof(UpdateItemDemand));
		AddChild(updatePricesTimer);
        updatePricesTimer.Start();
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
            character.Connect("OnItemWanted", this, nameof(OnTraderItemWanted));
            settlementInfo.WorkerAdded(character.GetProfession());
        }
    }

    // Trade handling. Controls item prices based on need and supply.
    /// <summary> When an Npc needs an item to buy, the price of the item increases locally. </summary>
    private void OnTraderItemWanted(Item item) {
        if (itemDemandChange.ContainsKey(item.itemName)) {
            itemDemandChange[item.itemName] += 1;
        } else {
            itemDemandChange.Add(item.itemName, 1);
        }
    }
    /// <summary> When the trader sells an item, the value of the item increases in steps </summary>
    private void OnTraderItemSold(Item item) {
        string itemName = item is ConsumableItem cItem ? (cItem.nutritionValue > 0 ? Constants.DEF_FOODNAME : cItem.commodityValue > 0 ? Constants.DEF_COMMODITYNAME : Constants.DEF_CONSUMABLENAME) : item.itemName;
        if (itemDemandChange.ContainsKey(itemName)) {
            itemDemandChange[itemName] += 1;
        } else {
            itemDemandChange.Add(itemName, 1);
        }
    }
    /// <summary> When the trader buys an item, the value the item decreases in steps. </summary>
    private void OnTraderItemBought(Item item) {
        string itemName = item is ConsumableItem cItem ? (cItem.nutritionValue > 0 ? Constants.DEF_FOODNAME : cItem.commodityValue > 0 ? Constants.DEF_COMMODITYNAME : Constants.DEF_CONSUMABLENAME) : item.itemName;
        if (itemDemandChange.ContainsKey(itemName)) {
            itemDemandChange[itemName] -= 1;
        } else {
            itemDemandChange.Add(itemName, -1);
        }
    }
    private void UpdateItemDemand() {
        foreach (KeyValuePair<string, int> kvp in itemDemandChange)
        {
            settlementInfo.UpdateItemDemand(kvp);
        }
        itemDemandChange.Clear();
        EmitSignal(nameof(UpdatePrices), settlementInfo.itemDemand);
        GD.Print(string.Format("Settlement {0}: Prices Updated", settlementInfo.settlementName));
    }
    public Dictionary<string, int> GetItemDemand() {
        return settlementInfo.itemDemand;
    }
    private void BeginTradeMission(TradeStall tradeStall) {
        if (!FindLocalTradeDepot(tradeStall)) {                                         // If a local caravan is found, immediately send it on a trade mission. Otherwise, find one from other faction settlements.
            EmitSignal(nameof(FindAvailableTradeDepot), tradeStall);
        }
    }
    private void ForwardCaravanConnect(Npc caravan) {
        EmitSignal(nameof(ConnectNewCaravan), caravan);
    }

    ///<summary>A function used to set a caravan on a trademission. To simply get a depot, use GetLocalTradeDepot</summary>
    public bool FindLocalTradeDepot(TradeStall tradeStall) {                            // Returns true or false based on success.
        foreach (TradeDepot depot in settlementTradedepots)
        {
            if (depot.GetWorkers().Any()) {
                depot.BeginTradeMission(tradeStall);
                return true;
            }
        }
        return false;
    }

    ///<summary>Gets a local trade depot for caravans.</summary>
    public TradeDepot GetLocalTradeDepot() {
        return settlementTradedepots.FirstOrDefault(x => x.GetWorkerNumber() < x.maxWorkers);
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
                //GD.Print(string.Format("Settlement: Handed job {0} to Npc: {1}", kvp.Key, currentNpc.entityName));
            }
        }
    }

    public string GetSettlementName() {
        return settlementInfo.settlementName;
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