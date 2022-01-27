using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TradeStall : Resources
{
    [Signal]
    public delegate void OnItemSold(Item item);
    [Signal]
    public delegate void OnItemBought(Item item);
    [Signal]
    public delegate void BeginTradeMission(TradeStall tradeStall);
    private float traderProfit = 0.05f;
    private float priceChangeStep = 0.025f;

    public override void _Ready()
    {
        workerProfession = Constants.TRADER_PROFESSION;
        actions = Constants.TRADEACTIONS;
        dialogue = new ResourceDialogue(this, Constants.TRADESTALL_DESCRIPTION, actions);

        maxWorkers = 1; // Only one trader per tradestall

        defaultTexture = Constants.OVEN_TEXTURE;
        defaultPortrait = Constants.OVEN_PORTRAIT;
        defaultName = Constants.OVEN_NAME;
        defaultDescription = Constants.TRADESTALL_DESCRIPTION;

        exhaustedDescription = Constants.TRADESTALL_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        createDebugInstance();

        base._Ready();
    }

    public override bool AddWorker(Character worker) {      // Upon entering, make it so that the trader used the stall's inventory while trading.
        if (!worker.IsInGroup(Constants.TRADER_GROUP)) {
            worker.AddToGroup(Constants.TRADER_GROUP);
            worker.EmitSignal("Refresh", worker);
        }
        worker.tradeInventory = inventory;
        return base.AddWorker(worker);
    }

    public override void RemoveWorker(Character worker) {   // Upon exiting, return default inventory as the traded inventory.
        if (worker.IsInGroup(Constants.TRADER_GROUP)) {
            worker.RemoveFromGroup(Constants.TRADER_GROUP);
            worker.EmitSignal("Refresh", worker);
        }
        worker.tradeInventory = worker.inventory;
        base.RemoveWorker(worker);
    }

    public override void GiveResource(Character worker) {
        if (!isExhausted) {
            worker.inventory.currency++;
        }
        /*
        Since the trader will continuously be in WorkState, check their needs here and buy what they need if needed.
        */
        worker.CheckNeeds();
        if (worker.checkBuyQueue() && worker is Npc npcWorker) {
            BuyTraderNeeds(npcWorker);
        }
    }

    public override void ItemRemoved(Item soldItem)
    {
        /*
        This is called every time a new item is removed from tradestall's inventory, ie. after every item sale.
        The trader (worker) gets paid for each trade a sum depending on the profit made.
        !TODO! When settlements are done, make it so that the paid profit depends on the actual sold value
        of the item. Tax should be static.
        */
        if (workers.Any()) {
            Trade trade = new Trade(inventory, workers.First().inventory);
            trade.TransferCurrency(GetTraderCompensation(soldItem));
            GD.Print(string.Format("A trader earned {0} currency from a trade.", GetTraderCompensation(soldItem)));
        }
        UpdatePrice(soldItem, true);
        EmitSignal(nameof(OnItemSold), soldItem);
    }
    public override void CheckNeeds(Item item)  // This runs when an item is added to inventory, ie. when the trader buys an item. 
    {
        UpdatePrice(item, false);
        EmitSignal(nameof(OnItemBought), item);
        if (inventory.IsFull()) {
            OnInventoryFull();
        }
    }
    private void OnInventoryFull() {
        EmitSignal(nameof(BeginTradeMission), this);
    }
    private void UpdatePrices(Dictionary<string,int> itemDemand) {
        if (tradeInventory.IsFull()) {
            EmitSignal(nameof(BeginTradeMission), this);
        }
        foreach (KeyValuePair<string,int> kvp in itemDemand)
        {
            if (inventory.itemPriceModifiers.ContainsKey(kvp.Key)) {
                inventory.itemPriceModifiers[kvp.Key] = kvp.Value * priceChangeStep;
            } else {
                inventory.itemPriceModifiers.Add(kvp.Key, kvp.Value * priceChangeStep);
            }
        }
    }
    private void UpdatePrice(Item item, bool isSale) {    // Updates the price of an item locally to avoid situations where large amounts of items are either sold for too high or -low values. Bool isSale means that the change in inventory was a sale. Otherwise it was a buy.
        string itemName = item is ConsumableItem cItem ? (cItem.nutritionValue > 0 ? Constants.DEF_FOODNAME : cItem.commodityValue > 0 ? Constants.DEF_COMMODITYNAME : Constants.DEF_CONSUMABLENAME) : item.itemName;
        if (inventory.itemPriceModifiers.ContainsKey(itemName)) {
            inventory.itemPriceModifiers[itemName] += isSale ? priceChangeStep : -priceChangeStep ;
        } else {
            inventory.itemPriceModifiers.Add(itemName, 0);
        }
    }
    private int GetTraderCompensation(Item soldItem) {
        return soldItem.value * traderProfit >= 1 ? (int)(soldItem.value * traderProfit) : 1;   // Minimum profit per trade for the trader is 1 currency.
    }

    private void BuyTraderNeeds(Npc worker) {                               // Modified version of buy function found in NpcTradeState. Could be improved.
        Trade tradeInstance = new Trade(worker.inventory, tradeInventory);  // Buyer = Owner, Seller = Trader
        //tradeSuccess = SellProduceItems(tradeInstance);                   // Traders don't produce anything.

        foreach (Item item in worker.GetBuyQueue().ToList())
        {
            if (item is ConsumableItem){
                if (((ConsumableItem)item).commodityValue > 0) {
                    if (tradeInstance.BuyConsumableItem(Constants.DEF_COMMODITYNAME)) {
                        worker.ClearCommoditiesFromBuyQueue();
                    }

                } else if (((ConsumableItem)item).nutritionValue > 0) {
                    if (tradeInstance.BuyConsumableItem(Constants.DEF_FOODNAME)) {
                        worker.ClearFoodFromBuyQueue();
                    }
                }
            }
            else if (tradeInstance.BuyItem(item)) {
                worker.PopFromBuyQueue(item);
            }
        }
    }

    
    private string GetItemPriceModifiersString() {
        string result = "\n";
        foreach (KeyValuePair<string,float> kvp in inventory.itemPriceModifiers)
        {
            result += string.Format("{0}: {1} x\n", kvp.Key, kvp.Value);
        }
        return result;
    }
    private void createDebugInstance()
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
        debugInstance.RectPosition += new Vector2(200, 0);  // This makes debug visible even if the trader is on top of the tradestall.
		AddChild(debugInstance);
		debugInstance.AddStat("Item Price Modifiers", this, "GetItemPriceModifiersString", true);
	}

/*
    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
*/
}