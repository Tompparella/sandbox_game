using Godot;
using System;

public class State : Node
{

    protected Character owner;
    
    [Signal]
    public delegate void Finished(string nextStateName);

    public override void _Ready()
    {
        owner = (Character)Owner;
    }

    public virtual void Enter() {
        return;
    }
    public virtual void Exit() {
        return;
    }
    public virtual void HandleInput(InputEvent @event) {
        return;
    }
    public virtual void Update(float delta) {
        return;
    }
    public virtual void _OnAnimationFinished(string animationName) {
        return;
    }


}
