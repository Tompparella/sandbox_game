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
    public Inventory inventory = new Inventory();

    public override void _Ready()
    {
        if (inventory == null) {
            inventory = new Inventory();
        }
        inventory.Connect("OnItemAdd", this, "CheckNeeds");
    }

    public virtual void CheckNeeds() {
    }

}