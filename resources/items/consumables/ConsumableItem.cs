using Godot;
using System;

public class ConsumableItem : Item
{
    public ConsumableItem(float _nutritionValue = 0, float _commodityValue = 0) {
        nutritionValue = _nutritionValue;
        commodityValue = _commodityValue;
        itemName = itemName.Equals("") ? (nutritionValue > 0 ? "Food" : commodityValue > 0 ? "Commodity" : "Consumable") : itemName;
    }
    public ConsumableItem() {
    }
    [Export]
    public float nutritionValue { get; set; } = 0;
    [Export]
    public float commodityValue { get; set; } = 0;
}
