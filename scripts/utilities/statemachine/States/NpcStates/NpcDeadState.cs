using Godot;
using System;

public class NpcDeadState : State
{
    
    public override void Enter() {
        owner.isDead = true;
    }
    public override void HandleAttacked()
    {
    }
    
    public override void _OnAnimationFinished(string animationName) {
        //EmitSignal(nameof(Finished), "dead");
    }
}