using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Barracks : Refinery
{
    private Supply supply;
    private List<Npc> soldiers = new List<Npc>();

    public override void _Ready()
    {
        craftableItems = new List<Item>() {
            (Item)GD.Load(Constants.LOGISTICSITEM),
        };
        // This is still badly unfinished.
        supply = supply == null ? new Supply() : supply;

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
        if (item is LogisticsItem && supply.supply <= 50)
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
}