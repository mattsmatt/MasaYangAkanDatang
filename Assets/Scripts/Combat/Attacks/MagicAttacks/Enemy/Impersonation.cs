using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impersonation : SkillAttack
{
    public Impersonation()
    {
        attackName = "Impersonation";
        attackDescription = "Receives additional strength as a result of imitating target.";
        attackDamage = 60f;
        attackCost = 100f;
        effect = new BuffEffect(BaseEffect.EffectStatType.ATK, 1f);
    }
}
