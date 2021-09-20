using Godot;
using System;

public class MotionState : State
{
    
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