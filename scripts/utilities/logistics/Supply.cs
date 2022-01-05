using Godot;

public class Supply : Resource
{
    public Supply()
    {
        supply = supply < 0 ? 0 : supply; // No negative values allowed.
    }

    [Export]
    public float supply = 0;
}
