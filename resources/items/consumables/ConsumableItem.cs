using Godot;
using System;

public class ConsumableItem : Item
{
    public ConsumableItem(float _nutritionValue = 0, float _commodityValue = 0) {
        nutritionValue = _nutritionValue;
        commodityValue = _commodityValue;
    }
    public ConsumableItem() {
    }
    [Export]
    public float nutritionValue { get; private set; } = 0;
    [Export]
    public float commodityValue { get; private set; } = 0;
}
