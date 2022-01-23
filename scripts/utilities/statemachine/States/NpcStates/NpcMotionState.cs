using Godot;
using System;

public class NpcMotionState : State
{

    private float needsTickDelta = 0;
    private float needsTick = 5 * Constants.TICK;
    
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
        needsTickDelta += delta;

        if (needsTickDelta > needsTick) {
            needsTickDelta = 0;
            owner.stats.LowerHunger();
            if (owner.IsDead()) {
                EmitSignal(nameof(Finished), "dead");
            }
            owner.stats.LowerCommodities();
        }
    }

    public void UpdateLookDirection(Vector2 direction) {
        // Look direction handling here
    }
}
