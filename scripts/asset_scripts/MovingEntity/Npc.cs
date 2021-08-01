using Godot;
using System;

public class Npc : Character
{

    public void _OnNpcClick(Viewport viewport, InputEvent @event, int shapeIndex) {
        if (@event.IsActionPressed("Click")) {
            EmitSignal("OnCharacterClick", this);
        }
    }
}
