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

    protected int requiredActions = Constants.DEF_REQUIREDACTIONS;
    protected int currentActions = 0;
    protected Sprite sprite;
    protected string[] actions;

    protected string exhaustedTexture;
    protected string exhaustedPortrait;
    protected string exhaustedName;
    protected string exhaustedDescription;
    protected bool isExhausted = false;
    protected List<Character> workers = new List<Character>();


    public List<Character> GetWorkers() {
        return workers;
    }
    public void AddWorker(Character worker) {
        workers.Add(worker);
    }
    public void RemoveWorker(Character worker) {
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
        //QueueFree();
    }

    public override void _Ready()
    {
        if (sprite == null) {
            sprite = (Sprite)GetNode("Sprite");
        }
        portrait = (Texture)ResourceLoader.Load(portraitResource);
        this.Connect("mouse_entered", this, nameof(_OnMouseOver));
        this.Connect("mouse_exited", this, nameof(_OnMouseExit));
        
        base._Ready();
    }

    public virtual void _OnMouseOver() {
        EmitSignal("OnMouseOver", this);
    }
    public virtual void _OnMouseExit() {
        EmitSignal("OnMouseExit");
    }
}