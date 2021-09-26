using Godot;
using System.Collections.Generic;

public class Tooltip : Popup {
    private Label itemName;
    private Label itemDescription;
    private List<Label[]> stats = new List<Label[]>();

    public override void _Ready()
    {
        itemName = (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/itemName");
        itemDescription = (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/itemDescription");
        stats.Add(new Label[] {
            (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/Stat1/Stat"),
            (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/Stat1/Difference")
            });
        stats.Add(new Label[] {
            (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/Stat2/Stat"),
            (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/Stat2/Difference")
            });
        stats.Add(new Label[] {
            (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/Stat2/Stat"),
            (Label)GetNode("NinePatchRect/MarginContainer/VBoxContainer/Stat2/Difference")
            });

    }

    public void GetTooltip(Item item) {
        itemName.Text = item.itemName;
        itemDescription.Text = item.itemDescription;
        foreach(Label[] i in stats) {
            i[0].Text = "Test";
            i[1].Text = "(Test)";
        }
    }
}

