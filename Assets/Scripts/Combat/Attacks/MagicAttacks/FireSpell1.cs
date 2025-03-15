using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell1 : SkillAttack
{
    public FireSpell1()
    {
        attackName = "Fire";
        attackDescription = "Basic fire spell which burns nothing.";
        attackDamage = 10f;
        attackCost = 50f;
        effect = new DebuffEffect(BaseEffect.EffectStatType.ATK, 0.25f);
    }
}
