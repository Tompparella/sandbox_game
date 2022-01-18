using Godot;
using System;

public class SettlementInfo : Resource {
    [Export]
    public string settlementName = "Settlement";
    [Export]
    public FactionInfo settlementFaction = new FactionInfo();
}