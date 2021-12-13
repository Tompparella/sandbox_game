using Godot;
using System;
using System.Linq;

public class Trade
{
    private Inventory buyer, seller;
    private float tradeProfit = Constants.DEF_TRADEPROFIT;

    public Trade(Inventory _buyer, Inventory _seller) {
        buyer = _buyer;
        seller = _seller;
    }

    public bool BuyItem(Item item) {
        if (seller.items.Contains(item)) {
            int value = (int)Math.Round((item.value * (1 + tradeProfit)), 0); // Trader gets a certain profit margin from each trade that's determined by the 'tradeProfit' modifier.
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
        if (seller.items.Any(x => x is ConsumableItem)) {
            ConsumableItem itemToBuy;
            switch (itemType)
            {
                case "food":
                    itemToBuy = seller.items.Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.nutritionValue).Last();
                    if (itemToBuy.nutritionValue <= 0) {
                        return false;
                    }
                    break;
                case "commodity":
                    itemToBuy = seller.items.Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.commodityValue).Last();
                    if (itemToBuy.commodityValue <= 0) {
                        return false;
                    }
                    break;
                default:
                    GD.Print(String.Format("Wrong item type when buying consumable item: {0}", itemType));
                    return false;
            }
            int value = (int)Math.Round((itemToBuy.value * (1 + tradeProfit)), 0);
            if (buyer.currency >= value && !buyer.IsFull()) {
                seller.currency += value;
                buyer.currency -= value;
                seller.RemoveItem(itemToBuy);
                buyer.AddItem(itemToBuy);
                return true;
            }
        } else {
            //GD.Print("Buying food: No food or currency in inventory.");
        }
        return false;
    }

    public bool SellItem(Item item) {
        if (buyer.items.Contains(item)) {
            int value = (int)Math.Round((item.value * (1 - tradeProfit)), 0);
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
}
