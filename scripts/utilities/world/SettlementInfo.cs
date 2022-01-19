using Godot;
using System.Collections.Generic;

public class SettlementInfo : Resource {
    [Export]
    public string settlementName = "Settlement";
    [Export]
    public FactionInfo settlementFaction = new FactionInfo();
    [Export]
    public Dictionary<string, int> jobsAvailable = new Dictionary<string, int>() {
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
}