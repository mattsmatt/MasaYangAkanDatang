using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpell : SkillAttack
{
    public FireSpell()
    {
        attackName = "Fire";
        attackDescription = "Basic fire spell which burns nothing.";
        attackDamage = 30f;
        attackCost = 20f;
        effect = new BuffEffect(BaseEffect.EffectStatType.ATK, 0.25f);
    }
}
