using Godot;
using System.Linq;
using System.Collections.Generic;

public class Npc : Character
{
    private List<Resources> surroundingResources = new List<Resources>();
    [Export]
    private string profession = null;

    public void _OnSurroundingsEntered(Area2D area) {
        switch (profession)
        {
            case "lumberjack":
                if (area is Lumber) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;

            case "miner":
                if (area is Deposit) {
                    surroundingResources.Add((Resources)area);
                    area.Connect("OnRemoval", this, nameof(SurroundingRemoved));
                }
                break;
                
            default:
                break;
        }
    }
    public void _OnSurroundingsExited(Area2D area) {
        if (surroundingResources.Contains((Resources)area)) {
            if (area.IsConnected("OnRemoval", this, nameof(SurroundingRemoved))) {
                area.Disconnect("OnRemoval", this, nameof(SurroundingRemoved));
            }
            surroundingResources.Remove((Resources)area);
        }
    }
    private void SurroundingRemoved(Interactive resource) {
        surroundingResources.Remove((Resources)resource);
    }
    public bool GetNextWork() {
        bool foundWork = surroundingResources.Any();
        Resources currentResource = null;
        float shortestDistance = float.MaxValue;
        foreach(Resources i in surroundingResources) {
            if ((currentResource == null || Position.DistanceTo(i.Position) < shortestDistance)) {
                if (!IsInstanceValid(i)) {
                    surroundingResources.Remove(i);
                    continue;
                }
                currentResource = i;
                shortestDistance = Position.DistanceTo(currentResource.Position);
            }
        }
        SetInteractive(currentResource);
        return foundWork;
    }
}
