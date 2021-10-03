using Godot;
using System.Linq;

public class Trade
{
    private Inventory buyer, seller;
    public Trade(Inventory _buyer, Inventory _seller) {
        buyer = _buyer;
        seller = _seller;
    }

    public void BuyItem(Item item) {
        if (seller.items.Contains(item)) {
            if (buyer.currency > item.value && !buyer.IsFull()) {
                int value = item.value;
                seller.currency += item.value;
                buyer.currency -= item.value;
                seller.RemoveItem(item);
                buyer.AddItem(item);
            }
        } else {
            GD.Print("Buying: No item or currency in inventory.");
        }
    }

    public void SellItem(Item item) {
        if (buyer.items.Contains(item)) {
            if (seller.currency > item.value && !seller.IsFull()) {
                int value = item.value;
                buyer.currency += item.value;
                seller.currency -= item.value;
                buyer.RemoveItem(item);
                seller.AddItem(item);
            }
        } else {
            GD.Print("Selling: No item or currency in inventory.");
        }
    }
}
