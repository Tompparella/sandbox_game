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

        List<Guardpost> guardPosts = new List<Guardpost>();     // We'll find the settlement guardposts here and assign them to barracks evenly.

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
                case Guardpost guardPost:
                    guardPosts.Add(guardPost);
                    GD.Print(string.Format("{0} found.", guardPost.entityName));
                    break;
                default:
                    break;
            }
        }
        AssignGuardPostsToBarracks(guardPosts);
    }

    private void AssignGuardPostsToBarracks(List<Guardpost> guardPosts) {
        if (guardPosts.Any() && settlementBarracks.Any()) {
            int guardPostsPerBarracks = guardPosts.Count() / settlementBarracks.Count();
            foreach (Barracks barracks in settlementBarracks)
            {
                for (int i = 0; i < guardPostsPerBarracks; i++) {
                    if (guardPosts.Any()) {
                        Guardpost currentPost = guardPosts.First();
                        barracks.AddGuardPost(currentPost);
                        guardPosts.RemoveAt(0);
                    }
                }
                if (barracks.Equals(settlementBarracks.Last()) && guardPosts.Any()) {
                    guardPosts.ForEach(x => barracks.AddGuardPost(x));    // If there's still unassigned guardposts, assign those to the last barracks in the list.
                }
                barracks.Initialize();
            }
        }
    }
}