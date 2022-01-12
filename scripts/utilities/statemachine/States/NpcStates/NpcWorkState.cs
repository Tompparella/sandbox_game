using Godot;
using System;
using System.Linq;

public class NpcWorkState : NpcMoveState
{
    private float workRange = Constants.DEF_ATTACKRANGE;
    private float tickDelta = 0;
    private bool staggered = false;
    private Resources resource;

    public override void Enter() {
        resource = (Resources)owner.GetInteractive();
        if (resource != null && !resource.GetExhausted() && resource.AddWorker(owner)) {
            workRange = resource.workRange;
            owner.GetMovePath(owner.GlobalPosition, resource.Position, owner);
        } else {
            EmitSignal(nameof(Finished), "idle");
        }
    }

    public override void Exit()
    {
        resource?.RemoveWorker(owner); // Could make this into a signal.
        ((Npc)owner).hasTraded = false;
        owner.SetInteractive();
        base.Exit();
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
            GD.Print("Error in NpcWorkState: ", resource.Name); // Weird bug still exists. This should help with finding it.
            throw e;
        }
        base.Update(delta);
    }

    private void TickLoop(float delta) {
        tickDelta += delta;

        if (tickDelta >= owner.stats.workSpeed) {
            if (resource.GetExhausted()) {
                EmitSignal(nameof(Finished), "idle");
            }
            tickDelta = 0;
            staggered = false;
        }
    }

    private void WorkTarget() {
        resource.workAction(owner);
        if (owner.GetInteractive() == null) {
            //GD.Print("Exiting workstate");
            EmitSignal(nameof(Finished), "idle");
            return;
        }
        staggered = true;
    }

    protected override void MoveAlongPath(float delta)
    {
        if (!owner.movePath.Any())
        {
            //owner.Position = owner.movePath[0];
            owner.currentSpeed = 0;
            return;
        }
        float currentSpeed = owner.currentSpeed * delta;
        Vector2 startPoint = owner.Position;
        for (int i = 0; i < owner.movePath.Count(); i++)
        {
            float distanceToNext = startPoint.DistanceTo(owner.movePath[0]);
            float distanceToLast = startPoint.DistanceTo(owner.movePath.Last());
            if (currentSpeed <= distanceToNext && currentSpeed >= 0.0)
            {
                owner.Position = startPoint.LinearInterpolate(owner.movePath[0], currentSpeed / distanceToNext);
                break;
            }
            currentSpeed -= distanceToNext;
            startPoint = owner.movePath[0];
            owner.movePath.RemoveAt(0);
        }
    }
}