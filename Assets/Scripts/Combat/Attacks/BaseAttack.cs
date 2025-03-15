using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;

    public string attackDescription;

    // base damage
    // example base damage = 15, melee, dependent on level
    // level 10, stamina 35
    // damage = base damage + (level / stamina) or base damage + stamina + level
    public float attackDamage;

    // mana cost
    public float attackCost;
}
