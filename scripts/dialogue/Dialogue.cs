using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Dialogue {

    public Dialogue(Interactive _owner) {
        owner = _owner;
        Random random = new Random();
        affirmative = Constants.AFFIRMATIVE[random.Next(Constants.AFFIRMATIVE.Count())];
        negative = Constants.NEGATIVE[random.Next(Constants.NEGATIVE.Count())];
        unknown = Constants.UNKNOWN[random.Next(Constants.UNKNOWN.Count())];
        initial = Constants.GREETINGS[random.Next(Constants.GREETINGS.Count())];
    }
    protected Interactive owner;
    protected string affirmative;
    protected string negative;
    protected string unknown;
    protected string[] knownWords = new string[] {};
    protected string[] knownActions = new string[] {};
    public string initial;
    public string Yes() {
        return unknown;
    }
    public string No() {
        return unknown;
    }
    public string Unknown {
        get { return unknown; }
    }

    public virtual string TakeSayInput(string[] words) {
        string word = knownWords.FirstOrDefault(x => x.Equals(words[0]));

        switch (word)
        {
            case null:
                return unknown;
            case "yes":
                return affirmative;
            case "no":
                return negative;
            default:
                return unknown;
        }
    }

    public virtual string TakeDoInput(string[] commands) {
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