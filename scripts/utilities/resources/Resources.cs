using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Resources : Interactive
{

    [Signal]
    public delegate void OnMouseOver(Resource resource);
    [Signal]
    public delegate void OnMouseExit();
    public string workerProfession { get; protected set; } = Constants.DEF_PROFESSION;

    protected int requiredActions = Constants.DEF_REQUIREDACTIONS;
    protected int currentActions = 0;
    protected Sprite sprite;
    protected string[] actions;

    [Export]
    protected float refreshTime = 600;

    protected string defaultTexture;
    protected string defaultPortrait;
    protected string defaultName;
    protected string defaultDescription;

    protected string exhaustedTexture;
    protected string exhaustedPortrait;
    protected string exhaustedName;
    protected string exhaustedDescription;
    protected bool isExhausted = false;
    protected List<Character> workers = new List<Character>();
    public int maxWorkers  { get; protected set; } = 3; // Default maximum amount of workers a resource can have.
    public float workRange { get; protected set; } = Constants.DEF_ATTACKRANGE;
    protected string defaultInventory; // The inventory that will be loaded upon refreshing the resource.

    private Timer refreshTimer = new Timer();


    public int GetWorkerNumber() {
        return workers.Count();
    }
    public List<Character> GetWorkers() {
        return workers;
    }
    public virtual bool AddWorker(Character worker) {
        workers.Add(worker);
        return true;
    }
    public virtual void RemoveWorker(Character worker) {
        workers.Remove(worker);
    }
    public bool GetExhausted() {
        return isExhausted;
    }
    public virtual void workAction(Character worker) {
        currentActions++;
        //GD.Print(currentActions);
        if (currentActions >= requiredActions) {
            GiveResource(worker);
            currentActions = 0;
        }
    }

    public virtual void GiveResource(Character worker) {
        if (!isExhausted) {
            if (!worker.inventory.IsFull()) {
                Item givenItem = inventory.PopLastItem();
                worker.AddToSellQueue(givenItem);
                worker.inventory.AddItem(givenItem);
                if (worker.inventory.IsFull()) {
                    worker.SetInteractive();
                    EmitSignal(nameof(OnRemoval), this);
                }
                if (inventory.IsEmpty()) {
                    worker.SetInteractive();
                    ExhaustResource();
                }
            }
        }
    }

    private void ExhaustResource() {
        isExhausted = true;
        EmitSignal(nameof(OnRemoval), this);
        sprite.Texture = (Texture)ResourceLoader.Load(exhaustedTexture);
        portrait = (Texture)ResourceLoader.Load(exhaustedPortrait);
        entityName = exhaustedName;
        dialogue = new ResourceDialogue(this, exhaustedDescription, actions);
        Monitorable = false;
        refreshTimer.Start();       // After resource is exhausted, put on a timer that will eventually refresh its resources.
        //QueueFree();
    }

    private void RefreshResource() {
        inventory = (Inventory)ResourceLoader.Load(defaultInventory).Duplicate();
        isExhausted = false;
        sprite.Texture = (Texture)ResourceLoader.Load(defaultTexture);
        portrait = (Texture)ResourceLoader.Load(defaultPortrait);
        entityName = defaultName;
        dialogue = new ResourceDialogue(this, defaultDescription, actions);
        Monitorable = true;         // Makes it so that after a resource refreshes, the Npc:s working on them will take note.
    }

    public override void _Ready()
    {
        if (sprite == null) {
            sprite = (Sprite)GetNode("Sprite");
        }
        portrait = (Texture)ResourceLoader.Load(portraitResource);
        this.Connect("mouse_entered", this, nameof(_OnMouseOver));
        this.Connect("mouse_exited", this, nameof(_OnMouseExit));

        // Instance the refreshTimer
        refreshTimer.OneShot = true;
        refreshTimer.WaitTime = refreshTime;
        refreshTimer.Connect("timeout", this, nameof(RefreshResource));
        AddChild(refreshTimer);
        
        base._Ready();
    }

    public virtual void _OnMouseOver() {
        EmitSignal("OnMouseOver", this);
    }
    public virtual void _OnMouseExit() {
        EmitSignal("OnMouseExit");
    }
}