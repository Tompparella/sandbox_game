using Godot;
using System.Collections.Generic;
using System.Linq;

public class NpcTradeState : NpcMoveState
{
    public override void Enter()
    {
        owner.GetMovePath(owner.Position, owner.GetInteractive().Position, owner);
        base.Enter();
    }

    public override void Exit()
    {
        Npc npcOwner = ((Npc)owner);
        npcOwner.hasTraded = true;
        if (npcOwner.IsInGroup(Constants.CARAVAN_GROUP)) {
            npcOwner.hasTraded = false;
        }
        else if (!npcOwner.WorkableResourcesExist()) {      // If the Npc is out of work, put a timer for x seconds to enter work state again.
            ((Npc)owner).outOfWork = true;
            npcOwner.outOfWorkTimer.Start();
        }
        base.Exit();
    }
    
    protected override void MovementLoop(float delta)
    {
        float distanceToLast = owner.Position.DistanceTo(owner.movePath.LastOrDefault());
        if (distanceToLast > Constants.DEF_ATTACKRANGE) {
            base.MovementLoop(delta);
        } else {
            TradeInventory();
        }
    }

    public void TradeInventory() {
        bool tradeSuccess = false;
        Trade tradeInstance = new Trade(owner.tradeInventory, owner.GetInteractive().tradeInventory); // Buyer = Owner, Seller = Trader
        Npc npcOwner = owner as Npc;
        /*
        Handling for caravans has to be done separately, since they're optimally always on the move.
        */
        if (owner.IsInGroup(Constants.CARAVAN_GROUP)) {
            tradeInstance.HandleCaravanTrade();
            if (!owner.tradeInventory.IsEmpty()) {
                owner.EmitSignal("GetProfitableTrader", owner);
            } else {
                owner.EmitSignal("ReturnCaravanToDepot", owner);
            }

        } else {
            tradeSuccess = SellProduceItems(tradeInstance);
            npcOwner.CheckNeeds();
            if (npcOwner.checkBuyQueue()) {
                bool buySuccess = BuyNeededItems(tradeInstance);
                tradeSuccess = tradeSuccess || buySuccess;
            }
            //npcOwner.hasTraded = tradeSuccess; // At the moment this does nothing since hasTraded is put to true upon exit.
        }
        owner.SetInteractive();
        EmitSignal(nameof(Finished), "idle");
    }

    private bool SellProduceItems(Trade tradeInstance) {
        bool tradeSuccess = false;
        List<Item> sellQueue = owner.GetSellQueue().ToList();
        foreach (Item item in sellQueue) {
            if (tradeInstance.SellItem(item)) {
                owner.PopFromSellQueue(item);
                tradeSuccess = true;
            }
        }
        return tradeSuccess;
    }

    private bool BuyNeededItems(Trade tradeInstance) {
        bool tradeSuccess = false;
        foreach (Item item in ((Npc)owner).GetBuyQueue().ToList())
        {
            if (item is ConsumableItem){
                if (((ConsumableItem)item).commodityValue > 0) {
                    if (tradeInstance.BuyConsumableItem(Constants.DEF_COMMODITYNAME)) {
                        tradeSuccess = true;
                        owner.EmitSignal("OnWantFulfilled", Constants.DEF_COMMODITYNAME, 1);
                        ((Npc)owner).ClearCommoditiesFromBuyQueue();
                        continue;
                    }

                } else if (((ConsumableItem)item).nutritionValue > 0) {
                    if (tradeInstance.BuyConsumableItem(Constants.DEF_FOODNAME)) {
                        tradeSuccess = true;
                        owner.EmitSignal("OnWantFulfilled", Constants.DEF_FOODNAME, 1);
                        ((Npc)owner).ClearFoodFromBuyQueue();
                        continue;
                    }
                }
            }
            else if (tradeInstance.BuyItem(item)) {
                tradeSuccess = true;
                owner.EmitSignal("OnWantFulfilled", item.itemName, 1);
                ((Npc)owner).PopFromBuyQueue(item);
                continue;
            }
        }
        return tradeSuccess;
    }
}