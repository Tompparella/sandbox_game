using Godot;
using System;

public class DialogueManager : Control
{
    private Popup popup;
    private RichTextLabel dialogue;
    private Label name;
    private TextureRect portrait;
    public override void _Ready()
    {
        popup = (Popup)GetNode("Popup");
        dialogue = (RichTextLabel)popup.GetNode("Dialogue");
        name = (Label)popup.GetNode("Name");
        portrait = (TextureRect)popup.GetNode("Portrait");
    }
    public void ShowDialogueBox() {
        popup.Popup_();
        dialogue.Text = "I am a regular placeholder character. My purpose is to serve as a lifeless shell for some future asset that will eventually become meaningless in the greater scheme of things. My existence is pointless.";
        name.Text = "Depressed Placeholder Character";
    }

    public void _OnPopupExited() {
        dialogue.Text.Empty();
        name.Text.Empty();
    }
}
