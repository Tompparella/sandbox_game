using Godot;
using System.Collections.Generic;

public class DisplayState : Panel
{
    private Label States;

    public void _OnStateChanged(State CurrentState) {
        string stateNames = "";
        stateNames += CurrentState.GetType().ToString();
        if (States == null) {
            States = (Label)GetNode("States");
        }
        States.Text = stateNames;
    }
}
