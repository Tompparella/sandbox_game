using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ResourceDialogue: Dialogue {

    public ResourceDialogue(Interactive _owner, string description, string[] actions): base(_owner) {
        owner = _owner;
        knownActions = actions;
        Random random = new Random();
        unknown = Constants.UNKNOWN[random.Next(Constants.UNKNOWN.Count())];
        initial = description;
    }

    public override string TakeSayInput(string[] words) {
        return String.Format(Constants.RESOURCETALK, owner.entityName); // Placeholder
    }

    public override string TakeDoInput(string[] commands) {
        string word = knownActions.FirstOrDefault(x => x.Equals(commands[0]));

        switch (word)
        {
            case null:
                return null;
            case "work":
                return "work";
            case "chop":
                return "work";
            default:
                return unknown;
        }
    }
}