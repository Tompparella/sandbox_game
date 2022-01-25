using Godot;
using System.Collections.Generic;
using System.Linq;

public class Inventory : Resource
{
    [Signal]
    public delegate void OnItemAdd(Item item);
    [Signal]
    public delegate void OnItemRemoved(Item item);
    [Export]
    public int currency = 0;
    [Export]
    public List<Item> items = new List<Item> {
        null, null, null, null,
        null, null, null, null,
        null, null, null, null,
        null, null, null, null
    };
    public Dictionary<string, float> itemPriceModifiers = new Dictionary<string, float>(); // Modifier that evaluates based on supply and demand. Mainly only used on traders.

    public void AddItem(Item item, int amount = 1) {
        for (int i = 0; i < amount; i++) {
            int index = items.IndexOf(null);
            if (index != -1) {
                items[index] = item;
            }
        }
        EmitSignal(nameof(OnItemAdd), item);
    }
    public void RemoveItem(Item item, int amount = 1) {
        for (int i = 0; i < amount; i++) {
            int index = items.IndexOf(item);
            if (index != -1) {
                items[index] = null;
            }
            EmitSignal(nameof(OnItemRemoved), item);
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
    /// <summary> Returns inventory items without empty (null) values. If empty, returns null. </summary>
    public IEnumerable<Item> GetFilteredItems() {
        if (items.Any()) {
            return items.Where(x => x != null);
        }
        return null;
    }

    /// <summary> Returns a boolean on whether inventory contains a desired number of items in dictionary. </summary>
    public bool HasItems(Dictionary<Item,int> requiredItems) {
        
        foreach(KeyValuePair<Item, int> kvp in requiredItems) {
            int itemsInInventory = items.Where(x => x == kvp.Key).Count();
            //GD.Print(string.Format("{0} in inventory: {1}\n{0} required: {2}", kvp.Key.itemName, itemsInInventory, kvp.Value));
            if (itemsInInventory < kvp.Value) {
                return false;
            }
        }
        return true;
    }

    /// <summary> Returns a boolean on whether inventory contains at least x number of a selected item. </summary>
    public bool HasItem(Item item, int amount = 1) {
        //GD.Print(items.Where(x => x != null && x.itemName.Equals(item.itemName))?.Count());
        return items.Where(x => x != null && x.itemName.Equals(item.itemName))?.Count() >= amount;
    }

    public List<ConsumableItem> GetEdibleItems() {
        if (items.Any(x => x is ConsumableItem && ((ConsumableItem)x).nutritionValue > 0)) {
            return items.Where(x => x is ConsumableItem && ((ConsumableItem)x).nutritionValue > 0).DefaultIfEmpty().Cast<ConsumableItem>().OrderBy(x => x.nutritionValue).ToList();
        }
        return null;
    }

    public int GetEdibleItemCount() {
        return GetEdibleItems() != null ? GetEdibleItems().Count() : 0;
    }

    public List<ConsumableItem> GetCommodityItems() {
        if (items.Any(x => x is ConsumableItem && ((ConsumableItem)x).commodityValue > 0)) {
            return items.Where(x => x is ConsumableItem && ((ConsumableItem)x).commodityValue > 0).DefaultIfEmpty().Cast<ConsumableItem>().OrderBy(x => x.commodityValue).ToList();
        }    
        return null;
    }

    public int GetCommodityItemCount() {
        return GetCommodityItems() != null ? GetCommodityItems().Count() : 0;
    }

    public float GetItemPriceModifier(Item item) {
        string itemName = item is ConsumableItem cItem ? (cItem.nutritionValue > 0 ? Constants.DEF_FOODNAME : cItem.commodityValue > 0 ? Constants.DEF_COMMODITYNAME : Constants.DEF_CONSUMABLENAME) : item.itemName;
        if (itemPriceModifiers.ContainsKey(itemName)) {
            return itemPriceModifiers[itemName];
        }
        return 0;
    }
    public Item PopLastItem() {
        int index = items.IndexOf(items.Last(x => x != null));
        Item item = items[index];
        items[index] = null;
        return item;
    }
}
