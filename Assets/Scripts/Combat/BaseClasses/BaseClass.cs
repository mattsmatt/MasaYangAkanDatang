using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string performerName;

    public float baseHp, currHp;
    public float baseMp, currMp;

    public float baseAtk, currAtk;
    public float baseDef, currDef;

    public List<BaseAttack> attacks = new List<BaseAttack>();
    public List<SkillAttack> SpecialSkills = new List<SkillAttack>();
}
