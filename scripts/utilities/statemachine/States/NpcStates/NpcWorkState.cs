using Godot;
using System;
using System.Linq;

public class NpcWorkState : NpcMoveState
{
    private float workRange = Constants.DEF_ATTACKRANGE;
    private float workspeed = Constants.DEF_ATTACKSPEED; // Placeholder. Take labour level to note.
    private const float tickSpeed = Constants.TICK;
    private float tickDelta = 0;
    private int ticks = 0;
    private bool staggered = false;
    private Resources resource;

    public override void Enter() {
        resource = (Resources)owner.GetInteractive();
        if (resource.inventory.IsEmpty()) {
            EmitSignal(nameof(Finished), "idle");
        }
        owner.GetMovePath(owner.GlobalPosition, resource.Position, owner);
    }

    public override void Update(float delta)
    {
        try
        {
            if (staggered) {
                TickLoop(delta);
            } else if (owner.Position.DistanceTo(resource.Position) < workRange) {
                WorkTarget();
                return;
            }
        }
        catch (System.Exception e)
        {
            GD.Print("Error:", e); // Weird bug still exists. This should help with finding it.
            throw;
        }
        base.Update(delta);
    }

    private void TickLoop(float delta) {
        tickDelta += delta;
        if (tickDelta >= tickSpeed) {
            ticks++;
            tickDelta = 0;
        }
        if (ticks >= workspeed) {
            tickDelta = 0;
            ticks = 0;
            staggered = false;
        }
    }

    private void WorkTarget() {
        resource.workAction(owner);
        if (owner.GetInteractive() == null) {
            EmitSignal("Finished", "idle");
            return;
        }
        staggered = true;
    }

    protected override void MovementLoop(float delta)
    {
        float distanceToLast = owner.Position.DistanceTo(owner.movePath.Last());
        if (distanceToLast > workRange) {
            base.MovementLoop(delta);
        }
    }
}