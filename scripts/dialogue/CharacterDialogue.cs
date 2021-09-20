using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterDialogue: Dialogue {
    public CharacterDialogue(Interactive _owner): base(_owner) {
        owner = _owner;
        Random random = new Random();
        affirmative = Constants.AFFIRMATIVE[random.Next(Constants.AFFIRMATIVE.Count())];
        negative = Constants.NEGATIVE[random.Next(Constants.NEGATIVE.Count())];
        unknown = Constants.UNKNOWN[random.Next(Constants.UNKNOWN.Count())];
        initial = Constants.GREETINGS[random.Next(Constants.GREETINGS.Count())];
    }
}