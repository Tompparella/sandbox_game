using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Oven : Resources
{
    private List<Item> craftableItems; // In priority order.
    private List<Item> workItemQueue;

    public override void _Ready()
    {
        craftableItems = new List<Item>() {
            (Item)GD.Load(Constants.BREADITEM),
            (Item)GD.Load(Constants.FLOURITEM),
        };

        actions = Constants.OVENACTIONS;
        dialogue = new ResourceDialogue(this, Constants.OVEN_DESCRIPTION, actions);

        exhaustedDescription = Constants.OVEN_DESCRIPTION;
        exhaustedTexture = Constants.TREETRUNK_TEXTURE;
        exhaustedName = "Tree Trunk";
        exhaustedPortrait = Constants.TREETRUNK_PORTRAIT;

        base._Ready();
    }

    public override void workAction(Character worker) {
        if (workItemQueue == null) {
            if (!setWorkItemQueue(worker)) {
                worker.SetInteractive(); // Leave work state if no longer resources to continue crafting.
                return;
            };
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

    private void addItemToWorkQueue(Item item, Character worker) {
        int requiredSpace = 0;
        foreach (int i in item.recipe.Values) {
            requiredSpace += i;
        }
        if (inventory.FreeSpace() >= requiredSpace) {
            foreach(KeyValuePair<Item, int> kvp in item.recipe) {
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
}