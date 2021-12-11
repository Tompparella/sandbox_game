using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Deposit : Resources
{
    [Export]
    private string depositType;
    public override void _Ready()
    {
        sprite = (Sprite)GetNode("Sprite");
        
        actions = Constants.DEPOSITACTIONS;
        exhaustedDescription = Constants.EMPTYDEPOSIT_DESCRIPTION;
        exhaustedTexture = Constants.DEPOSIT_TEXTURE;
        exhaustedName = "Barren Ore Deposit";
        exhaustedPortrait = Constants.DEPOSIT_PORTRAIT;

        string description;
        switch (depositType)
        {
            case "iron":
                inventory = (Inventory)ResourceLoader.Load(Constants.IRON_DEPOSIT_INVENTORY).Duplicate();
                description = Constants.IRONDEPOSIT_DESCRIPTION;
                entityName = "Iron Deposit";
                portraitResource = Constants.IRON_DEPOSIT_PORTRAIT;
                defaultTexture = Constants.IRON_DEPOSIT_TEXTURE;
                defaultInventory = Constants.IRON_DEPOSIT_INVENTORY;
                sprite.Texture = (Texture)ResourceLoader.Load(Constants.IRON_DEPOSIT_TEXTURE);
                requiredActions = requiredActions * 2;
                break;
            case "silver":
                inventory = (Inventory)ResourceLoader.Load(Constants.SILVER_DEPOSIT_INVENTORY).Duplicate();
                description = Constants.SILVERDEPOSIT_DESCRIPTION;
                entityName = "Silver Deposit";
                portraitResource = Constants.SILVER_DEPOSIT_PORTRAIT;
                defaultTexture = Constants.SILVER_DEPOSIT_TEXTURE;
                defaultInventory = Constants.SILVER_DEPOSIT_INVENTORY;
                sprite.Texture = (Texture)ResourceLoader.Load(Constants.SILVER_DEPOSIT_TEXTURE);
                requiredActions = requiredActions * 3;
                break;
            default:
                description = Constants.EMPTYDEPOSIT_DESCRIPTION;
                entityName = "Barren Ore Deposit";
                portraitResource = Constants.DEPOSIT_PORTRAIT;
                defaultTexture = Constants.DEPOSIT_TEXTURE;
                sprite.Texture = (Texture)ResourceLoader.Load(Constants.DEPOSIT_TEXTURE);
                isExhausted = true;
                break;
        }

        defaultPortrait = portraitResource;
        defaultName = entityName;
        defaultDescription = description;

        dialogue = new ResourceDialogue(this, description, actions);

        base._Ready();
    }
}