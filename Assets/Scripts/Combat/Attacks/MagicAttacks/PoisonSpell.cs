using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSpell : SkillAttack
{
    public PoisonSpell()
    {
        attackName = "Poison";
        attackDescription = "Basic poison spell which drags damage over time.";
        attackDamage = 5f;
        attackCost = 50f;
        effect = new DebuffEffect(BaseEffect.EffectStatType.DEF, 0.4f);
    }
}
