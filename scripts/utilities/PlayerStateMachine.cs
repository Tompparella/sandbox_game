using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class PlayerStateMachine : StateMachine
{
    private string[] prioritizedStates = { "battle" };

    public override void _Ready() {
        Character parent = (Player)GetParent();
        statesMap = new Dictionary<string, State> {
            { "idle", (State)GetNode("Idle")},
            { "move", (State)GetNode("Move")},
            { "battle", (State)GetNode("Battle")},
            { "dead", (State)GetNode("Dead")}
        };
        base._Ready();
    }

    public override void _Input(InputEvent @event) {
        // Put only input that can interrupt other states here.
        CurrentState.HandleInput(@event);
    }

    protected override void _ChangeState(string stateName) {

        if (!_active) { return; }

        if (prioritizedStates.Contains(stateName)) {
            stateStack.Insert(0, statesMap[stateName]);
        }
        /* Tähän tilakohtainen kohtelu, esim:
        if (stateName == 'jump') {
            ...jotakin
        }
        */
        base._ChangeState(stateName);
    }
}
