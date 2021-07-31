using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MovingEntity : KinematicBody2D
{

    [Signal]
    public delegate void CallForPath(Vector2 start, Vector2 end, Character caller);

    public int maxSpeed { get; } = 3;
    public float currentSpeed { get; set; } = 0;
    public int acceleration = 6;
    protected Vector2 movement = new Vector2();
    protected Vector2 destination = new Vector2();
    public List<Vector2> movePath { get; set; }

    public Vector2[] MovePath {
        get { return movePath.ToArray(); }
        set { movePath = value.ToList(); }
    }

    public void GetMovePath(Vector2 start, Vector2 end, Character caller) {
        EmitSignal(nameof(CallForPath), start, end, caller);
    }
}
