using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TazerGun : BaseAttack
{
    public TazerGun()
    {
        attackName = "Tazer Gun";
        attackDescription = "Shoots an electric force which damages enemies.";
        attackDamage = 50f;
        attackCost = 0;
    }
}
