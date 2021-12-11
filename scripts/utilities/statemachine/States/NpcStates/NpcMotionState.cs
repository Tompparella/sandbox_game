using Godot;
using System;

public class NpcMotionState : State
{

    private float hungerTickDelta = 0;
    private float hungerTick = 5 * Constants.TICK;
    
    public override void HandleInput(InputEvent @event)
    {
        /*
        if (@event.IsActionPressed("Click")) {
			EmitSignal(nameof(Finished), "move");
		}
        */
    }

    public override void Update(float delta) {
        // Hunger handling
        hungerTickDelta += delta;

        if (hungerTickDelta > hungerTick) {
            hungerTickDelta = 0;
            owner.stats.lowerHunger();
        }
    }

    public void UpdateLookDirection(Vector2 direction) {
        // Look direction handling here
    }
}
