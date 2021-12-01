using Godot;
using System;

public class NpcIdleState : NpcMotionState
{
    private Timer timer = new Timer();
    
    public override void _Ready()
    {
        timer.OneShot = true;
        timer.WaitTime = 3;
        timer.Connect("timeout", this, "MoveAtRandom");
        AddChild(timer);
        base._Ready();
    }

    public override void Enter()
    {
        Npc npcOwner = (Npc)owner;
        if (!npcOwner.outOfWork) {
            if (npcOwner.GetTrader()) {
                EmitSignal(nameof(Finished), "trade");
                return;
            } else if (npcOwner.GetNextWork()) {
                EmitSignal(nameof(Finished), "work");
                return;
            }
        }
        timer.Start();
    }

    public void MoveAtRandom() {
        EmitSignal(nameof(Finished), "move");
    }

    public override void HandleInput(InputEvent @event)
    {
    /*
        if (@event.IsActionPressed("L-Click")) {
			EmitSignal(nameof(Finished), "move");
		}
    */
        //base.HandleInput(@event);
    }
}
