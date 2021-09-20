using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Interactive : Area2D
{
    public Dialogue dialogue;
    [Export]
    public String portraitResource = Constants.DEF_PORTRAIT;
    [Export]
    public string entityName = Constants.DEF_CHARACTERNAME;
    
    public Texture portrait;
    public string type;

}