using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedDroneStrike : BaseAttack
{
    public TargetedDroneStrike()
    {
        attackName = "Targeted Drone Strike";
        attackDescription = "Identifies target and uses precision drone strike to deal damage.";
        attackDamage = 20f;
        attackCost = 0;
    }
}
