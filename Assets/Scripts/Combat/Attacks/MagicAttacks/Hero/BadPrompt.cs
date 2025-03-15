using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadPrompt : SkillAttack
{
    public BadPrompt()
    {
        attackName = "Bad Prompt";
        attackDescription = "Charges AI requests which deals significant damage.";
        attackDamage = 60f;
        attackCost = 50f;
        effect = new BuffEffect(BaseEffect.EffectStatType.ATK, 2f);
    }
}
