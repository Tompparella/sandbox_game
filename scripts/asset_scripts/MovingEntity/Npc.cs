using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Npc : Character
{
	public List<Resources> surroundingResources { get; private set; } = new List<Resources>();
	public List<Npc> nearbyTraders { get; private set; } = new List<Npc>();
	public List<Item> soldItems { get; private set; } = new List<Item>();

	[Export]
	public string profession { get; private set; }

	public bool hasTraded = true;
	public bool outOfWork = true; // If there's nothing for the Npc to do, idle.

	public Timer outOfWorkTimer = new Timer();

	public void _OnSurroundingsEntered(Area2D area)
	{
		switch (profession)
		{
			case Constants.LUMBERJACK_PROFESSION:
				if (area is Lumber)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;

			case Constants.MINER_PROFESSION:
				if (area is Deposit)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;

			case Constants.FARMER_PROFESSION:
				if (area is WheatField)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;
			case Constants.TRADER_PROFESSION:
				if (area is TradeStall)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;
			case Constants.BAKER_PROFESSION:
				if (area is Oven)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;
			case Constants.CRAFTSMAN_PROFESSION:
				if (area is Woodcraft)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;
			case Constants.BLACKSMITH_PROFESSION:
				if (area is Blacksmith)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;
			case Constants.LOGISTICSOFFICER_PROFESSION:
				if (area is Barracks)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;

			case Constants.SOLDIER_PROFESSION:
				if (area is Guardpost)
				{
					outOfWork = false;
					surroundingResources.Add((Resources)area);
					area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
				}
				break;
			default:
				break;
		}
		if (area.IsInGroup(Constants.TRADER_GROUP))
		{
			
			nearbyTraders.Add((Npc)area);
		}
	}

	public bool WorkableResourcesExist()
	{
		return (surroundingResources.Any());
	}

	public void _OnSurroundingsExited(Area2D area)
	{
		if (surroundingResources.Contains(area))
		{
			if (area.IsConnected("OnRemoval", this, nameof(SurroundingRemoved)))
			{
				area.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
			}
			surroundingResources.Remove((Resources)area);
		}
	}
	private void SurroundingRemoved(Interactive resource)
	{
		if (!WorkableResourcesExist())
		{
			outOfWork = true;
		}
		resource.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
		surroundingResources.Remove((Resources)resource);
	}
	private void toggleOutOfWork()
	{
		GD.Print(String.Format("{0}: Started outOfWorkTimer", Name));

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
		bool foundWork = surroundingResources.Any();
		Resources currentResource = null;
		float shortestDistance = float.MaxValue;
		int leastWorkers = int.MaxValue;
		foreach (Resources i in surroundingResources.ToList())
		{
			if ((currentResource == null || i.GetWorkers().Count() <= leastWorkers))
			{
				if (!IsInstanceValid(i) || i.GetExhausted())
				{
					surroundingResources.Remove(i);
					continue;
				}
				if (Position.DistanceTo(i.Position) < shortestDistance && i.maxWorkers > i.GetWorkerNumber())
				{
					currentResource = i;
					shortestDistance = Position.DistanceTo(currentResource.Position);
					leastWorkers = i.GetWorkers().Count();
				}
			}
		}
		SetInteractive(currentResource);
		return foundWork;
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
			neededItems.Add(new ConsumableItem(_nutritionValue: 1));
		}
	}
	public void addCommoditiesToBuyQueue()
	{
		if (!neededItems.Any(x => x is ConsumableItem && ((ConsumableItem)x).commodityValue > 0))
		{
			neededItems.Add(new ConsumableItem(_commodityValue: 1));
		}
	}

	public void ClearFoodFromBuyQueue() {
		GD.Print("Npc '{}' cleared fooditems", entityName);
		neededItems.RemoveAll(x => x is ConsumableItem && ((ConsumableItem)x).nutritionValue > 0);
	}

	public void ClearCommoditiesFromBuyQueue() {
		GD.Print("Npc '{}' cleared commodities", entityName);
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
		return false;
	}

	private string GetNeededItemsString() {
		//GD.Print(string.Join("," ,neededItems.Select(x => x.itemName)));
		return string.Join(", " ,neededItems.Select(x => x.itemName));
	}

	private void SelectTrader()
	{
		Npc trader = null;
		float shortestDistance = float.MaxValue;
		foreach (Npc i in nearbyTraders)
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
		debugInstance.AddStat("Profession", this, "profession", false);
		debugInstance.AddStat("Surrounding Resources", this, "surroundingResources", false);
		// debugInstance.AddStat("Nearby Traders", this, "nearbyTraders", false);
		debugInstance.AddStat("Needed Items", this, "GetNeededItemsString", true);
		// debugInstance.AddStat("Sold Items", this, "soldItems", false);
		debugInstance.AddStat("Has Traded", this, "hasTraded", false);
		debugInstance.AddStat("Out Of Work", this, "outOfWork", false);
		debugInstance.AddStat("Hunger", this, "GetHungerValue", true);
		debugInstance.AddStat("Commodities", this, "GetCommoditiesValue", true);
		debugInstance.AddStat("Current Health", this, "GetCurrentHealth", true);
	}

	public override void _Ready()
	{
		if (profession == null)
		{
			Random r = new Random();
			int index = r.Next(Constants.PROFESSIONS.Count());
			profession = Constants.PROFESSIONS[index];
		}
		createDebugInstance(); // For debugging. Comment out to disable.

		// Instance the outOfWork timer
		outOfWorkTimer.OneShot = true;
		outOfWorkTimer.WaitTime = 15;
		outOfWorkTimer.Connect("timeout", this, "toggleOutOfWork");
		AddChild(outOfWorkTimer);

		base._Ready();
	}
}
