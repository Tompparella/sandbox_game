using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Character : MovingEntity
{
    // Character specific variables here.
    public float attackSpeed; // Placeholder
    public float health; // Placeholder

    [Export]
    public bool isDead = false;
    [Export]
    public int strength = 0;
    [Export]
    public int vitality = 0;
    [Export]
    public int agility = 0;
    [Export]
    public int dexterity = 0;
    [Export]
    public int defence = 0;
    [Export]
    public int labour = 0;

    // Signals

    [Signal]
    public delegate void OnMouseOver(Character character);
    [Signal]
    public delegate void OnMouseExit(Character character);
    [Signal]
    public delegate void OnCharacterClick(Character character, InputEvent @event);
    [Signal]
    public delegate void Attacked();
    
    // Utility variables

    private List<Character> targets = new List<Character>();
    private Interactive currentInteractive;
    private PackedScene damageCounter;

    // Helper functions

    public void SetInteractive(Interactive interactive = null) {
        currentInteractive = interactive;
    }
    public Interactive GetInteractive() {
        return currentInteractive;
    }
    
    public void AddTarget(Character target) {
        if (!targets.Contains(target)) {
            targets.Add(target);
        }
    }

    public void ClearTargets() {
        targets.Clear();
    }

    public void SetTarget(Character target) {
        targets.Clear();
        targets.Add(target);
    }

    public Character GetTarget() {
        if (targets.Any()) {
            return targets[0];
        }
        return null;
    }

    public void ClearCurrentTarget() {
        targets.RemoveAt(0);
    }

    // Combat functions

    public void TakeAttack(Attack attack) {
        AddTarget(attack.source);
        health -= attack.damage;
        GD.Print(Name, ":", health);
        DamageCounter newCounter = (DamageCounter)damageCounter.Instance();
        newCounter.init(attack.damage);
        this.AddChild(newCounter);
        EmitSignal("Attacked");

    }

    // Utility functions
    
    public override void _Ready()
    {
        dialogue = new CharacterDialogue(this);
        portrait = (Texture)ResourceLoader.Load(portraitResource);
        attackSpeed = Constants.DEF_ATTACKSPEED;
        health = Constants.DEF_HEALTH + vitality;
        this.Connect("mouse_entered", this, nameof(_OnMouseOver));
        this.Connect("mouse_exited", this, nameof(_OnMouseExit));
        this.Connect("input_event", this, nameof(_OnClickEvent));
        damageCounter = (PackedScene)ResourceLoader.Load("res://assets/combat/damagecounter.tscn");
    }

    // Input functions

    public virtual void _OnMouseOver() {
        EmitSignal("OnMouseOver", this);
    }
    public virtual void _OnMouseExit() {
        EmitSignal("OnMouseExit");
    }
    public virtual void _OnClickEvent(Node viewport, InputEvent @event, int shapedIdx) {
        if (@event is InputEventMouseButton) {
            EmitSignal("OnCharacterClick", this, @event);
        }
    }
}
