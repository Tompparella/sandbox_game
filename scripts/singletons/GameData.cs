using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameData : Node
{
    /*For having a collection of items in one place*/
    private Godot.Collections.Dictionary itemData;
    public override void _Ready()
    {
        File file = new File();
        file.Open("res://data/ItemData.json", File.ModeFlags.Read);
        JSONParseResult itemDataJson = JSON.Parse(file.GetAsText());
        file.Close();
        itemData = (Godot.Collections.Dictionary)itemDataJson.Result;
    }
}
