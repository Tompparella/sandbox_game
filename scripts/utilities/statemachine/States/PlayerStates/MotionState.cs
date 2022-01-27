using Godot;
using System;

public class MotionState : State
{
    protected float agilityTickDelta = 0;
    protected float agilityTick = 5 * Constants.TICK;
    
    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("L-Click")) {
			EmitSignal(nameof(Finished), "move");
		}
    }

    public void UpdateLookDirection(Vector2 direction) {
        // Look direction handling here
    }
}