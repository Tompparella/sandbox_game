using Godot;
using System.Collections.Generic;

public class DisplayState : Panel
{
    private Label States;
    public override void _Ready()
    {
        States = (Label)GetNode("States");
    }
    public void _OnStateChanged(State CurrentState) {

        string stateNames = "";
        stateNames += CurrentState.GetType().ToString();
        States.Text = stateNames;
    }
}
