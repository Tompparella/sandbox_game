using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Barracks : Refinery
{
    private Supply supply;
    private List<Npc> soldiers = new List<Npc>();
    private int logisticsPayment = 100;     // Amount of currency given to logistics officers when sent out to purchase goods for the barracks.
    private int storedFoodAmount = 3;       // Barracks always attempts to keep at least this number of food in store.
    private int storedCommodityAmount = 1;  // Barracks always attempts to keep at least this number of commodities in store.
    private float funding = 0.75f;          // The amount of funding the state is giving to this barracks. Affects the amount of subsidized supplies.

    public override void _Ready()
    {
        craftableItems = new List<Item>() {
            (Item)GD.Load(Constants.LOGISTICSITEM),
            (Item)GD.Load(Constants.RATIONITEM),
            (Item)GD.Load(Constants.RATIONEDGOODSITEM),
        };
        // This is still badly unfinished.
        supply = supply == null ? new Supply() : supply;


        requiredActions = (int)Math.Round(requiredActions * 0.5); // The logistics officer only repurposes existing items for military use, which doesn't take much effort.
        actions = Constants.DEF_WORKACTIONS;
        dialogue = new ResourceDialogue(this, Constants.BARRACKS_DESCRIPTION, actions);

        defaultTexture = Constants.WOODCRAFT_TEXTURE;
        defaultPortrait = Constants.WOODCRAFT_PORTRAIT;
        defaultName = Constants.WOODCRAFT_NAME;
        defaultDescription = Constants.BARRACKS_DESCRIPTION;

        exhaustedDescription = Constants.OVEN_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        createDebugInstance(); // Comment out for production.

        base._Ready();
    }

    /*
    Logistics Officer gets assigned in the extended AddWorker function. The barracks adds items on its
    neededItems list depending on the amount of stored goods at the moment. For example, the barracks tries
    to always keep at least four food items and two consumer goods stored in its inventory. 
    */

    /*
    Supply represents the materials that are consumed while maintaining an armed force. Anything from repairing
    chipped blades and bent armor, to providing training materials, as well as decent living quarters. If supplies
    aren't met, the combat efficiency of soldiers degrades depending on how large the deficiency becomes. 
    */

    /*
    The logistics officer will create special 'ration' food items for the soldiers. These are free for the soldiers to use.
    Ration's nutrition value depends on what food item it was made from.
    */

    public override bool setWorkItemQueue(Character worker)
    {
        foreach (Item i in craftableItems)
        {
            /*
            Food- and commodity items require separate handling.
            */

            if (i is ConsumableItem) {                                                          
                ConsumableItem item = (ConsumableItem)i;
                if (item.nutritionValue > 0) {
                    if (inventory.HasItem(item, storedFoodAmount)) { // If inventory has stored amount of food/commodities, don't add it to workqueue and clear food/commodities from logistics officer's neededItems.
                        ((Npc)worker).ClearFoodFromBuyQueue();
                        continue;
                    }
                    ConsumableItem originItem = worker.inventory.GetEdibleItems()?.Last();  
                    if (originItem != null) {
                        item.nutritionValue = originItem.nutritionValue;
                        addItemToWorkQueue(i, worker);
                        return true;
                    }
                }

                else if (item.commodityValue > 0) {
                    if (inventory.HasItem(item, storedCommodityAmount)) {
                        ((Npc)worker).ClearCommoditiesFromBuyQueue();
                        continue;
                    }
                    ConsumableItem originItem = worker.inventory.GetCommodityItems()?.Last();
                    if (originItem != null) {
                        item.commodityValue = originItem.commodityValue;
                        addItemToWorkQueue(i, worker);
                        return true;
                    }
                }
            }
            else if (worker.inventory.HasItems(i.recipe.GetRecipeDictionary()))
            {
                //GD.Print(String.Format("Craftable item: {0}\n{1}", i.itemName, i.recipe.GetRecipe()));
                addItemToWorkQueue(i, worker);
                return true;
            }
            if (i is LogisticsItem && supply.supply > (100 * funding)) {   // If there's enough supplies, don't buy supplies to make them.
                continue;
            }
            worker.checkBuyQueue(i.recipe.GetRecipeDictionary());
        }

        if (worker.inventory.currency < logisticsPayment) {     // Transfer funds from inventory to the logistics officer to purchase necessary goods.
            Trade tradeInstance = new Trade(inventory, worker.inventory);
            tradeInstance.TransferCurrency(logisticsPayment);
        }
        return false;
    }

    public override void GiveResource(Character worker)
    /*
    Barracks doesn't create items. It just consumes supply to provide logistics for soldiers.
    Logistics Officers provide barracks with food and goods that the soldiers will use.
    Soldiers are notified if the stocks of the barracks are updated with something they need.
    */
    {
        Item workItem = workItemQueue[0];
        //GD.Print("Currently given item:", workItem.itemName);
        if (inventory.HasItems(workItem?.recipe.GetRecipeDictionary()))
        {
            foreach (KeyValuePair<Item, int> kvp in workItem.recipe.GetRecipeDictionary())
            {
                inventory.RemoveItem(kvp.Key, kvp.Value);
            }
            inventory.AddItem(workItem);
            workItemQueue.RemoveAt(0);
            ItemAdded(workItem);
        }
        else
        {
            GD.Print(this.Name, ": Refinery : GiveResource: Inventory lacks required raw materials.");
        }
        currentActions = 0;
        workAction(worker);
    }

    private void ItemAdded(Item item)
    {
        if (item is LogisticsItem && supply.supply <= 100 * funding)
        {
            RefillSupply((LogisticsItem)item);
        }
    }

    private void RefillSupply(LogisticsItem supplyItem)
    {
        supply.supply += supplyItem.supplyValue;
        if (!Enumerable.Range(0, 100).Contains((int)supply.supply))
        {
            supply.supply = supply.supply < 0 ? 0 : 100; // If supply is out of bounds, set it to be either 0 or 100 depending on whether it's over or below the limits.
        }
        inventory.RemoveItem(supplyItem);
    }

    private float GetSupply() {
        return supply.supply;
    }

    private void createDebugInstance() // Keep track of supply etc. during development.
	{
		PackedScene packedDebug = (PackedScene)ResourceLoader.Load("res://assets/debug/DebugInstance.tscn");
		DebugInstance debugInstance = (DebugInstance)packedDebug.Instance();
		AddChild(debugInstance);
		debugInstance.AddStat("Soldiers", this, "soldiers", false);
		debugInstance.AddStat("Supply", this, "GetSupply", true);
		debugInstance.AddStat("Logistics Payment", this, "logisticsPayment", false);
	}
}