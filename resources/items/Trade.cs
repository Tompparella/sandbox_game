using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary> Used for handling trade instances between inventories. T.ex. trading items for currency, transferring currency, etc. </summary>
public class Trade
{
    private Inventory buyer, seller;
    private float tradeProfit = Constants.DEF_TRADEPROFIT;

    public Trade(Inventory _buyer, Inventory _seller) {
        buyer = _buyer;
        seller = _seller;
    }

    public bool BuyItem(Item item) {
        if (seller.GetFilteredItems().Contains(item)) {
            int value = (int)Math.Round((item.value * (1 + tradeProfit + seller.GetItemPriceModifier(item))), 0); // Trader gets a certain profit margin from each trade that's determined by the 'tradeProfit' modifier.
            if (buyer.currency >= value && !buyer.IsFull()) {
                seller.currency += value;
                buyer.currency -= value;
                seller.RemoveItem(item);
                buyer.AddItem(item);
                return true;
            }
        } else {
            //GD.Print(string.Format("Buying '{1}': No item or currency in inventory.", item.itemName));
        }
        return false;
    }

    public bool BuyConsumableItem(string itemType) {
        if (seller.GetFilteredItems().Any(x => x is ConsumableItem)) {
            ConsumableItem itemToBuy;
            switch (itemType)
            {
                case "Food":        // Same as def_foodname in constants
                    itemToBuy = seller.GetFilteredItems().Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.nutritionValue).Last();
                    if (itemToBuy.nutritionValue <= 0) {
                        return false;
                    }
                    break;
                case "Commodity":   // Same as def_commodityname in constants
                    itemToBuy = seller.GetFilteredItems().Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.commodityValue).Last();
                    if (itemToBuy.commodityValue <= 0) {
                        return false;
                    }
                    break;
                default:
                    //GD.Print(String.Format("Wrong item type when buying consumable item: {0}", itemType));
                    return false;
            }
            int value = (int)Math.Round((itemToBuy.value * (1 + tradeProfit + seller.GetItemPriceModifier(itemToBuy))), 0);
            if (buyer.currency >= value && !buyer.IsFull()) {
                seller.currency += value;
                buyer.currency -= value;
                seller.RemoveItem(itemToBuy);
                buyer.AddItem(itemToBuy);
                return true;
            }
        }
        return false;
    }

    private void BuyCheapItems(List<string> soldItems) {                    // We have to check if the trader has just sold a bunch of the same items as it's gonna buy.

        IEnumerable<Item>   filteredItems   =   seller.GetFilteredItems();  // A list of seller items that are not null.
        List<string>        cheapItems      =   new List<string>();

        foreach (KeyValuePair<string, float> kvp in seller.itemPriceModifiers)
        {
            if (kvp.Value < 0 && !soldItems.Contains(kvp.Key)) {
                cheapItems.Add(kvp.Key);
            }
        }
        if (!cheapItems.Any()) {
            return;
        }
        foreach (string itemName in cheapItems)
        {
            int itemCount;
            switch (itemName)
            {
                case "Food":
                    itemCount = filteredItems.Count(x => x is ConsumableItem cItem && cItem.nutritionValue > 0);
                    for (int i = 0; i < itemCount; i++) {
                    if (!BuyConsumableItem(itemName)) {
                        break;
                    }
                }
                    break;
                case "Commodity":
                    itemCount = filteredItems.Count(x => x is ConsumableItem cItem && cItem.commodityValue > 0);
                    for (int i = 0; i < itemCount; i++) {
                    if (!BuyConsumableItem(itemName)) {
                        break;
                    }
                }
                    break;
                default:
                    Item currentItem = filteredItems.FirstOrDefault(x => x.itemName.Equals(itemName));
                    if (currentItem == null) {
                        continue;
                    }
                    itemCount = filteredItems.Count(x => x.itemName.Equals(itemName));
                    for (int i = 0; i < itemCount; i++) {
                        if (!BuyItem(currentItem)) {
                            break;
                        }
                    }
                    break;
            }
        }
    }

    public bool SellItem(Item item) {
        if (buyer.GetFilteredItems().Contains(item)) {
            int value = (int)Math.Round((item.value * (1 - tradeProfit + seller.GetItemPriceModifier(item))), 0);
            if (seller.currency >= value && !seller.IsFull()) {
                buyer.currency += value;
                seller.currency -= value;
                buyer.RemoveItem(item);
                seller.AddItem(item);
                return true;
            }
        } else {
            //GD.Print(string.Format("Selling {0}: No item or currency in inventory.", item.itemName));
        }
        return false;
    }

    private bool SellConsumableItem(string itemType) {
        if (buyer.GetFilteredItems().Any(x => x is ConsumableItem)) {
            ConsumableItem itemToSell;
            switch (itemType)
            {
                case "Food":        // Same as def_foodname in constants
                    itemToSell = buyer.GetFilteredItems().Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.nutritionValue).Last();
                    if (itemToSell.nutritionValue <= 0) {
                        return false;
                    }
                    break;
                case "Commodity":   // Same as def_commodityname in constants
                    itemToSell = buyer.GetFilteredItems().Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.commodityValue).Last();
                    if (itemToSell.commodityValue <= 0) {
                        return false;
                    }
                    break;
                default:
                    //GD.Print(String.Format("Wrong item type when selling consumable item: {0}", itemType));
                    return false;
            }
            int value = (int)Math.Round((itemToSell.value * (1 + tradeProfit + seller.GetItemPriceModifier(itemToSell))), 0);
            if (seller.currency >= value && !seller.IsFull()) {
                buyer.currency += value;
                seller.currency -= value;
                buyer.RemoveItem(itemToSell);
                seller.AddItem(itemToSell);
                return true;
            }
        }
        return false;
    }

    private List<string> SellExpensiveItems() {

        IEnumerable<Item>   filteredItems   =   buyer.GetFilteredItems();   // A list of buyer items that are not null.
        List<string>        expensiveItems  =   new List<string>();
        List<string>        soldItems       =   new List<string>();

        foreach (KeyValuePair<string, float> kvp in seller.itemPriceModifiers)
        {
            if (kvp.Value > 0) {
                expensiveItems.Add(kvp.Key);
            }
        }
        if (!expensiveItems.Any()) {
            return soldItems;
        }
        foreach (string itemName in expensiveItems)
        {
            int itemCount;
            switch (itemName)
            {
                case "Food":
                    itemCount = filteredItems.Count(x => x is ConsumableItem cItem && cItem.nutritionValue > 0);
                    for (int i = 0; i < itemCount; i++) {
                        if (!SellConsumableItem(itemName)) {
                            break;
                        }
                        if(!soldItems.Contains(itemName)) {
                            soldItems.Add(itemName);
                        }
                    }
                    break;
                case "Commodity":
                    itemCount = filteredItems.Count(x => x is ConsumableItem cItem && cItem.commodityValue > 0);
                    for (int i = 0; i < itemCount; i++) {
                        if (!SellConsumableItem(itemName)) {
                            break;
                        }
                        if(!soldItems.Contains(itemName)) {
                            soldItems.Add(itemName);
                        }
                    }
                    break;
                default:
                    Item currentItem = filteredItems.FirstOrDefault(x => x.itemName.Equals(itemName));
                    if (currentItem == null) {
                        continue;
                    }
                    itemCount = filteredItems.Count(x => x.itemName.Equals(itemName));
                    for (int i = 0; i < itemCount; i++) {
                        if (!SellItem(currentItem)) {
                            break;
                        }
                        if(!soldItems.Contains(itemName)) {
                            soldItems.Add(itemName);
                        }
                    }
                    break;
            }
        }
        return soldItems;
    }

    /// <summary>Transfers x amount of currency from instance's buyer to the seller.</summary>
    public void TransferCurrency(int amount) {
        if (buyer.currency >= amount) {
            buyer.currency -= amount;
            seller.currency += amount;
        } else {
            GD.Print(String.Format("Not enough currency while transferring. Tried to transfer {0} from inventory 1 ({1}) to inventory 2 ({2})", amount, buyer.currency, seller.currency));
        }
    }
    /// <summary>Handles trades done on a trade mission by caravans. Buyer = Caravan, Seller = Trader.</summary>
    public void HandleCaravanTrade() {
        List<string> soldItems = new List<string>();
        if (!buyer.IsEmpty()) {
            soldItems = SellExpensiveItems();
        }
        if (!seller.IsEmpty()) {  // If caravan inventory is empty, it wants to buy whatever it can for cheap.
            BuyCheapItems(soldItems);
        }
    }
}
