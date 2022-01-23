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
        if (!npcOwner.WorkableResourcesExist()) {      // If the Npc is out of work, put a timer for x seconds to enter work state again.
            ((Npc)owner).outOfWork = true;
            npcOwner.outOfWorkTimer.Start();
        }
        ((Npc)owner).hasTraded = true;
        owner.SetInteractive();
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

    private void TradeInventory() {
        bool tradeSuccess = false;
        Trade tradeInstance = new Trade(owner.tradeInventory, owner.GetInteractive().tradeInventory); // Buyer = Owner, Seller = Trader
        Npc npcOwner = ((Npc)owner);
        tradeSuccess = SellProduceItems(tradeInstance);
        npcOwner.CheckNeeds();
        if (npcOwner.checkBuyQueue()) {
            bool buySuccess = BuyNeededItems(tradeInstance);
            tradeSuccess = tradeSuccess || buySuccess;
        }
        owner.SetInteractive();
        npcOwner.hasTraded = tradeSuccess; // At the moment this does nothing since hasTraded is put to true upon exit.
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
                    if (tradeInstance.BuyConsumableItem("commodity")) {
                        tradeSuccess = true;
                        ((Npc)owner).ClearCommoditiesFromBuyQueue();
                        continue;
                    }

                } else if (((ConsumableItem)item).nutritionValue > 0) {
                    if (tradeInstance.BuyConsumableItem("food")) {
                        tradeSuccess = true;
                        ((Npc)owner).ClearFoodFromBuyQueue();
                        continue;
                    }
                }
            }
            else if (tradeInstance.BuyItem(item)) {
                tradeSuccess = true;
                ((Npc)owner).PopFromBuyQueue(item);
                continue;
            }
        }
        return tradeSuccess;
    }
}