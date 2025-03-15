using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageData : SkillAttack
{
    public GarbageData()
    {
        attackName = "Garbage Data";
        attackDescription = "Garbage in, garbage out, enemies' attacks are decreased.";
        attackDamage = 80f;
        attackCost = 75f;
        effect = new DebuffEffect(BaseEffect.EffectStatType.ATK, 0.75f);
    }
}
