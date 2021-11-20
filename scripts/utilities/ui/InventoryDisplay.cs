using Godot;
using System;

public class InventoryDisplay : GridContainer {
    private Inventory currentInventory = new Inventory();

    public void UpdateInventory(Inventory inventory) {
        currentInventory = inventory;
        for (int i = 0; i < inventory.items.Count; i++) {
            UpdateSlotTexture(i);
        }
    }
    public Item GetInventoryItem(int index) {
        return currentInventory.items[index];
    }
    private void UpdateSlotTexture(int index) {
        Panel slot = (Panel)GetChild(index);
        TextureRect texture = (TextureRect)slot.GetChild(0);
        string modulateColor;
        switch (currentInventory.items[index]?.GetType().ToString())
        {
            case "ResourceItem":
                modulateColor = Constants.RESOURCECOLOR;
                break;
            case "ConsumableItem":
                modulateColor = Constants.CONSUMABLECOLOR;
                break;
            default:
                modulateColor = Constants.EMPTYCOLOR;
                break;
        }
        slot.SelfModulate = new Color(modulateColor);
        texture.Texture = currentInventory.items[index]?.texture ?? null;
    }
    public void ClearItems() {
        currentInventory = new Inventory();
        UpdateInventory(currentInventory);
    }
}