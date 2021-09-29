public class Constants
{
    public const string DEF_CHARACTERNAME = "Character";
    public const float DEF_HEALTH = 100;
    public const float DEF_MAXPEED = 3;
    public const float DEF_ACCELERATION = 6;
    public const float TICK = 0.4f; // Determines how many seconds one combat tick takes to activate.
    public const int DEF_ATTACKSPEED = 3; // Determines how many combat ticks an attack takes to commence.
    public const int DEF_REQUIREDACTIONS = 5;
    public const float DEF_WORKSPEED = 5;
    public const float DEF_DODGECHANCE = 0.05f;
    public const float DEF_ATTACKRANGE = 100.0f;
    public const float DEF_MINDAMAGE = 1.0f;
    public const float DEF_MAXDAMAGE = 5.0f;
    public const float COMBATESCAPETIME = 10;

    public const string DEF_PORTRAIT = "res://resources/sprites/placeholderCharacter/placeholder_portrait.png";
    public const string TREETRUNK_PORTRAIT = "res://resources/sprites/assets/resources/portrait/tree_trunk_portrait.png";
    public const string DEPOSIT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/ore_deposit_base_portrait.png";
    public const string IRON_DEPOSIT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/ore_deposit_iron_portrait.png";
    public const string SILVER_DEPOSIT_PORTRAIT = "res://resources/sprites/assets/resources/portrait/ore_deposit_silver_portrait.png";
    public const string EMPTYFARM_PORTRAIT = "res://resources/sprites/assets/resources/portrait/wheatfield_empty_portrait.png";


    public const string TREETRUNK_TEXTURE = "res://resources/sprites/assets/resources/full/tree_trunk.png";
    public const string DEPOSIT_TEXTURE = "res://resources/sprites/assets/resources/full/ore_deposit_base.png";
    public const string IRON_DEPOSIT_TEXTURE = "res://resources/sprites/assets/resources/full/ore_deposit_iron.png";
    public const string SILVER_DEPOSIT_TEXTURE = "res://resources/sprites/assets/resources/full/ore_deposit_silver.png";
    public const string EMPTYFARM_TEXTURE = "res://resources/sprites/assets/resources/full/wheatfield_empty.png";


    public const string IRON_DEPOSIT_INVENTORY = "res://resources/items/resources/IronDepositInventory.tres";
    public const string SILVER_DEPOSIT_INVENTORY = "res://resources/items/resources/SilverDepositInventory.tres";
    public const string LUMBER_INVENTORY = "res://resources/items/resources/LumberInventory.tres";
    public const string WHEATFIELD_INVENTORY = "res://resources/items/resources/WheatFieldInventory.tres";

    public static readonly string[] ROOTWORDS = new string[] {"yes", "no", "how", "where"};
    public static readonly string[] SUPPORTWORDS = new string[] {"find", "are", "chop", "work", "mine", "farm", "collect", "harvest"};
    public static readonly string[] OBJECTWORDS = new string[] {"iron sword", "you"};
    public static readonly string[] AFFIRMATIVE = new string[] {"I knew you'd agree.", "I agree!"};
    public static readonly string[] NEGATIVE = new string[] {"That's a shame.", "Dang, screw you too.", "Well ExcuUuSe mEe, pRinceSs!"};
    public static readonly string[] UNKNOWN = new string[] {"Sorry, I couldn't quite catch that.", "What on earth are you talking about?", "Umm, I don't know what you're saying."};
    public static readonly string[] GREETINGS = new string[] {"Well hello there!", "Hey!", "Salutations!"};
    public static readonly string RESOURCETALK = "Why the hell am I talking to a {0}?";

    public static readonly string LUMBER_DESCRIPTION = "It's an exceptionally robust tree. A sufficient source of fresh lumber.";
    public static readonly string TREETRUNK_DESCRIPTION = "It's what's left of a cut down tree.";
    public static readonly string FARM_DESCRIPTION = "A fully grown field of wheat. The lifeblood of civilizations.";
    public static readonly string EMPTYFARM_DESCRIPTION = "A harvested patch of farmland. It should grow back eventually.";
    public static readonly string IRONDEPOSIT_DESCRIPTION = "It's a huge mass of stone with rich veins of Iron. An essential resource for any civilization.";
    public static readonly string SILVERDEPOSIT_DESCRIPTION = "It's a huge mass of stone with clear traces of silver. It's used in almost all vessels that bear magic.";
    public static readonly string EMPTYDEPOSIT_DESCRIPTION = "It's a huge mass of stone. Any traces of ore have already been exploited";

    public static readonly string[] DEPOSITACTIONS = new string[] {"mine", "work"};
    public static readonly string[] LUMBERACTIONS = new string[] {"chop", "work"};
    public static readonly string[] FARMACTIONS = new string[] {"farm", "collect", "harvest", "work"};

    public static readonly string[] PROFESSIONS = new string[] {"farmer", "miner", "lumberjack", null};

    public const string RESOURCECOLOR = "ffa100";
    public const string EMPTYCOLOR = "ffffff";
}
