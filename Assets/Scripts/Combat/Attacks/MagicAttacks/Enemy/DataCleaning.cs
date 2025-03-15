using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCleaning : SkillAttack
{
    public DataCleaning()
    {
        attackName = "Data Cleaning";
        attackDescription = "Cleans training data to receive more accurate results, defenses are higher.";
        attackDamage = 70f;
        attackCost = 100f;
        effect = new BuffEffect(BaseEffect.EffectStatType.DEF, 1f);
    }
}
