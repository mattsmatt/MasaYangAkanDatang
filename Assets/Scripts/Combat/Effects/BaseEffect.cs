using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect
{
    public enum EffectStatType
    {
        ATK,
        DEF
    }

    public enum EffectType
    {
        DEBUFF,
        BUFF
    }

    public float effectPercentage;
    public int durationTurns;

    public EffectStatType effectStatType;
    public EffectType effectType;
}
