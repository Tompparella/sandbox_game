using Godot;
using System;

public class NpcDeadState : State
{
    private Timer queueFreeTimer = new Timer();
    public override void Enter() {
        GD.Print(string.Format("{0} has died", owner.entityName));
        owner.stats.isDead = true;
        owner.EmitSignal("Dead", owner);
        queueFreeTimer.OneShot = true;
		queueFreeTimer.WaitTime = 5;
		queueFreeTimer.Connect("timeout", this, "RemoveNpc");
		AddChild(queueFreeTimer);
        queueFreeTimer.Start();
    }
    private void RemoveNpc() {
        GD.Print(string.Format("Removing dead Npc: {0}", owner.entityName));
        owner.QueueFree();
    }
    public override void HandleAttack()
    {
    }
    
    public override void _OnAnimationFinished(string animationName) {
        //EmitSignal(nameof(Finished), "dead");
    }
}