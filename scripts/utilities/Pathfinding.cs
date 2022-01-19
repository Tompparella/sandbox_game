using Godot;
using System;
public class Pathfinding : Navigation2D
{
    //private Line2D pathLine; // Uncomment for a navigation line.

    public override void _Ready()
    {
        //pathLine = (Line2D)GetNode("PathLine");
        Godot.Collections.Array characters, spawners;
        SceneTree tree = GetTree();
        characters = tree.GetNodesInGroup("character");
        spawners = tree.GetNodesInGroup("spawner");


        foreach (Node character in characters) {
            character.Connect("CallForPath", this, nameof(FindPath));
        }
        foreach (Node spawner in spawners)
        {
            spawner.Connect("SpawnEntity", this, nameof(AddMovingEntity));
        }
    }

    public void AddMovingEntity(MovingEntity entity) {
        entity.Connect("CallForPath", this, nameof(FindPath));
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
