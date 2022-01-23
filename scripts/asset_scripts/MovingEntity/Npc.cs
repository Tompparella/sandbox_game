using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Npc : Character
{
	public List<Resources> surroundingResources { get; private set; } = new List<Resources>();
	public List<Character> nearbyTraders { get; private set; } = new List<Character>();
	public List<Item> soldItems { get; private set; } = new List<Item>();


	public bool hasTraded = true;
	public bool outOfWork = false; // If there's nothing for the Npc to do, idle.

	public Timer outOfWorkTimer = new Timer();

	public void _OnSurroundingsEntered(Area2D area)
	{
		switch(area) {
			case Resources resource:
				if (resource.workerProfession.Equals(GetProfession())) {
				outOfWork = false;
				}
				surroundingResources.Add(resource);
				area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
			break;
			case Character character:
				character.Connect("Refresh", this, nameof(_RefreshSurroundingCharacter));
				if (GetProfession().Equals(Constants.SOLDIER_PROFESSION)) {
					if (character.IsInGroup(Constants.LOGISTICS_GROUP)) {
						AddNearbyTrader(character);
					}
				} else if (character.IsInGroup(Constants.TRADER_GROUP)) {
					AddNearbyTrader(character);
				}
			break;
		}
	}
	public void _OnSurroundingsExited(Area2D area)
	{
		switch (area)
		{
			case Resources resource:
				if (surroundingResources.Contains(resource))
				{
					if (resource.IsConnected("OnRemoval", this, nameof(SurroundingRemoved)))
					{
						resource.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
					}
					surroundingResources.Remove(resource);
				}
			break;
			case Character character:
				if (nearbyTraders.Count() > 1 && nearbyTraders.Contains(character))	// Most Npc's always need at least 1 nearby trader. This can be redone.
				{
					nearbyTraders.Remove(character);
				}
				if (character.IsConnected("Refresh", this, nameof(_RefreshSurroundingCharacter))) {
					character.Disconnect("Refresh", this, nameof(_RefreshSurroundingCharacter));
				}
				break;
			default:
			break;
		}
	}

	public void _OnProximityEntered(Area2D area) {
		if (aggressive && area is Character character) {
			if (IsEnemy(character.GetFaction())) {
				AttackTarget(character);
			}
		}
	}
	public void _OnProximityExited(Area2D area) {
	}

	public void _RefreshSurroundingCharacter(Character character) {

		if (nearbyTraders.Contains(character)) {
			nearbyTraders.Remove(character);
		}
		if (GetProfession().Equals(Constants.SOLDIER_PROFESSION)) {
			if (character.IsInGroup(Constants.LOGISTICS_GROUP)) {
				AddNearbyTrader(character);
			}
		} else if (character.IsInGroup(Constants.TRADER_GROUP)) {
			nearbyTraders.Add(character);
		}
	}

	public bool WorkableResourcesExist()
	{
		return (surroundingResources.Any(x => x.workerProfession.Equals(GetProfession())));
	}

	/*
	public void ClearSurroundings() {
		surroundingResources.ToList().ForEach(x => _OnSurroundingsExited(x));
	}
	public void AddSurroundings(List<Resources> newSurroundings) {
		newSurroundings.ForEach(x => _OnSurroundingsEntered(x));
	}
	*/

	private void SurroundingRemoved(Interactive resource)
	{
		if (!WorkableResourcesExist())
		{
			outOfWork = true;
		}
		resource.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
		surroundingResources.Remove((Resources)resource);
	}

	public void AddNearbyTrader(Character trader) {
		if (!nearbyTraders.Contains(trader)) {
			nearbyTraders.Add(trader);
		}
	}

	public void ToggleOutOfWork()
	{
		//GD.Print(String.Format("{0}: Started outOfWorkTimer", Name));

		CheckNeeds();
		if (GetBuyQueue().Any())
		{
			hasTraded = false;
			outOfWork = false;
		}
		if (!WorkableResourcesExist())
		{      // If the Npc is out of work, put a timer for 1 minutes to enter work state again.
			outOfWorkTimer.Start();
		}
		else
		{
			outOfWork = false;
		}
	}
	public bool GetNextWork()
	{
		if (inventory.IsFull())
		{
			return false;
		}
		bool foundWork = false;
		Resources currentResource = null;
		float shortestDistance = float.MaxValue;
		int leastWorkers = int.MaxValue;
		foreach (Resources i in GetSurroundingWorkableResources())
		{
			if ((currentResource == null || i.GetWorkerNumber() <= leastWorkers))
			{
				if (!IsInstanceValid(i) || i.GetExhausted())
				{
					surroundingResources.Remove(i);
					continue;
				} else if (i.maxWorkers <= i.GetWorkerNumber()) {
					continue;
				}
				if (Position.DistanceTo(i.Position) < shortestDistance && i.maxWorkers > i.GetWorkerNumber())
				{
					foundWork = true;
					currentResource = i;
					shortestDistance = Position.DistanceTo(currentResource.Position);
					leastWorkers = i.GetWorkers().Count();
				}
			}
		}
		SetInteractive(currentResource);
		return foundWork;
	}
	private List<Resources> GetSurroundingWorkableResources() {
		return surroundingResources.Where(x => x.workerProfession.Equals(GetProfession())).ToList();
	}

	private bool IsEnemy(string factionName) {
		if (stats.faction != null) {
			return stats.faction.hostileFactions.Contains(factionName);
		}
		return false;
	}

	public float GetHungerValue()
	{
		return stats.hunger;
	}
	public float GetCommoditiesValue()
	{
		return stats.commodities;
	}
	public float GetCurrentHealth()
	{
		return stats.currentHealth;
	}
	public override void GetFood()
	{
		addFoodToBuyQueue();
	}
	public override void GetCommodities()
	{
		addCommoditiesToBuyQueue();
	}

	public void addFoodToBuyQueue()
	{
		if (!neededItems.Any(x => x is ConsumableItem && ((ConsumableItem)x).nutritionValue > 0))
		{
			ConsumableItem foodItem = new ConsumableItem(_nutritionValue: 1);
			neededItems.Add(foodItem);
			EmitSignal("OnItemWanted", foodItem);
		}
	}
	public void addCommoditiesToBuyQueue()
	{
		if (!neededItems.Any(x => x is ConsumableItem && ((ConsumableItem)x).commodityValue > 0))
		{
			ConsumableItem commodityItem = new ConsumableItem(_commodityValue: 1);
			neededItems.Add(commodityItem);
			EmitSignal("OnItemWanted", commodityItem);
		}
	}

	public void ClearFoodFromBuyQueue() {
		//GD.Print(string.Format("Npc '{0}' cleared fooditems", entityName));
		neededItems.RemoveAll(x => x is ConsumableItem && ((ConsumableItem)x).nutritionValue > 0);
	}

	public void ClearCommoditiesFromBuyQueue() {
		//GD.Print(string.Format("Npc '{0}' cleared commodities", entityName));
		neededItems.RemoveAll(x => x is ConsumableItem && ((ConsumableItem)x).commodityValue > 0);
	}

	public override List<Item> GetSellQueue()
	{
		return soldItems;
	}
	public override void AddToSellQueue(Item item)
	{
		soldItems.Add(item);
	}
	public override void PopFromSellQueue(Item item)
	{
		soldItems.Remove(item);
	}

	public override void PopFromBuyQueue(Item item)
	{
		neededItems.Remove(item);
	}

	public bool GetTrader()
	{
		//GD.Print(string.Format("{0} found {1} traders.", this.Name, nearbyTraders.Count()));
		if (nearbyTraders.Any() && !hasTraded)
		{
			SelectTrader();
			return true;
		}
		hasTraded = true;
		return false;
	}

	// These are used mainly for debugging.
	private string GetNeededItemsString() {
		//GD.Print(string.Join("," ,neededItems.Select(x => x.itemName)));
		return string.Join(", " ,neededItems.Select(x => x.itemName));
	}
	private string GetNearbyTradersString() {
		return string.Join(", " ,nearbyTraders.Select(x => x.entityName));
	}
	private string GetSurroundingWorkableResourcesString() {
		return string.Join(", " ,GetSurroundingWorkableResources().Select(x => x.entityName));
	}
	private string GetHostilesString() {
		if (stats.faction != null) {
			return string.Join(", " ,stats.faction?.hostileFactions);
		} else {
			return "";
		}
	}

	private void SelectTrader()
	{
		Character trader = null;
		float shortestDistance = float.MaxValue;
		foreach (Character i in nearbyTraders.ToList())
		{
			if ((trader == null || Position.DistanceTo(i.Position) < shortestDistance))
			{
				if (!IsInstanceValid(i))
				{
					nearbyTraders.Remove(i);
					continue;
				}
				trader = i;
				shortestDistance = Position.DistanceTo(trader.Position);
			}
		}
		SetInteractive(trader);
	}

	private void createDebugInstance()
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
		AddChild(debugInstance);
		debugInstance.AddStat("Faction", this, "GetFaction", true);
		//debugInstance.AddStat("Hostile", this, "GetHostilesString", true);
		//debugInstance.AddStat("Aggressive", this, "aggressive", false);
		debugInstance.AddStat("Profession", this, "GetProfession", true);
		debugInstance.AddStat("Surrounding Resources", this, "GetSurroundingWorkableResourcesString", true);
		debugInstance.AddStat("Nearby Traders", this, "GetNearbyTradersString", true);
		debugInstance.AddStat("Needed Items", this, "GetNeededItemsString", true);
		// debugInstance.AddStat("Sold Items", this, "soldItems", false);
		//debugInstance.AddStat("Has Traded", this, "hasTraded", false);
		//debugInstance.AddStat("Out Of Work", this, "outOfWork", false);
		debugInstance.AddStat("Hunger", this, "GetHungerValue", true);
		debugInstance.AddStat("Commodities", this, "GetCommoditiesValue", true);
		debugInstance.AddStat("Current Health", this, "GetCurrentHealth", true);
	}

	public override void _Ready()
	{
		base._Ready();

		createDebugInstance(); // For debugging. Comment out to disable.

		// Instance the outOfWork timer
		outOfWorkTimer.OneShot = true;
		outOfWorkTimer.WaitTime = 15;
		outOfWorkTimer.Connect("timeout", this, "ToggleOutOfWork");
		AddChild(outOfWorkTimer);

	}
}