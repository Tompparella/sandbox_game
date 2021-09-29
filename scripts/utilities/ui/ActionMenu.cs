using Godot;
using System;

public class ActionMenu : Control
{
    [Signal]
    public delegate void CallAction(string nextStateName);
    private Popup popup;
    private Label name;
    private InterfaceManager interfaceManager;

    public override void _Ready()
    {
        popup = (Popup)GetNode("Popup");
        name = (Label)popup.GetNode("Name");
        interfaceManager = (InterfaceManager)GetParent();
    }

    public void ShowActionMenu(Character source) {
        UpdateActionMenu(source);
        popup.SetPosition(GetLocalMousePosition());
        popup.Popup_();
    }

    public void UpdateActionMenu(Character source) {
    if (source != null) {
        interfaceManager.AddPlayerTarget(source);
        name.Text = source.entityName;
    } else {
        name.Text = "";
    }
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
    }
    
    private void AttackCommand() {
        EmitSignal("CallAction", "battle");
        popup.Hide();
        GD.Print("Attack");
    }
    private void ExamineCommand() {
        popup.Hide();
        GD.Print("Examine");
    }
}
