using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Character : MovingEntity
{
    // Character specific variables here.

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
    public delegate void AttackSignal();
    [Signal]
    public delegate void UnderAttack(Character attacker);
    
    // Utility variables

    private Dictionary<Character,float> targets = new Dictionary<Character, float>(); // The float represents aggro
    public List<Item> neededItems { get; private set; } = new List<Item>();
    public bool aggressive = false;
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
        if (!targets.ContainsKey(target)) {
            targets.Add(target, 1.0f);
        }
    }

    public void ClearTargets() {
        targets.Clear();
    }

    public void SetTarget(Character target) {
        targets.Clear();
        AddTarget(target);
    }

    public Character GetTarget() {
        if (targets.Any()) {    // Select target with highest aggro.
            Character target = targets.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            if (IsInstanceValid(target)) {
                return target;
            } else {
                targets.Remove(target);
                return GetTarget();
            }
        }
        return null;
    }

    public void ClearCurrentTarget() {
        Character target = GetTarget();
        if (target != null) {
            targets.Remove(target);
        }
    }

    public bool checkBuyQueue()
	{
		return neededItems.Any();
	}
	public List<Item> GetBuyQueue()
	{
		return neededItems;
	}
    public string GetFaction() {
        return stats.faction?.factionName;
    }
    public void SetFaction(FactionInfo faction) {
        stats.faction = faction;
    }

    // Combat functions

    public void AttackTarget(Character target) {
        AddTarget(target);
        EmitSignal(nameof(AttackSignal));
    }
    public void TakeAttack(Attack attack) {
        AddTarget(attack.source);
        EmitSignal(nameof(UnderAttack), attack.source);
        // Increase aggro
        targets[attack.source] += attack.damage;
        Random random = new Random();
        DamageCounter newCounter = (DamageCounter)damageCounter.Instance();
        if (random.NextDouble() >= stats.dodge) {
            stats.currentHealth -= attack.damage * (1 - 0.01f * stats.defence);
            GD.Print(entityName, ":", stats.currentHealth);
            newCounter.init(attack.damage.ToString());
        } else {
            newCounter.init("Dodge");
        }
        EmitSignal(nameof(AttackSignal));
        this.AddChild(newCounter);
    }

    // Utility functions

    public bool checkBuyQueue(Dictionary<Item,int> items) {
        bool itemsAdded = false;
        foreach(KeyValuePair<Item, int> kvp in items) {
            int itemsInQueue = neededItems.Where(x => x == kvp.Key).Count();
            if (itemsInQueue < kvp.Value) {
                for (int i = itemsInQueue; i < kvp.Value; i++) {
                    neededItems.Add(kvp.Key);
                    //GD.Print(string.Format("'{0}' added item '{1}' to buy queue.", Name, kvp.Key.itemName));
                }
                itemsAdded = true;
            }
        }
        //neededItems.ForEach(x => GD.Print(x.itemName));
        return itemsAdded;
    }
    public bool IsDead() {
        return stats.isDead;
    }
    public string GetProfession() {
        return stats?.profession;
    }
    public void SetProfession(string profession) {
        aggressive = Constants.AGGRESSIVE_PROFESSIONS.Contains(profession);
        stats.profession = profession;
    }

    public override void CheckNeeds() {

        if (stats.hunger < stats.maxHunger/2) {
            Eat();
        }
        /*
        else {
            neededItems.RemoveAll(x => x is ConsumableItem && ((ConsumableItem)x).nutritionValue > 0); // If not hungry, don't buy more food. HANDLED NOW IN NPCTRADESTATE
        }
        */
        if (stats.commodities < stats.maxCommodities/2) {
            Consooom();
        }
        /*
        else {
            neededItems.RemoveAll(x => x is ConsumableItem && ((ConsumableItem)x).commodityValue > 0); // If no need for commodities, don't buy more commodities.
        }
        */
    }
    protected void Eat() {
        ConsumableItem bestFood = inventory.GetEdibleItems()?.Last();
        if(bestFood?.nutritionValue > 0) {
            stats.RaiseHealth(bestFood.healValue);
            stats.RaiseHunger(bestFood.nutritionValue);
            inventory.RemoveItem(bestFood);
        } else {
            GetFood();
        }
    }

    private void Consooom() {
        ConsumableItem bestConsumable = inventory.GetCommodityItems()?.Last();
        if(bestConsumable?.commodityValue > 0) {
            stats.RaiseCommodities(bestConsumable.commodityValue);
            inventory.RemoveItem(bestConsumable);
        } else {
            GetCommodities();
        }
    }
    

    // Virtual Functions

    public virtual List<Item> GetSellQueue() {
        return new List<Item>();
    }
    public virtual void AddToSellQueue(Item item) {
        return;
    }
    public virtual void PopFromSellQueue(Item item) {
        return;
    }
    public virtual void PopFromBuyQueue(Item item) {
        return;
    }

    public virtual void GetFood() {
        return;
    }
    public virtual void GetCommodities() {
        return;
    }

    // GD

    public override void _Ready()
    {
        base._Ready();
        dialogue = new CharacterDialogue(this);

        if (stats == null) {
            stats = (Stats)ResourceLoader.Load(Constants.DEF_STATS).Duplicate();
        } else {
            stats.UpdateStats();
        }

        portrait = (Texture)ResourceLoader.Load(portraitResource);
        damageCounter = (PackedScene)ResourceLoader.Load("res://assets/combat/damagecounter.tscn");

        if (Constants.AGGRESSIVE_PROFESSIONS.Contains(GetProfession())) {
            aggressive = true;
        }

        this.Connect("mouse_entered", this, nameof(_OnMouseOver));
        this.Connect("mouse_exited", this, nameof(_OnMouseExit));
        this.Connect("input_event", this, nameof(_OnClickEvent));
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