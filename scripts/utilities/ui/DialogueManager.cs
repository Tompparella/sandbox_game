using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class DialogueManager : Control
{

    [Signal]
    public delegate void CallAction(string stateName);
    private Popup popup;
    private RichTextLabel dialogue;
    private Label name;
    private TextureRect portrait;
    private Timer timer;
    private Interactive target;
    private GridContainer stats;
    private LineEdit doInput, sayInput;
    private DialogueHandler dialogueHandler = new DialogueHandler();
    private InventoryDisplay inventoryDisplay;
    
    private void SetStats(Stats statSource) {

        Label str, vit, agi, dxt, def, lbr;

        str = (Label)stats.GetNode("Str/Label");
        vit = (Label)stats.GetNode("Vit/Label");
        agi = (Label)stats.GetNode("Agi/Label");
        dxt = (Label)stats.GetNode("Dxt/Label");
        def = (Label)stats.GetNode("Def/Label");
        lbr = (Label)stats.GetNode("Lbr/Label");

        int[] statList = statSource.GetStats();
        str.Text = "Str: " + statList[0].ToString();
        vit.Text = "Vit: " + statList[1].ToString();
        agi.Text = "Agi: " + statList[2].ToString();
        dxt.Text = "Dxt: " + statList[3].ToString();
        def.Text = "Def: " + statList[4].ToString();
        lbr.Text = "Lbr: " + statList[5].ToString();
    }

    private void ClearStats() {
        
        Label str, vit, agi, dxt, def, lbr;

        str = (Label)stats.GetNode("Str/Label");
        vit = (Label)stats.GetNode("Vit/Label");
        agi = (Label)stats.GetNode("Agi/Label");
        dxt = (Label)stats.GetNode("Dxt/Label");
        def = (Label)stats.GetNode("Def/Label");
        lbr = (Label)stats.GetNode("Lbr/Label");

        str.Text = "Str: " + 0.ToString();
        vit.Text = "Vit: " + 0.ToString();
        agi.Text = "Agi: " + 0.ToString();
        dxt.Text = "Dxt: " + 0.ToString();
        def.Text = "Def: " + 0.ToString();
        lbr.Text = "Lbr: " + 0.ToString();
    }

    private void SetDialogue(String text) {
        dialogue.Text = text;
    }

    public void ShowDialogueBox(Character source = null, Resources resource = null) {
        StopTimer();
        this.SetProcess(true);
        popup.Visible = true;
        popup.Popup_();

        if (source != null) {
            HandleCharacterDialogue(source);
        } else if (resource != null) {
            HandleResourceDialogue(resource);
        }
    }

    public void CloseDialogueBox() {
    //    timer.Start(1);
    }
    private void HandleCharacterDialogue(Character source) {
        target = source;
        portrait.Texture = source.portrait;
        name.Text = source.entityName;
        SetDialogue(source.dialogue.initial);
        inventoryDisplay.UpdateInventory(source.inventory);
        SetStats(source.stats);
    }

    private void HandleResourceDialogue(Resources resource) {
        target = resource;
        portrait.Texture = resource.portrait;
        name.Text = resource.entityName;
        SetDialogue(resource.dialogue.initial);
        inventoryDisplay.UpdateInventory(resource.inventory);
        ClearStats();
    }

    private void UpdateInventory(Inventory inventory) {
        inventoryDisplay.UpdateInventory(inventory);
    }

    public void HidePopup() {
        this.SetProcess(false);
    }

    public void StopTimer() {
        timer.Stop();
    }

    public void OnSayButtonPressed() {
        TakeSayInput(sayInput.Text);
    }
    public void OnDoButtonPressed() {
        TakeDoInput(doInput.Text);
    }

    public void TakeSayInput(string input) {
        sayInput.Clear();
        string response = dialogueHandler.HandleSayInput(input, target.dialogue);
        SetDialogue(response);
    }

    public void TakeDoInput(string input) {
        doInput.Clear();
        string response = dialogueHandler.HandleDoInput(input, target.dialogue);
        switch (response) {
            case null:
                break;
            case "work":
                EmitSignal(nameof(CallAction), "work");
                break;
            default:
                // Set do-information
                break;
        }
    }

    public void _Timeout() {
        dialogue.Text.Empty();
        name.Text.Empty();
        popup.Visible = false;
    }

    public override void _Ready()
    {
        this.SetProcess(false);
        timer = (Timer)GetNode("Timer");
        popup = (Popup)GetNode("Popup");
        dialogue = (RichTextLabel)popup.GetNode("TabContainer/Dialogue/DialogueText");
        name = (Label)popup.GetNode("Name");
        portrait = (TextureRect)popup.GetNode("TabContainer/Dialogue/Portrait");
        stats = (GridContainer)popup.GetNode("TabContainer/Details/Stats");
        sayInput = (LineEdit)popup.GetNode("TabContainer/Dialogue/InputSay");
        doInput = (LineEdit)popup.GetNode("TabContainer/Details/InputDo");
        inventoryDisplay = (InventoryDisplay)popup.GetNode("TabContainer/Details/Inventory");
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
    }
}
