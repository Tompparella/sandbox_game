using Godot;
using System;

public class NpcIdleState : NpcMotionState
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void Enter()
    {
        Npc npcOwner = (Npc)owner;
        if (npcOwner.GetNextWork()) {
            EmitSignal(nameof(Finished), "work");
        } else {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(MoveAtRandom);   // If there's nothing to do, wait five seconds and look for a path.
            timer.Interval = 3000;
            timer.AutoReset = false;
            timer.Start();
        }
        base.Enter();
    }

    public void MoveAtRandom(object source, System.Timers.ElapsedEventArgs e) {
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
