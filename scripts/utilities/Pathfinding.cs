using Godot;
using System;
public class Pathfinding : Navigation2D
{
    //private Line2D pathLine; // Uncomment for a navigation line.
    private Godot.Collections.Array characters;

    public override void _Ready()
    {
        //pathLine = (Line2D)GetNode("PathLine");
        characters = GetTree().GetNodesInGroup("character");

        foreach (Node character in characters) {
            character.Connect("CallForPath", this, nameof(FindPath));
        }
    }

    public void FindPath(Vector2 start, Vector2 end, Character caller) {
        //pathLine.Points = GetSimplePath(start, end);
        try
        {
            caller.MovePath = GetSimplePath(start, end);
        }
        catch (System.Exception e)
        {
            GD.Print(e);
            throw e;
        }
    }
}
