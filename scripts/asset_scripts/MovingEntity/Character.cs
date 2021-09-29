using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Character : MovingEntity
{
    // Character specific variables here.

    [Export]
    public bool isDead = false;
    [Export]
    public Stats stats;

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
        Random random = new Random();
        DamageCounter newCounter = (DamageCounter)damageCounter.Instance();
        if (random.NextDouble() >= stats.dodge) {
            stats.currentHealth -= attack.damage * (1 - 0.01f * stats.defence);
            GD.Print(Name, ":", stats.currentHealth);
            newCounter.init(attack.damage.ToString());
        } else {
            newCounter.init("Dodge");
        }
        EmitSignal("Attacked");
        this.AddChild(newCounter);
    }

    // Utility functions
    
    public override void _Ready()
    {
        dialogue = new CharacterDialogue(this);

        if (stats == null) {
            stats = new Stats();
        } else {
            stats.UpdateStats();
        }

        portrait = (Texture)ResourceLoader.Load(portraitResource);
        damageCounter = (PackedScene)ResourceLoader.Load("res://assets/combat/damagecounter.tscn");

        this.Connect("mouse_entered", this, nameof(_OnMouseOver));
        this.Connect("mouse_exited", this, nameof(_OnMouseExit));
        this.Connect("input_event", this, nameof(_OnClickEvent));

        base._Ready();
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