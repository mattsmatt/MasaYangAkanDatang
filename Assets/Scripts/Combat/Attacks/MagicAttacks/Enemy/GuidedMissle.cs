using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissle : SkillAttack
{
    public GuidedMissle()
    {
        attackName = "Guided Missle";
        attackDescription = "Uses AI to identify target's location and strike a heavy blow.";
        attackDamage = 50f;
        attackCost = 100f;
        effect = new DebuffEffect(BaseEffect.EffectStatType.ATK, 0.5f);
    }
}
