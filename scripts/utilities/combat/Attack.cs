using System;

public class Attack
{
    public float damage;
    public Character source;
    public bool isCritical = false;
    public Attack(Character _source) {
        source = _source;
        //Stats stats = source.stats;
        Random r = new Random();
        damage = r.Next((int)source.stats.minDamage, (int)(source.stats.maxDamage));
        isCritical = r.NextDouble() < source.stats.critChance;                       // Critical attack handling. Will be improved.
        damage = isCritical ? damage * 2 : damage;
    }
}