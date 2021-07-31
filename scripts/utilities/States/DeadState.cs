using Godot;
using System;

public class DeadState : State
{
    
    public override void Enter() {
    }

    public override void _OnAnimationFinished(string animationName) {
        EmitSignal(nameof(Finished), "dead");
    }
}