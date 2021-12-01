using Godot;
using System.Collections.Generic;
using System.Linq;

public class NpcTradeState : NpcMoveState
{
    private Timer timer = new Timer();
    public override void _Ready()
    {
        timer.OneShot = true;
        timer.WaitTime = 10;
        timer.Connect("timeout", this, "toggleOutOfWork");
        AddChild(timer);
        base._Ready();
    }

    public override void Enter()
    {
        owner.GetMovePath(owner.Position, owner.GetInteractive().Position, owner);
        base.Enter();
    }

    public override void Exit()
    {
        Npc npcOwner = ((Npc)owner);
        if (!npcOwner.hasTraded && !npcOwner.WorkableResourcesExist()) {      // If the Npc is out of work, put a timer for 1 minutes to enter work state again.
            ((Npc)owner).outOfWork = true;
            timer.Start();
        }
        ((Npc)owner).hasTraded = true;
        owner.SetInteractive();
        base.Exit();
    }

    private void toggleOutOfWork() {
        ((Npc)owner).outOfWork = false;
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
        Trade tradeInstance = new Trade(owner.inventory, owner.GetInteractive().inventory);
        tradeSuccess = SellProduceItems(tradeInstance);
        if (((Npc)owner).checkBuyQueue()) {
            bool buySuccess = BuyNeededItems(tradeInstance);
            tradeSuccess = tradeSuccess || buySuccess;
        }
        owner.SetInteractive();
        ((Npc)owner).hasTraded = tradeSuccess;
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
        foreach (Item item in ((Npc)owner).GetBuyQueue())
        {
            if (tradeInstance.BuyItem(item)) {
                tradeSuccess = true;
            }
        }
        return tradeSuccess;
    }
}