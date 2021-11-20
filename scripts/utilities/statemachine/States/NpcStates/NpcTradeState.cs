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
        Trade tradeInstance = new Trade(owner.inventory, owner.GetInteractive().inventory);
        SellProduceItems(tradeInstance);
        if (((Npc)owner).checkBuyQueue()) {
            BuyNeededItems(tradeInstance);
        }
        owner.SetInteractive();
        EmitSignal(nameof(Finished), "idle");
    }

    private void SellProduceItems(Trade tradeInstance) {
        
        owner.GetSellQueue().ForEach(x => GD.Print(x.itemName));

        List<Item> sellQueue = owner.GetSellQueue().ToList();
        foreach (Item item in sellQueue) {
            if (tradeInstance.SellItem(item)) {
                owner.PopFromSellQueue(item);
            }
        }
    }

    private void BuyNeededItems(Trade tradeInstance) {
        ((Npc)owner).GetBuyQueue().ForEach(x => tradeInstance.BuyItem(x));
    }
}