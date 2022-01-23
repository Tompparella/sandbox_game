using Godot;
using System.Collections.Generic;
using System.Linq;

public class FactionInfo : Resource {
    public List<Settlement> settlements = new List<Settlement>();
    [Export]
    public List<string> hostileFactions;
    [Export]
    public string factionName = "Faction";
}