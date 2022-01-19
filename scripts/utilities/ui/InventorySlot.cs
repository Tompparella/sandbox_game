using Godot;
using System;

public class InventorySlot : Panel {
    private InventoryDisplay inventory;
    private Tooltip tooltip;
    private PackedScene packedTooltip = (PackedScene)GD.Load("res://assets/tooltip.tscn");
    public override void _Ready()
    {
        inventory = (InventoryDisplay)GetParent();
        
        Connect("mouse_entered", this, nameof(_OnMouseOver));
        Connect("mouse_exited", this, nameof(_OnMouseExit));
        
        tooltip = (Tooltip)packedTooltip.Instance();
        AddChild(tooltip);
    }
    public void _OnMouseOver() {
        Item item = inventory.GetInventoryItem(GetIndex());
        if (item != null) {
            tooltip.RectPosition = GetGlobalMousePosition();
            tooltip.GetTooltip(item);
            tooltip.Popup_();
        }
    }
    public void _OnMouseExit() {
        tooltip.Hide();
    }
}