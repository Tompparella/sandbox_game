using Godot;
using System;
using System.Linq;

public class NpcWorkState : NpcMoveState
{
    private float workRange = Constants.DEF_ATTACKRANGE;
    private const float tickSpeed = Constants.TICK;
    private float tickDelta = 0;
    private bool staggered = false;
    private Resources resource;

    public override void Enter() {
        resource = (Resources)owner.GetInteractive();
        if (resource.GetExhausted()) {
            EmitSignal(nameof(Finished), "idle");
        }
        owner.GetMovePath(owner.GlobalPosition, resource.Position, owner);
    }

    public override void Exit()
    {
        if (!((Npc)owner).hasTraded) {      // If the Npc is out of work, put a timer for 1 minutes to enter work state again.
            ((Npc)owner).outOfWork = true;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(toggleOutOfWork);
            timer.Interval = 60000;
            timer.AutoReset = false;
            timer.Start();    
        }
        ((Npc)owner).hasTraded = false;
        owner.SetInteractive();
        base.Exit();
    }

    private void toggleOutOfWork(object source, System.Timers.ElapsedEventArgs e) {
        ((Npc)owner).outOfWork = false;
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
            GD.Print("Exiting workstate");
            EmitSignal(nameof(Finished), "idle");
            return;
        }
        staggered = true;
    }

    protected override void MovementLoop(float delta)
    {
        float distanceToLast = owner.Position.DistanceTo(owner.movePath.LastOrDefault());
        if (distanceToLast > workRange) {
            base.MovementLoop(delta);
        }
    }
}