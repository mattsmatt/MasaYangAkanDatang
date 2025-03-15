using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacking : SkillAttack
{
    public Hacking()
    {
        attackName = "Hacking";
        attackDescription = "Hacks into target's defenses, target's defenses become lower.";
        attackDamage = 40f;
        attackCost = 100f;
        effect = new DebuffEffect(BaseEffect.EffectStatType.DEF, 0.5f);
    }
}
