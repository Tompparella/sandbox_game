using Godot;
using System;

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
}
