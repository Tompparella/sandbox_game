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
    public void AddItem(Item item) {
        int index = items.IndexOf(null);
        if (index != -1) {
            items[index] = item;
        }
    }
    public void RemoveItem(Item item) {
        int index = items.IndexOf(item);
        items[index] = null;
    }

    public bool IsFull() {
        return !items.Any(x => x == null);
    }

    public bool IsEmpty() {
        return !items.Any(x => x != null);
    }

    public Item PopLastItem() {
        int index = items.IndexOf(items.Last(x => x != null));
        Item item = items[index];
        items[index] = null;
        return item;
    }
}
