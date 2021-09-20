using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Lumber : Resource
{
    public override void _Ready()
    {
        dialogue = new ResourceDialogue(this, Constants.LUMBERDESCRIPTION, Constants.LUMBERACTIONS);
        entityName = "Lumber";
        base._Ready();
    }
}
