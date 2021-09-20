public class Constants
{
    public const string DEF_CHARACTERNAME = "Character";
    public const float DEF_HEALTH = 100;
    public const float DEF_MAXPEED = 3;
    public const float DEF_ACCELERATION = 6;
    public const float TICK = 0.4f; // Determines how many seconds one combat tick takes to activate.
    public const int DEF_ATTACKSPEED = 3; // Determines how many combat ticks an attack takes to commence.
    public const float DEF_ATTACKRANGE = 100.0f;
    public const float DEF_BASEDAMAGE = 5.0f;
    public const float COMBATESCAPETIME = 10;
    public const string DEF_PORTRAIT = "res://resources/sprites/placeholderCharacter/placeholder_portrait.png";
    public static readonly string[] ROOTWORDS = new string[] {"yes", "no", "how", "where"};
    public static readonly string[] SUPPORTWORDS = new string[] {"find", "are", "chop", "work"};
    public static readonly string[] OBJECTWORDS = new string[] {"iron sword", "you"};
    public static readonly string[] AFFIRMATIVE = new string[] {"I knew you'd agree.", "I agree!"};
    public static readonly string[] NEGATIVE = new string[] {"That's a shame.", "Dang, screw you too.", "Well ExcuUuSe mEe, pRinceSs!"};
    public static readonly string[] UNKNOWN = new string[] {"Sorry, I couldn't quite catch that.", "What on earth are you talking about?", "Umm, I don't know what you're saying."};
    public static readonly string[] GREETINGS = new string[] {"Well hello there!", "Heyyy...", "Salutations!"};
    public static readonly string RESOURCETALK = "Why the hell am I talking to a {0}?";
    public static readonly string LUMBERDESCRIPTION = "It's an exceptionally robust tree. A sufficient source of fresh lumber.";
    public static readonly string[] LUMBERACTIONS = new string[] {"chop", "work"};
    

}
