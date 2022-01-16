using Godot;
using System.Linq;

public class Supply : Resource
{
    public Supply()
    {
        supply = supply < 0 ? 0 : supply; // No negative values allowed.
    }

    [Export]
    public float supply = 0;

    public void AddSupply(float _supply) {
        supply += _supply;
        CheckSupply();
    }
    public void DecreaseSupply(float _supply = 0.5f) {
        supply -= _supply;
        CheckSupply();
    }

    private void CheckSupply() {
        if (!Enumerable.Range(0, 100).Contains((int)supply))
        {
            supply = supply < 0 ? 0 : 100; // If supply is out of bounds, set it to be either 0 or 100 depending on whether it's over or below the limits.
        }
    }
}
