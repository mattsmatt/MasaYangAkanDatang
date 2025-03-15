using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurityPotion : HealingSkill
{
    public PurityPotion()
    {
        attackName = "Purity Potion";
        attackDescription = "Heals by 75% max hp, removes all debuffs.";
        attackDamage = 0.75f;
        attackCost = 75f;
    }
}
