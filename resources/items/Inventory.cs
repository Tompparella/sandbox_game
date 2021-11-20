using Godot;
using System.Collections.Generic;
using System.Linq;

public class Inventory : Resource
{
    [Export]
    public int currency;
    [Export]
    public List<Item> items = new List<Item> {
        null, null, null, null,
        null, null, null, null,
        null, null, null, null,
        null, null, null, null
    };
    public void AddItem(Item item, int amount = 1) {
        for (int i = 0; i < amount; i++) {
            int index = items.IndexOf(null);
            if (index != -1) {
                items[index] = item;
            }
        }
    }
    public void RemoveItem(Item item, int amount = 1) {
        for (int i = 0; i < amount; i++) {
            int index = items.IndexOf(item);
            if (index != -1) {
                items[index] = null;
            }
        }
    }

    public bool IsFull() {
        return !items.Any(x => x == null);
    }

    public bool IsEmpty() {
        return !items.Any(x => x != null);
    }

    public int GetFreeSpace() {
        return items.Where(x => x == null).Count();
    }

    public bool HasItems(Dictionary<Item,int> requiredItems) {
        
        foreach(KeyValuePair<Item, int> kvp in requiredItems) {
            int itemsInInventory = items.Where(x => x == kvp.Key).Count();
            GD.Print(string.Format("{0} in inventory: {1}\n{0} required: {2}", kvp.Key.itemName, itemsInInventory, kvp.Value));
            if (itemsInInventory < kvp.Value) {
                return false;
            }
        }

        return true;
    }

    public Item PopLastItem() {
        int index = items.IndexOf(items.Last(x => x != null));
        Item item = items[index];
        items[index] = null;
        return item;
    }
}
