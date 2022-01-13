using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TradeStall : Resources
{
    private float traderProfit = 0.05f;

    public override void _Ready()
    {
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

        base._Ready();
    }

    public override bool AddWorker(Character worker) {      // Upon entering, make it so that the trader used the stall's inventory while trading.
        worker.AddToGroup(Constants.TRADER_GROUP);
        worker.tradeInventory = inventory;
        worker.Monitorable = false;                         // This is done because we want other entities to notice this worker as a trader.
        worker.Monitorable = true;
        return base.AddWorker(worker);
    }

    public override void RemoveWorker(Character worker) {   // Upon exiting, return default inventory as the traded inventory.
        worker.RemoveFromGroup(Constants.TRADER_GROUP);
        worker.tradeInventory = worker.inventory;
        worker.Monitorable = false;
        worker.Monitorable = true;
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
        if (worker.checkBuyQueue() && worker is Npc) {
            BuyTraderNeeds((Npc)worker);
        }
    }

    public override void ItemRemoved(Item soldItem)
    {
        /*
        This is called every time a new item is added to tradestall's inventory, ie. after every trade.
        The trader (worker) gets paid for each trade a sum depending on the profit made.
        !TODO! When settlements are done, make it so that the paid profit depends on the actual sold value
        of the item. Tax should be static.
        */
        if (workers.Any()) {
            float salary = soldItem.value * traderProfit;
            Trade trade = new Trade(inventory, workers.First().inventory);
            trade.TransferCurrency((int)salary);
            GD.Print(string.Format("A trader earned {0} currency from a trade.",(int)salary));
        }
    }

    private void BuyTraderNeeds(Npc worker) {                               // Modified version of buy function found in NpcTradeState. Could be improved.
        Trade tradeInstance = new Trade(worker.inventory, tradeInventory);  // Buyer = Owner, Seller = Trader
        //tradeSuccess = SellProduceItems(tradeInstance);                   // Traders don't produce anything.

        foreach (Item item in worker.GetBuyQueue().ToList())
        {
            if (item is ConsumableItem){
                if (((ConsumableItem)item).commodityValue > 0) {
                    if (tradeInstance.BuyConsumableItem("commodity")) {
                        worker.ClearCommoditiesFromBuyQueue();
                    }

                } else if (((ConsumableItem)item).nutritionValue > 0) {
                    if (tradeInstance.BuyConsumableItem("food")) {
                        worker.ClearFoodFromBuyQueue();
                    }
                }
            }
            else if (tradeInstance.BuyItem(item)) {
                worker.PopFromBuyQueue(item);
            }
        }
    }

/*
    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
*/
}