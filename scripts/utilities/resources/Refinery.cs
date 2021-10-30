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
        if (workItemQueue == null) {
            workItemQueue = new List<Item>();
        }
        base._Ready();
    }
    public override void workAction(Character worker) {
        if (!workItemQueue.Any()) {
            if (!setWorkItemQueue(worker)) {
                worker.SetInteractive(); // Leave work state if no longer resources to continue crafting.
                return;
            };
            GD.Print(workItemQueue[0].itemName);
        }
        currentActions++;
        GD.Print(currentActions);
        if (currentActions >= requiredActions) {
            GiveResource(worker);
            currentActions = 0;
        }
    }

    public bool setWorkItemQueue(Character worker) { // Returns false if there's not enough resource items.
        // In descending priority order, 
        foreach(Item i in craftableItems) {
            if (worker.inventory.HasItems(i.recipe)) {
                addItemToWorkQueue(i, worker);
                return true;
            }
        }
        return false;
    }

    protected void addItemToWorkQueue(Item item, Character worker) {
        int requiredSpace = 0;
        foreach (int i in item.recipe.Values) {
            requiredSpace += i;
        }
        if (inventory.GetFreeSpace() >= requiredSpace) {
            foreach(KeyValuePair<Item, int> kvp in item.recipe) {   //  Move the specific amount of resource items from the worker to the crafting resource.
                worker.inventory.RemoveItem(kvp.Key, kvp.Value);
                inventory.AddItem(kvp.Key, kvp.Value);
            }
            workItemQueue.Add(item);
        }
    }

    public override void GiveResource(Character worker) {
        if (!worker.inventory.IsFull()) {
            Item workItem = workItemQueue[0];
            if (inventory.HasItems(workItem?.recipe)) {
                foreach(KeyValuePair<Item, int> kvp in workItem.recipe) {
                    inventory.RemoveItem(kvp.Key, kvp.Value);
                }
                worker.inventory.AddItem(workItem);
            } else {
                GD.Print(this, ": Inventory lacks required raw materials.");
            }
        }
    }

    public override void _OnMouseOver() {
    }
    public override void _OnMouseExit() {
    }
}