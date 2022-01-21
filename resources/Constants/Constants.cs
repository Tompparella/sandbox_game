public class Constants
{
    public const string DEF_CHARACTERNAME = "Character";
    public const float DEF_HEALTH = 100;
    public const float DEF_MAXPEED = 360;
    public const float DEF_ACCELERATION = 500;
    public const float TICK = 0.4f; // Determines how many seconds one tick takes to activate.
    public const int DEF_ATTACKSPEED = 3; // Determines how many combat ticks an attack takes to commence.
    public const int DEF_REQUIREDACTIONS = 5; // Default is 5
    //public const int DEF_REQUIREDACTIONS = 0; // Debug.
    public const float DEF_WORKSPEED = 20;
    public const float DEF_DODGECHANCE = 0.05f;
    public const float DEF_ATTACKRANGE = 100.0f;
    public const float DEF_MINDAMAGE = 1.0f;
    public const float DEF_MAXDAMAGE = 5.0f;
    public const float DEF_MAXHUNGER = 100;
    public const float DEF_MINHUNGER = -10;
    public const float DEF_MINCOMMODITIES = -10;
    public const float DEF_MAXCOMMODITIES = 100;
    public const float DEF_TRADEPROFIT = 0.1f; // The amount of profit a trader makes per sale I.e, when buying an item worth 20, they get it for 18, and when selling, they get 22, the trader.
    public const float COMBATESCAPETIME = 10;

    public const string DEF_PORTRAIT = "res://resources/sprites/placeholderCharacter/placeholder_portrait.png";
    public const string TREETRUNK_PORTRAIT = "res://resources/sprites/assets/resources/portrait/tree_trunk_portrait.png";
    public const string TREE_PORTRAIT = "res://resources/sprites/assets/resources/portrait/tree_portrait.png";
    public const string DEPOSIT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/ore_deposit_base_portrait.png";
    public const string IRON_DEPOSIT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/ore_deposit_iron_portrait.png";
    public const string SILVER_DEPOSIT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/ore_deposit_silver_portrait.png";
    public const string FARM_PORTRAIT = "res://resources/sprites/assets/resources/portrait/wheatfield_portrait.png";
    public const string EMPTYFARM_PORTRAIT = "res://resources/sprites/assets/resources/portrait/wheatfield_empty_portrait.png";
    public const string OVEN_PORTRAIT = "res://resources/sprites/assets/resources/portrait/bread_oven_portrait.png";
    public const string WOODCRAFT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/woodcraft_portrait.png";


    public const string TREETRUNK_TEXTURE = "res://resources/sprites/assets/resources/full/tree_trunk.png";
    public const string TREE_TEXTURE = "res://resources/sprites/assets/resources/full/tree.png";
    public const string DEPOSIT_TEXTURE = "res://resources/sprites/assets/resources/full/ore_deposit_base.png";
    public const string IRON_DEPOSIT_TEXTURE = "res://resources/sprites/assets/resources/full/ore_deposit_iron.png";
    public const string SILVER_DEPOSIT_TEXTURE = "res://resources/sprites/assets/resources/full/ore_deposit_silver.png";
    public const string FARM_TEXTURE = "res://resources/sprites/assets/resources/full/wheatfield.png";
    public const string EMPTYFARM_TEXTURE = "res://resources/sprites/assets/resources/full/wheatfield_empty.png";
    public const string OVEN_TEXTURE = "res://resources/sprites/assets/resources/full/bread_oven.png";
    public const string WOODCRAFT_TEXTURE = "res://resources/sprites/assets/resources/full/woodcraft.png";


    public const string DEF_INVENTORY = "res://resources/items/Inventory.tres";
    public const string IRON_DEPOSIT_INVENTORY = "res://resources/items/resources/IronDepositInventory.tres";
    public const string SILVER_DEPOSIT_INVENTORY = "res://resources/items/resources/SilverDepositInventory.tres";
    public const string LUMBER_INVENTORY = "res://resources/items/resources/LumberInventory.tres";
    public const string WHEATFIELD_INVENTORY = "res://resources/items/resources/WheatFieldInventory.tres";


    public const int WHEATPRICE = 15;
    public const int IRONPRICE = 20;
    public const int SILVERPRICE = 25;
    public const int LUMBERPRICE = 10;

    public const string DEF_PROFESSION = "worker";
    public const string LUMBERJACK_PROFESSION = "lumberjack";
    public const string MINER_PROFESSION = "miner";
    public const string TRADER_PROFESSION = "trader";
    public const string FARMER_PROFESSION = "farmer";
    public const string BAKER_PROFESSION = "baker";
    public const string CRAFTSMAN_PROFESSION = "craftsman";
    public const string BLACKSMITH_PROFESSION = "blacksmith";
    public const string LOGISTICSOFFICER_PROFESSION = "logistics_officer";
    public const string SOLDIER_PROFESSION = "soldier";
    public const string HUNTER_PROFESSION = "hunter";
    public const string BANDIT_PROFESSION = "bandit";

    public const string TRADER_GROUP = "traders";
    public const string SOLDIER_GROUP = "soldier";
    public const string LOGISTICS_GROUP = "logistics";
    public const string RESOURCES_GROUP = "resource";
    public const string SETTLEMENT_GROUP = "settlement";
    public const string CHARACTER_GROUP = "character";

    public static readonly string[] ROOTWORDS = new string[] { "yes", "no", "how", "where" };
    public static readonly string[] SUPPORTWORDS = new string[] { "find", "are", "chop", "work", "mine", "farm", "collect", "harvest" };
    public static readonly string[] OBJECTWORDS = new string[] { "iron sword", "you" };
    public static readonly string[] AFFIRMATIVE = new string[] { "I knew you'd agree.", "I agree!" };
    public static readonly string[] NEGATIVE = new string[] { "That's a shame.", "Dang, screw you too.", "Well ExcuUuSe mEe, pRinceSs!" };
    public static readonly string[] UNKNOWN = new string[] { "Sorry, I couldn't quite catch that.", "What on earth are you talking about?", "Umm, I don't know what you're saying." };
    public static readonly string[] GREETINGS = new string[] { "Well hello there!", "Hey!", "Salutations!" };
    public static readonly string RESOURCETALK = "Why the hell am I talking to a {0}?";

    public static readonly string LUMBER_DESCRIPTION = "It's an exceptionally robust tree. A sufficient source of fresh lumber.";
    public static readonly string TREETRUNK_DESCRIPTION = "It's what's left of a cut down tree.";
    public static readonly string FARM_DESCRIPTION = "A fully grown field of wheat. The lifeblood of civilizations.";
    public static readonly string EMPTYFARM_DESCRIPTION = "A harvested patch of farmland. It should grow back eventually.";
    public static readonly string IRONDEPOSIT_DESCRIPTION = "It's a huge mass of stone with rich veins of Iron. An essential resource for any civilization.";
    public static readonly string SILVERDEPOSIT_DESCRIPTION = "It's a huge mass of stone with clear traces of silver. It's used in almost all vessels that bear magic.";
    public static readonly string EMPTYDEPOSIT_DESCRIPTION = "It's a huge mass of stone. Any traces of ore have already been exploited";
    public static readonly string TRADESTALL_DESCRIPTION = "The next best thing to supermarkets.";
    public static readonly string OVEN_DESCRIPTION = "A professional culinarian could make something edible here.";
    public static readonly string WOODCRAFT_DESCRIPTION = "A place where professionals of woodcraft can practice their craft.";
    public static readonly string BLACKSMITH_DESCRIPTION = "Home for the masters of metallurgy.";
    public static readonly string BARRACKS_DESCRIPTION = "A local safehouse for the soldiers of the settlement.";
    public static readonly string GUARDPOST_DESCRIPTION = "Marks the settlement's soldiers to guard the vicinity, and asserts dominance.";
    public static readonly string CAMP_DESCRIPTION = "Someone has made camp here. Friend or foe, who knows.";

    public static readonly string FARM_NAME = "Wheat Field";
    public static readonly string OVEN_NAME = "Bread Oven";
    public static readonly string TREE_NAME = "Tree";
    public static readonly string WOODCRAFT_NAME = "Craftsman's Retreat";
    public static readonly string BLACKSMITH_NAME = "Blacksmith's Shop";
    public static readonly string CAMP_NAME = "Woodland Camp";

    public static readonly string[] FACTIONS = { "Test Faction" };
    public static readonly string[] OUTLAWS = { "Bandits" };

    public static readonly string BANDIT = "res://assets/npc/bandit.tscn";
    public static readonly string NPC = "res://assets/npc/npc.tscn";

    public static readonly string[] DEF_WORKACTIONS = new string[] { "work" };
    public static readonly string[] DEPOSITACTIONS = new string[] { "mine", "work" };
    public static readonly string[] LUMBERACTIONS = new string[] { "chop", "work" };
    public static readonly string[] FARMACTIONS = new string[] { "farm", "collect", "harvest", "work" };
    public static readonly string[] TRADEACTIONS = new string[] { "trade", "sell", "buy" };
    public static readonly string[] OVENACTIONS = new string[] { "bake", "cook", "work" };
    public static readonly string[] CRAFTACTIONS = new string[] { "craft", "work" };
    public static readonly string[] SMITHACTIONS = new string[] { "smith", "work" };

    public static readonly string[] PROFESSIONS = new string[] { FARMER_PROFESSION, MINER_PROFESSION, LUMBERJACK_PROFESSION, CRAFTSMAN_PROFESSION, BLACKSMITH_PROFESSION, LOGISTICSOFFICER_PROFESSION, SOLDIER_PROFESSION, HUNTER_PROFESSION, null };
    public static readonly string[] AGGRESSIVE_PROFESSIONS = new string[] { LOGISTICSOFFICER_PROFESSION, SOLDIER_PROFESSION, HUNTER_PROFESSION, BANDIT_PROFESSION };

    public const string RESOURCECOLOR = "ffa100";
    public const string CONSUMABLECOLOR = "d1e53b";
    public const string EMPTYCOLOR = "ffffff";

    public const string DEF_STATS = "res://resources/stats/Stats.tres";

    public const string FLOURITEM = "res://resources/items/resources/Flour.tres";
    public const string BREADITEM = "res://resources/items/consumables/Bread.tres";
    public const string RATIONITEM = "res://resources/items/consumables/Ration.tres";
    public const string CONSUMERITEM = "res://resources/items/consumables/ConsumerGoods.tres";
    public const string RATIONEDGOODSITEM = "res://resources/items/consumables/RationedGoods.tres";
    public const string SUPPLIESITEM = "res://resources/items/resources/Supplies.tres";
    public const string LOGISTICSITEM = "res://resources/items/logistics/LogisticsItem.tres";
    

}
