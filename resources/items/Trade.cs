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

    public bool BuyFoodItem() {
        if (seller.items.Any(x => x is ConsumableItem)) {
            ConsumableItem food = seller.items.Where(x => x is ConsumableItem).Cast<ConsumableItem>().OrderBy(x => x.nutritionValue).Last();
            int value = (int)Math.Round((food.value * (1 + tradeProfit)), 0);
            if (buyer.currency >= value && !buyer.IsFull() && food.nutritionValue > 0) {
                seller.currency += value;
                buyer.currency -= value;
                seller.RemoveItem(food);
                buyer.AddItem(food);
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
