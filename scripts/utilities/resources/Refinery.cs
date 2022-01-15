using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Refinery : Resources
{
    protected List<Item> craftableItems; // In priority order.
    protected List<Item> workItemQueue;

    public override void _Ready()
    {
        if (workItemQueue == null)
        {
            workItemQueue = new List<Item>();
        }
        base._Ready();
    }
    public override bool AddWorker(Character worker)
    {
        if (worker is Npc && ((Npc)worker).hasTraded && !setWorkItemQueue(worker))
        {
            Npc npcWorker = (Npc)worker;
            npcWorker.outOfWork = true;
            npcWorker.outOfWorkTimer.Start();
            return false;
        }
        workers.Add(worker);
        return true;
    }
    public override void workAction(Character worker)
    {
        if (!workItemQueue.Any())
        {
            if (!setWorkItemQueue(worker))
            {
                //GD.Print(String.Format("Nothing to work on refinery {0}", this.Name));
                worker.SetInteractive(); // Leave work state if no longer resources to continue crafting.
                return;
            };
            //GD.Print(String.Format("Refinery {0} now working on item {1}", this.Name, workItemQueue[0].itemName));
        }
        currentActions++;
        //GD.Print(worker.Name, currentActions);
        if (currentActions >= requiredActions)
        {
            GiveResource(worker);
        }
    }

    public virtual bool setWorkItemQueue(Character worker)
    { // Returns false if there's not enough resource items.
        // In descending priority order, 
        foreach (Item i in craftableItems)
        {
            if (worker.inventory.HasItems(i.recipe.GetRecipeDictionary()))
            {
                //GD.Print(String.Format("Craftable item: {0}\n{1}", i.itemName, i.recipe.GetRecipe()));
                addItemToWorkQueue(i, worker);
                return true;
            }
            worker.checkBuyQueue(i.recipe.GetRecipeDictionary());
        }
        return false;
    }

    protected void addItemToWorkQueue(Item item, Character worker)
    {
        int requiredSpace = 0;
        Dictionary<Item, int> recipeItemDictionary = item.recipe.GetRecipeDictionary();
        foreach (int i in recipeItemDictionary.Values)
        {
            requiredSpace += i;
        }
        if (inventory.GetFreeSpace() >= requiredSpace)
        {
            foreach (KeyValuePair<Item, int> kvp in recipeItemDictionary)
            {   //  Move the specific amount of resource items from the worker to the crafting resource.
                worker.inventory.RemoveItem(kvp.Key, kvp.Value);
                worker.PopFromSellQueue(kvp.Key);
                inventory.AddItem(kvp.Key, kvp.Value);
            }
            workItemQueue.Add(item);
        }
    }

    public override void GiveResource(Character worker)
    {
        if (!worker.inventory.IsFull())
        {
            Item workItem = workItemQueue[0];
            //GD.Print("Currently given item:", workItem.itemName);
            if (inventory.HasItems(workItem?.recipe.GetRecipeDictionary()))
            {
                foreach (KeyValuePair<Item, int> kvp in workItem.recipe.GetRecipeDictionary())
                {
                    inventory.RemoveItem(kvp.Key, kvp.Value);
                }
                worker.AddToSellQueue(workItem);
                worker.inventory.AddItem(workItem);
                workItemQueue.RemoveAt(0);
            }
            else
            {
                GD.Print(this.Name, ": Refinery : GiveResource: Inventory lacks required raw materials.");
            }
        }
        currentActions = 0;
        workAction(worker);
    }
    /*
        public override void _OnMouseOver() {
        }
        public override void _OnMouseExit() {
        }
    */
}