using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Stats : Resource
{
    // Entity faction
    [Export]
    public FactionInfo faction;

    // Utilities
    [Export]
	public string profession { get; set; }
    [Export]
    public bool isDead;

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
    private int agilityProficiency;
    private int agilityLeap;

    [Export]
    public int labour { get; private set; }
    private int labourProficiency;
    private int labourLeap;

    // How many skill upgrades per level. Default is two (2).
    [Export]
    public int skillLeap { get; private set; }
    // How many times skill upgrades have already triggered on this character. Similar to classic levels. Affects the amount of tribulation required for next upgrades.
    [Export]
    public int skillUpgrades { get; private set; }
    public int tribulation { get; private set; }
    private int tribulationLeap; // This is how much tribulation is needed to gain a skillUpgrade. Default is [ ( 1.09^skillUpgrades / ((skillUpgrades + 10) * log10(2)) ) * skillUpgrades ] (0 at 0 skillUpgrades)
    public int attackSpeed { get; private set; }    // The amount of combat ticks it takes to do one attack.
    public float critChance { get; private set; }
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

    public float commodities { get; private set; }
    public float minCommodities { get; private set; }
    public float maxCommodities { get; private set; }


    public Stats() {
        isDead = false;
        profession = "";

        strength = 0;
        vitality = 0;
        agility = 0;
        dexterity = 0;
        defence = 0;
        labour = 0;

        hunger = 75;
        commodities = 60;

        tribulation = 0;
        tribulationLeap = CountTribulationLeap();
        skillLeap = 2;

        attackSpeed = Constants.DEF_ATTACKSPEED;

        health = Constants.DEF_HEALTH;
        currentHealth = health;

        maxHunger = Constants.DEF_MAXHUNGER;
        minHunger = Constants.DEF_MINHUNGER;

        minCommodities = Constants.DEF_MINCOMMODITIES;
        maxCommodities = Constants.DEF_MAXCOMMODITIES;

        moveSpeed = Constants.DEF_MAXPEED;
        workSpeed = Constants.DEF_WORKSPEED;

        maxDamage = Constants.DEF_MAXDAMAGE;
        minDamage = Constants.DEF_MINDAMAGE;
    }
    private int GetSkillUpgradesAmount() {
        return (strength + vitality + dexterity + defence) / 2;
    }
    private int CountTribulationLeap() {
        return skillUpgrades == 0 ? 0 : (int)(((Math.Pow(1.09, skillUpgrades) / ((skillUpgrades + 10) * Math.Log10(2))) * skillUpgrades ) * 100);
    }
    private int CountAgilityLeap() {
        return agility == 0 ? 0 : (int)((((Math.Pow(2, 0.05*agility) / Math.Log10(2)+1)) * (0.1 * agility)) * 100);
    }
    private int CountLabourLeap() {
        return labour == 0 ? 0 : (int)((((Math.Pow(2, 0.05*labour) / Math.Log10(2)+1)) * (0.1 * labour)) * 100);
    }
    public void GetTribulationFromKill(int _killedEnemyTribulation, int _killedEnemySkillUpgrades) {
        /*  
            Tribulation gets awarded based on the killed creature's tribulation, and scales downwards.
            A creature with no skill upgrades awards about 25% of its tribulation, while a creature
            with 100 skill upgrades awards about 1,3% of its total tribulation.
        */
        tribulation += (int)(_killedEnemyTribulation * ((Math.Pow(_killedEnemySkillUpgrades, 0.275) / -15) + 0.25));
        GD.Print(string.Format("{0} tribulation after kill", tribulation));
        CheckTribulation();
    }
    public void GetPassiveTribulation() {
        tribulation += 5;
        CheckTribulation();
    }
    private void CheckTribulation() {
        if (tribulation > tribulationLeap) {
            for (int i = tribulation; i > tribulationLeap; ) {
                skillUpgrades++;
                CombatSkillUpgrade();
            }
        }
    }
    public int[] GetStats() {
        return new int[] {strength, vitality, agility, dexterity, defence, labour};
    }
    private void CombatSkillUpgrade() {
        GD.Print("Gained a skill upgrade!");
        RaiseCombatStats();
        RaiseHealth(0.25f * health);
    }
    public void RaiseCombatStats() {
        Random random = new Random();
        string[] stats = new string[] {"str", "vit", "dex", "def"};
        int index;
        for (int i = 0; i < skillLeap; i++) {
            index = random.Next(stats.Length);
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
                    return;
            }
        }
        UpdateStats();
    }
    public void LowerHunger() {
        hunger = (hunger - 0.05f > minHunger ? hunger - 0.05f : minHunger);
        if (hunger < 0) {
            currentHealth -= 0.1f;
            isDead = currentHealth <= 0;
        }
    }
    public void RaiseHunger(float nutrition) {
        hunger = (hunger + nutrition <= maxHunger ? hunger + nutrition : maxHunger);
    }

    public void LowerCommodities() {
        commodities = (commodities - 0.025f > minCommodities ? commodities - 0.025f : minCommodities);
        /*                          // Make it so, that when commodities drop under a certain threshold, the character starts losing 'happiness', or a similar resource.
        if (commodities < 0) {
            currentHealth -= 0.1f;
        }
        */
    }
    public void RaiseCommodities(float commodity) {
        commodities = (commodities + commodity <= maxCommodities ? commodities + commodity : maxCommodities);
    }

    private void RaiseLabour() {
        labour++;
        workSpeed = Constants.DEF_WORKSPEED * (1 - 0.005f * labour);
        labourLeap = CountLabourLeap();
    }
    public void TrainLabour() {
        labourProficiency += 5;
        if (labourProficiency > labourLeap) {
            RaiseLabour();
        }
    }
    private void RaiseAgility() {
        agility++;
        moveSpeed = Constants.DEF_MAXPEED * (1 + 0.01f * agility);
        agilityLeap = CountAgilityLeap();
    }
    public void TrainAgility() {
        agilityProficiency += 5;
        if (agilityProficiency > agilityLeap) {
            RaiseAgility();
        }
    }
    public void RaiseHealth(float heal) {
        currentHealth += heal;
        currentHealth = currentHealth > health ? health : currentHealth;
    }
    public void LowerHealth(float hurt) {
        isDead = (currentHealth -= hurt) <= 0;
    }

    public void UpdateStats() {
        critChance = 0.005f * dexterity;
        dodge = Constants.DEF_DODGECHANCE + (0.005f * dexterity);
        health = Constants.DEF_HEALTH  * (1 + 0.03f * vitality);
        moveSpeed = Constants.DEF_MAXPEED * (1 + 0.01f * agility);
        maxDamage = Constants.DEF_MAXDAMAGE * (1 + 0.025f * strength);
        workSpeed = Constants.DEF_WORKSPEED * (1 - 0.0075f * labour);
        skillUpgrades = GetSkillUpgradesAmount();
        if (tribulation == 0 && skillUpgrades > 0) {
            tribulation = CountTribulationLeap();
        }
        tribulationLeap = CountTribulationLeap();
    }
}