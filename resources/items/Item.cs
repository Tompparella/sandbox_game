using Godot;
using System.Collections.Generic;

public class Item : Resource
{
    [Export]
    public string itemName = "";
    [Export]
    public Texture texture;
    [Export]
    public Texture worldTexture;
    [Export]
    public string itemDescription = "";
    [Export]
    public int value = 0;
    [Export]
    public Recipe recipe;
}
