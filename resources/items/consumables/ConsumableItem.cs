using Godot;
using System;

public class ConsumableItem : Item
{
    public ConsumableItem(float _nutritionValue = 0) {
        nutritionValue = _nutritionValue;
    }
    public ConsumableItem() {
    }
    [Export]
    public float nutritionValue { get; private set; } = 0;
}
