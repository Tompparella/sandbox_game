using Godot;
using System;

public class DeadState : State
{
    
    public override void Enter() {
        owner.stats.isDead = true;
    }

    public override void HandleAttack()
    {
    }

    public override void _OnAnimationFinished(string animationName) {
        EmitSignal(nameof(Finished), "dead");
    }
}