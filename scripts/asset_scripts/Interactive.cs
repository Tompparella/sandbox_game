using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Interactive : Area2D
{
    [Signal]
    public delegate void OnRemoval(Interactive entity);
    public Dialogue dialogue;
    [Export]
    public String portraitResource = Constants.DEF_PORTRAIT;
    [Export]
    public string entityName = Constants.DEF_CHARACTERNAME;
    
    public Texture portrait;
    public string type;
    [Export]
    public Inventory inventory;
    public Inventory tradeInventory;    // The inventory used for trading. On professional traders (trader, logistics officer) it's set to be the working resource's inventory (tradestall, barracks).

    public override void _Ready()
    {
        if (inventory == null) {
            inventory = (Inventory)ResourceLoader.Load(Constants.DEF_INVENTORY).Duplicate();
        } else {
            inventory = (Inventory)inventory.Duplicate(); // Creates a unique instance of inventory. TODO: When implementing savegame, this has to be redone.
        }
        inventory.Connect("OnItemAdd", this, nameof(CheckNeeds));
        inventory.Connect("OnItemRemoved", this, nameof(ItemRemoved));
        tradeInventory = inventory;
    }

    public virtual void CheckNeeds(Item item = null) {
    }

    public virtual void ItemRemoved(Item item) {
    }

}