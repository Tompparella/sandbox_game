using Godot;
using System;

public class NpcDeadState : State
{
    private Timer queueFreeTimer = new Timer();
    public override void Enter() {
        owner.stats.isDead = true;
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