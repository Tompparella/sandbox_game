using Godot;
using System;
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
            SellResourceItems();
        }
    }

    private void SellResourceItems() {
        Trade tradeInstance = new Trade(owner.inventory, owner.GetInteractive().inventory);
        owner.inventory.items.Where(x => x is ResourceItem).ToList().ForEach(x => tradeInstance.SellItem(x));
        owner.SetInteractive();
        EmitSignal(nameof(Finished), "idle");
    }
}