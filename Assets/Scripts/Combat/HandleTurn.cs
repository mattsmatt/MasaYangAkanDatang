using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{

// test comment

    // name of the attacker
    public string Attacker;

    public string Type;

    // who is attacking
    public GameObject AttackersGameObject;

    // who is attacked
    public GameObject AttackersTarget;

    // which attack is performed
    public BaseAttack chosenAttack;
}
