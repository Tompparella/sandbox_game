using Godot;
using System;

public class ConsumableItem : Item
{
    public ConsumableItem(float _nutritionValue = 0, float _commodityValue = 0) {
        nutritionValue = _nutritionValue;
        commodityValue = _commodityValue;
        itemName = itemName.Equals("") ? (nutritionValue > 0 ? Constants.DEF_FOODNAME : commodityValue > 0 ? Constants.DEF_COMMODITYNAME : Constants.DEF_CONSUMABLENAME) : itemName;
    }
    public ConsumableItem() {
    }
    [Export]
    public float nutritionValue { get; set; } = 0;
    [Export]
    public float commodityValue { get; set; } = 0;
    [Export]
    public float healValue { get; set; } = 0;
}
