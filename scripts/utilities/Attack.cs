using System;

public class Attack
{
    private const float baseDamage = Constants.DEF_BASEDAMAGE;
    private const float maxDamage = baseDamage * 5;

    public float damage;
    public Character source;
    public Attack(Character _source, float damageModifier = 1) {
        source = _source;
        Random r = new Random();
        damage = r.Next((int)baseDamage, (int)(maxDamage * damageModifier));
    }
}
