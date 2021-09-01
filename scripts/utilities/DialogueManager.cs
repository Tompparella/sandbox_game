using Godot;
using System;

public class DialogueManager : Control
{
    private Popup popup;
    private RichTextLabel dialogue;
    private Label name;
    private TextureRect portrait;
    private Timer timer;
    private Character target;
    
    public override void _Ready()
    {
        this.SetProcess(false);
        timer = (Timer)GetNode("Timer");
        popup = (Popup)GetNode("Popup");
        dialogue = (RichTextLabel)popup.GetNode("Dialogue");
        name = (Label)popup.GetNode("Name");
        portrait = (TextureRect)popup.GetNode("Portrait");
    }

    public override void _Process(float delta)
    {
        this.RectPosition = target.Position;
    }

    public void ShowDialogueBox(Character source) {
        target = source;
        StopTimer();
        this.SetProcess(true);
        popup.Visible = true;
        popup.Popup_();
        name.Text = target.Name;
        dialogue.Text = "I am a regular placeholder character. My purpose is to serve as a lifeless shell for some future asset that will eventually become meaningless in the greater scheme of things. My existence is pointless.";
    }

    public void CloseDialogueBox() {
    //    timer.Start(1);
    }

    public void HidePopup() {
        this.SetProcess(false);
    }

    public void StopTimer() {
        timer.Stop();
    }
    public void _Timeout() {
        dialogue.Text.Empty();
        name.Text.Empty();
        popup.Visible = false;
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
    }
}
