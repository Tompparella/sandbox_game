using System;

public class Attack
{
    public float damage;
    public Character source;
    public Attack(Character _source) {
        source = _source;
        Stats stats = source.stats;
        Random r = new Random();
        damage = r.Next((int)stats.minDamage, (int)(stats.maxDamage));
    }
}