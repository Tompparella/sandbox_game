using Godot;
using System.Collections.Generic;

public class SettlementInfo : Resource {
    [Export]
    public string settlementName = "Settlement";
    [Export]
    public FactionInfo settlementFaction = new FactionInfo();
    [Export]
    public Dictionary<string, int> jobsAvailable { get; private set; } = new Dictionary<string, int>() {
        // Also needs a data  structure that has max worker values, depending on the amount of resources.
        { Constants.TRADER_PROFESSION,          1   },
        { Constants.FARMER_PROFESSION,          2   },
        { Constants.BAKER_PROFESSION,           1   },
        { Constants.LUMBERJACK_PROFESSION,      1   },
        { Constants.CRAFTSMAN_PROFESSION,       1   },
        { Constants.MINER_PROFESSION,           1   },
        { Constants.BLACKSMITH_PROFESSION,      1   },
        { Constants.LOGISTICSOFFICER_PROFESSION,1   },
        { Constants.SOLDIER_PROFESSION,         3   },
    };

    public Dictionary<string, int> itemDemand { get; private set; } = new Dictionary<string, int>();

    public void WorkerAdded(string profession) {
        if (jobsAvailable.ContainsKey(profession)) {
            jobsAvailable[profession] = jobsAvailable[profession] <= 0 ? 0 : jobsAvailable[profession] -= 1;    // Cannot be negative
        }
    }
    public void WorkerRemoved(string profession) {
        if (jobsAvailable.ContainsKey(profession)) {
            jobsAvailable[profession] += 1;
        }
    }
    /// <summary> Updates settlement prices based on demand. By default, gets called every 60 seconds. </summary>
    public void UpdateItemDemand(KeyValuePair<string,int> item) {
        if (itemDemand.ContainsKey(item.Key)) {
            int newDemand = itemDemand[item.Key] + item.Value;
            itemDemand[item.Key] = newDemand >= 20 ? 20 : newDemand <= -20 ? -20 : newDemand; // Max modifier both ways is 10.
        } else {
            itemDemand.Add(item.Key, item.Value);
        }
    }
}