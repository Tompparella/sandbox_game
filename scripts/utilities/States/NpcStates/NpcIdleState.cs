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
        base.Enter();
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click")) {
			EmitSignal(nameof(Finished), "move");
		}
        //base.HandleInput(@event);
    }
}
