using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class WheatField : Resources
{
    public override void _Ready()
    {
        actions = Constants.FARMACTIONS;
        dialogue = new ResourceDialogue(this, Constants.FARM_DESCRIPTION, actions);
        entityName = "Wheat Field";
        inventory = (Inventory)ResourceLoader.Load(Constants.WHEATFIELD_INVENTORY).Duplicate();

        defaultTexture = Constants.FARM_TEXTURE;
        defaultPortrait = Constants.FARM_PORTRAIT;
        defaultName = Constants.FARM_NAME;
        defaultDescription = Constants.FARM_DESCRIPTION;
        defaultInventory = Constants.WHEATFIELD_INVENTORY;

        exhaustedDescription = Constants.EMPTYFARM_DESCRIPTION;
        exhaustedTexture = Constants.EMPTYFARM_TEXTURE;
        exhaustedName = "Farm Patch";
        exhaustedPortrait = Constants.EMPTYFARM_PORTRAIT;

        base._Ready();
    }
}