using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseShield : SkillAttack
{
    public PulseShield()
    {
        attackName = "Pulse Shield";
        attackDescription = "Fires an electromagnetic pulse which damages enemies while defending yourself.";
        attackDamage = 40f;
        attackCost = 50f;
        effect = new BuffEffect(BaseEffect.EffectStatType.DEF, 2f);
    }
}
