using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Resource : Interactive
{

    [Signal]
    public delegate void OnMouseOver(Resource resource);
    [Signal]
    public delegate void OnMouseExit(Resource resource);

    public override void _Ready()
    {
        portrait = (Texture)ResourceLoader.Load(portraitResource);
        this.Connect("mouse_entered", this, nameof(_OnMouseOver));
        this.Connect("mouse_exited", this, nameof(_OnMouseExit));
    }

    public virtual void _OnMouseOver() {
        EmitSignal("OnMouseOver", this);
    }
    public virtual void _OnMouseExit() {
        EmitSignal("OnMouseExit");
    }
}