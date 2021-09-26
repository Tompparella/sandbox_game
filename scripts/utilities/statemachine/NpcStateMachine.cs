using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class NpcStateMachine : StateMachine
{
    private string[] prioritizedStates = { "battle" };

    public override void _Ready() {
        Character parent = (Npc)GetParent();
        statesMap = new Dictionary<string, State> {
            { "idle", (State)GetNode("Idle")},
            { "move", (State)GetNode("Move")},
            { "battle", (State)GetNode("Battle")},
            { "dead", (State)GetNode("Dead")},
            { "work", (State)GetNode("Work")}
        };
        base._Ready();
    }
    /*
    public override void _UnhandledInput(InputEvent @event) {
        //Mainly for testing purposes.
        //CurrentState.HandleInput(@event);
    }
    */

    public virtual void _OnNpcClick(Node viewPort, InputEvent @event, int shapeIndex) {
        CurrentState.HandleInput(@event);
    }

    protected override void _ChangeState(string stateName) {

        if (!_active) { return; }

        if (prioritizedStates.Contains(stateName)) {
            stateStack.Insert(0, statesMap[stateName]);
        }
        base._ChangeState(stateName);
    }
}
