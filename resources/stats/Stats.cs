using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Stats : Resource
{

    // Combat stats
    [Export]
    public int strength { get; private set; }
    [Export]
    public int vitality { get; private set; }
    [Export]
    public int dexterity { get; private set; }
    [Export]
    public int defence { get; private set; }

    // Non-Combat stats
    [Export]
    public int agility { get; private set; }
    [Export]
    public int labour { get; private set; }

    public float attackSpeed { get; private set; }
    public float maxDamage { get; private set; }
    public float minDamage { get; private set; }
    public float moveSpeed { get; private set; }
    public float workSpeed { get; private set; }
    public float dodge { get; private set; }


    private float health;
    public float currentHealth;
    public float hunger { get; private set; }
    public float maxHunger { get; private set; }
    public float minHunger { get; private set; }

    public Stats() {
        strength = 0;
        vitality = 0;
        agility = 0;
        dexterity = 0;
        defence = 0;
        labour = 0;
        hunger = 50;

        attackSpeed = Constants.DEF_ATTACKSPEED;
        health = Constants.DEF_HEALTH;
        currentHealth = health;
        maxHunger = Constants.DEF_MAXHUNGER;
        minHunger = Constants.DEF_MINHUNGER;
        moveSpeed = Constants.DEF_MAXPEED;
        workSpeed = Constants.DEF_WORKSPEED;
        maxDamage = Constants.DEF_MAXDAMAGE;
        minDamage = Constants.DEF_MINDAMAGE;
    }

    public int[] GetStats() {
        return new int[] {strength, vitality, agility, dexterity, defence, labour};
    }

    public void RaiseCombatStats() {
        Random random = new Random();
        string[] stats = new string[] {"str", "vit", "dex", "def"};
        for (int i = 0; i < 2; i++) {
            int index = random.Next(stats.Length);
            switch (stats[index])
            {
                case "str":
                    strength++;
                    break;
                case "vit":
                    vitality++;
                    break;
                case "dex":
                    dexterity++;
                    break;
                case "def":
                    defence++;
                    break; 
                default:
                    GD.Print("Error while leveling character.");
                    break;
            }
        }
        UpdateStats();
    }
    public void lowerHunger() {
        hunger = (hunger - 0.1f > minHunger ? hunger - 0.1f : minHunger);
        if (hunger < 0) {
            currentHealth -= 0.1f;
        }
    }
    public void raiseHunger(float nutrition) {
        hunger += nutrition;
    }

    public void RaiseLabour() {
        labour++;
        workSpeed = Constants.DEF_WORKSPEED * (1 - 0.005f * labour);
    }

    public void RaiseAgility() {
        agility++;
        moveSpeed = Constants.DEF_MAXPEED * (1 + 0.01f * agility);
    }

    public void UpdateStats() {
        attackSpeed = Constants.DEF_ATTACKSPEED * (1 - 0.005f * dexterity);
        dodge = Constants.DEF_DODGECHANCE + (0.005f * dexterity);
        health = Constants.DEF_HEALTH  * (1 + 0.03f * vitality);
        currentHealth = health;
        moveSpeed = Constants.DEF_MAXPEED * (1 + 0.01f * agility);
        maxDamage = Constants.DEF_MAXDAMAGE * (1 + 0.025f * strength);
        workSpeed = Constants.DEF_WORKSPEED * (1 - 0.0075f * labour);
    }
}