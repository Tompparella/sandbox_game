using Godot;
using System;

public class Character : MovingEntity
{
    // Character specific variables here.

    [Signal]
    public delegate void OnCharacterClick(Character character); // Add dialogue info here.
}
