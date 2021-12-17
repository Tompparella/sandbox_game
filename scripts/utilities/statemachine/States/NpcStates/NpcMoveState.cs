using Godot;
using System.Collections.Generic;
using System.Linq;

public class NpcMoveState : NpcMotionState
{
    private RandomNumberGenerator rand = new RandomNumberGenerator();
    public override void Enter()
    {
        if (!owner.movePath.Any())
        {
            rand.Randomize();
            try
            {
                int radius = 300;
                owner.GetMovePath(owner.GlobalPosition, owner.GlobalPosition + new Vector2(rand.RandiRange(-radius, radius), rand.RandiRange(-radius, radius)), owner);
            }
            catch (System.Exception e)
            {
                GD.Print("Crash on random movement.");
                throw e;
            }
            if (owner.movePath == null)
            {
                EmitSignal(nameof(Finished), "idle");
            }
        }
    }

    public override void Update(float delta)
    {
        // Movement handling here - if (!moving) { EmitSignal(nameof(Finished), "idle")}
        MovementLoop(delta);

        base.Update(delta);
    }

    protected virtual void MovementLoop(float delta)
    {
        // owner.currentSpeed += owner.currentSpeed < owner.stats.moveSpeed ? owner.acceleration * delta : 0;
        owner.currentSpeed += owner.currentSpeed < owner.stats.moveSpeed ? owner.acceleration : 0;
        delta = 1 + delta;
        MoveAlongPath(delta);
    }

    protected void MoveAlongPath(float delta)
    {
        if (!owner.movePath.Any())
        {
            //owner.Position = owner.movePath[0];
            owner.currentSpeed = 0;
            EmitSignal(nameof(Finished), "idle");
            return;
        }
        float currentSpeed = owner.currentSpeed;
        Vector2 startPoint = owner.Position;
        for (int i = 0; i < owner.movePath.Count(); i++)
        {
            float distanceToNext = startPoint.DistanceTo(owner.movePath[0]);
            float distanceToLast = startPoint.DistanceTo(owner.movePath.Last());
            if (currentSpeed <= distanceToNext && currentSpeed >= 0.0)
            {
                owner.Position = startPoint.LinearInterpolate(owner.movePath[0], (currentSpeed / distanceToNext) * delta);
                break;
            }
            currentSpeed -= distanceToNext;
            startPoint = owner.movePath[0];
            owner.movePath.RemoveAt(0);
        }
    }
}
