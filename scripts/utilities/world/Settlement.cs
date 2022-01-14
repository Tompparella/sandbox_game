using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Settlement : Area2D {
    /*
    Settlement holds information about all important nodes within a town/city/fortress.
    It sends signals to barracks, tradestalls etc. to control its soldiers, prices, etc.
    */

    private List<Barracks> settlementBarracks = new List<Barracks>();
    private List<TradeStall> settlementTradestalls = new List<TradeStall>();

    public void Initiate()  // Called from gamemanager to initiate the factions information.
    {
        Godot.Collections.Array areaEntities = GetOverlappingAreas();

        foreach (Area2D area in areaEntities)
        {
            switch (area)
            {
                case Barracks barracks:
                    settlementBarracks.Add(barracks);
                    GD.Print(string.Format("{0} added to settlement info", barracks.entityName));
                    break;
                case TradeStall tradeStall:
                    settlementTradestalls.Add(tradeStall);
                    GD.Print(string.Format("{0} added to settlement info", tradeStall.entityName));
                    break;
                default:
                    break;
            }
        }
    }
}