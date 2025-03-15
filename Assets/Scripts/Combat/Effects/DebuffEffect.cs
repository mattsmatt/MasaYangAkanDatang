using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffEffect : BaseEffect
{
    public DebuffEffect(EffectStatType effectType, float effectPercentage)
    {
        this.effectStatType = effectType;
        this.durationTurns = 3;
        this.effectPercentage = effectPercentage;
        this.effectType = EffectType.DEBUFF;
    }
}
