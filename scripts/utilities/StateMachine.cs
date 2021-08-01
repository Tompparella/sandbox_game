using Godot;
using System;
using System.Collections.Generic;

public class StateMachine : Node
{

    [Signal]
    public delegate void StateChanged(State currentState);

    protected State StartState;
    protected List<State> stateStack;
    protected Dictionary<string, State> statesMap { set; get; }
    protected State CurrentState;
    protected bool _active = false;

    public override void _EnterTree()
    {
        StartState = (State)GetNode("Idle");
        stateStack = new List<State>();
    }
    public override void _Ready()
    {
        foreach (State state in statesMap.Values) {
            state.Connect("Finished", this, "_ChangeState");
        }
        Initialize(StartState);
    }

    private void Initialize(State startState) {
        SetActive(true);
        stateStack.Insert(0, StartState);
        CurrentState = stateStack[0];
        CurrentState.Enter();
    }

    private void SetActive(bool value) {
        _active = value;
        SetProcess(value);
        SetProcessInput(value);
        if (!_active) {
            stateStack.Clear();
            CurrentState = null;
        }
    }

    public override void _UnhandledInput(InputEvent @event) {
        CurrentState.HandleInput(@event);
    }

    public override void _Process(float delta) {
        CurrentState.Update(delta);
    }

    protected virtual void _OnAnimationFinished(string animationName) {
        if (!_active) { return; }
        CurrentState._OnAnimationFinished(animationName);
    }

    protected virtual void _ChangeState(string stateName) {

        if (!_active) { return; }

        CurrentState.Exit();

        if (stateName == "previous") { // States can be added here as needed.
                //State popState = stateStack[0]; // In GDScript this is just pop_front, so I initially save the item to a variable.
                stateStack.RemoveAt(0);
        } else {
                stateStack[0] = statesMap[stateName];
        }
        /* Tähän tilakohtainen käsittely, esim:
        if (stateName == 'jump') {
            ...jotakin
        }
        */
        CurrentState = stateStack[0];
        EmitSignal(nameof(StateChanged), CurrentState);

        if (stateName != "previous") {
            CurrentState.Enter();
        }
    }
}