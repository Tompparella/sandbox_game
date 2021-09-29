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
                sprite.Texture = (Texture)ResourceLoader.Load(Constants.IRON_DEPOSIT_TEXTURE);
                break;
            case "silver":
                inventory = (Inventory)ResourceLoader.Load(Constants.SILVER_DEPOSIT_INVENTORY).Duplicate();
                description = Constants.SILVERDEPOSIT_DESCRIPTION;
                entityName = "Silver Deposit";
                portraitResource = Constants.SILVER_DEPOSIT_PORTRAIT;
                sprite.Texture = (Texture)ResourceLoader.Load(Constants.SILVER_DEPOSIT_TEXTURE);
                break;
            default:
                description = Constants.EMPTYDEPOSIT_DESCRIPTION;
                entityName = "Barren Ore Deposit";
                portraitResource = Constants.DEPOSIT_PORTRAIT;
                sprite.Texture = (Texture)ResourceLoader.Load(Constants.DEPOSIT_TEXTURE);
                isExhausted = true;
                break;
        }
        dialogue = new ResourceDialogue(this, description, actions);

        base._Ready();
    }
}