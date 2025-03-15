using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack attackToPerform;

    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input4(attackToPerform);
    }
}
