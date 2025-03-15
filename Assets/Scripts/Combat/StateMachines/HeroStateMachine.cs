using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseHero hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState currState;

    // For the ProgressBar
    // private float cur_cooldown = 0f;
    // private float max_cooldown = 5f;
    // private Image progressBar;


    private Image healthBar, magicBar;

    // public GameObject Selector;

    // IEnumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10f;

    // For Dead State
    private bool isAlive = true;

    private float variance = 0.1f; // 10% variance (10% below, 10% above)
    private float increaseMp = 0.15f; // 15% of base MP

    // For Hero Panel
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    public GameObject damageCanvas;

    public List<BaseEffect> baseEffects = new List<BaseEffect>();
    private List<GameObject> effectIcons = new List<GameObject>();

    void Start()
    {
        // find spacer, make connection to it
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");

        // create panel, fill in info
        CreateHeroPanel();

        // cur_cooldown = Random.Range(0, 2.5f);
        // Selector.SetActive(false);
        // currState = TurnState.PROCESSING;

        // actionStarted = false;
        // isAlive = true;

        currState = TurnState.ADDTOLIST;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;

        UpdateHeroPanel();

        damageCanvas.SetActive(false);
    }

    void Update()
    {
        // Debug.Log(currState);
        switch (currState)
        {
            case TurnState.PROCESSING:
                {
                    // UpgradeProgressBar();
                    break;
                }
            case TurnState.ADDTOLIST:
                {
                    UpdateEffectsList();
                    BSM.HeroesToManage.Add(this.gameObject);
                    BSM.HeroInput = BattleStateMachine.HeroGUI.ACTIVATE;

                    currState = TurnState.WAITING;

                    break;
                }
            // Idle State
            case TurnState.WAITING:
                {
                    if (BSM.isAutoFight && BSM.PerformList.Count == 0)
                    {
                        StartCoroutine(AutoBattle());
                        currState = TurnState.SELECTING;
                    }
                    break;
                }
            // Idle State for Auto Battle
            case TurnState.SELECTING:
                {
                    break;
                }
            case TurnState.ACTION:
                {
                    StartCoroutine(TimeForAction());
                    break;
                }
            case TurnState.DEAD:
                {
                    if (!isAlive)
                    {
                        return;
                    }
                    else
                    {
                        // change tag
                        this.gameObject.tag = "deadHero";

                        // not attackable by any enemy
                        BSM.HeroesInBattle.Remove(this.gameObject);

                        // not managable by player
                        BSM.HeroesToManage.Remove(this.gameObject);

                        // deactivate selector
                        // Selector.SetActive(false);

                        // reset gui
                        BSM.ActionPanel.SetActive(false);
                        BSM.EnemySelectPanel.SetActive(false);

                        // remove item from performList
                        if (BSM.HeroesInBattle.Count > 0)
                        {
                            for (int i = 0; i < BSM.PerformList.Count; i++)
                            {
                                if (i != 0)
                                {
                                    if (BSM.PerformList[i].AttackersGameObject == this.gameObject)
                                    {
                                        BSM.PerformList.Remove(BSM.PerformList[i]);
                                    }

                                    if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                                    {
                                        BSM.PerformList[i].AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];
                                    }
                                }
                            }
                        }

                        isAlive = false;
                    }

                    break;
                }
        }
    }

    private IEnumerator AutoBattle()
    {
        // create list for skills with non zero mana points 
        // (one for skill attack and healing skill)
        List<SkillAttack> manaAttacks = new List<SkillAttack>();
        List<HealingSkill> healingSkills = new List<HealingSkill>();

        List<SkillAttack> buffAtkSkills = new List<SkillAttack>();
        List<SkillAttack> buffDefSkills = new List<SkillAttack>();
        List<SkillAttack> debuffAtkSkills = new List<SkillAttack>();
        List<SkillAttack> debuffDefSkills = new List<SkillAttack>();

        List<GameObject> enemies = new List<GameObject>();

        bool isDebuffAtk;
        bool isDebuffDef;

        // filter skills with minimal
        manaAttacks.AddRange(hero.SpecialSkills.FindAll(
            delegate (SkillAttack sk)
            {
                return sk.attackCost <= hero.currMp;
            }
        ));
        manaAttacks = manaAttacks.OrderByDescending(h => h.attackDamage).ThenBy(h => h.attackName).ToList();

        buffAtkSkills.AddRange(manaAttacks.FindAll(
            delegate (SkillAttack sk)
            {
                return sk.effect.effectType == BaseEffect.EffectType.BUFF && sk.effect.effectStatType == BaseEffect.EffectStatType.ATK;
            }
        ));
        buffAtkSkills = buffAtkSkills.OrderByDescending(h => h.attackDamage).ThenBy(h => h.attackName).ToList();

        buffDefSkills.AddRange(manaAttacks.FindAll(
            delegate (SkillAttack sk)
            {
                return sk.effect.effectType == BaseEffect.EffectType.BUFF && sk.effect.effectStatType == BaseEffect.EffectStatType.DEF;
            }
        ));
        buffDefSkills = buffDefSkills.OrderByDescending(h => h.attackDamage).ThenBy(h => h.attackName).ToList();

        debuffAtkSkills.AddRange(manaAttacks.FindAll(
            delegate (SkillAttack sk)
            {
                return sk.effect.effectType == BaseEffect.EffectType.DEBUFF && sk.effect.effectStatType == BaseEffect.EffectStatType.ATK;
            }
        ));
        debuffAtkSkills = debuffAtkSkills.OrderByDescending(h => h.attackDamage).ThenBy(h => h.attackName).ToList();

        debuffDefSkills.AddRange(manaAttacks.FindAll(
            delegate (SkillAttack sk)
            {
                return sk.effect.effectType == BaseEffect.EffectType.DEBUFF && sk.effect.effectStatType == BaseEffect.EffectStatType.DEF;
            }
        ));
        debuffDefSkills = debuffDefSkills.OrderByDescending(h => h.attackDamage).ThenBy(h => h.attackName).ToList();

        healingSkills.AddRange(hero.HealingSkills.FindAll(
            delegate (HealingSkill sk)
            {
                return sk.attackCost <= hero.currMp;
            }
        ));
        healingSkills = healingSkills.OrderByDescending(h => h.attackDamage).ThenBy(h => h.attackName).ToList();


        foreach (GameObject enemyGameObject in BSM.EnemiesInBattle)
        {
            enemies.Add(enemyGameObject);
        }
        enemies = enemies.OrderBy(e => e.GetComponent<EnemyStateMachine>().enemy.currHp).ToList();

        isDebuffAtk = baseEffects.Find(
            delegate (BaseEffect e)
            {
                return e.effectStatType == BaseEffect.EffectStatType.ATK && e.effectType == BaseEffect.EffectType.DEBUFF;
            }
        ) != null;

        isDebuffDef = baseEffects.Find(
            delegate (BaseEffect e)
            {
                return e.effectStatType == BaseEffect.EffectStatType.DEF && e.effectType == BaseEffect.EffectType.DEBUFF;
            }
        ) != null;

        // if hp <= 50%
        if ((float)hero.currHp / hero.baseHp <= 0.5)
        {
            // heal
            if (healingSkills.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
                BSM.Input5();

            }
            // if not possible, increase def
            else
            {
                if (buffDefSkills.Count > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input3();

                    yield return new WaitForSeconds(0.5f);
                    BSM.Input4(buffDefSkills[0]);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input1();
                }
                yield return new WaitForSeconds(0.5f);
                BSM.Input2(enemies[0]);
            }
        }
        // if debuff atk
        // check if possible to buff atk
        else if (isDebuffAtk)
        {
            if (buffAtkSkills.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
                BSM.Input3();

                yield return new WaitForSeconds(0.5f);
                BSM.Input4(buffAtkSkills[0]);

                yield return new WaitForSeconds(0.5f);
                BSM.Input2(enemies[0]);
            }
            else
            {
                if (manaAttacks.Count > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input3();
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input4(manaAttacks[0]);
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input2(enemies[0]);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input1();
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input2(enemies[0]);
                }
            }
        }
        // if debuff def
        // check if possible to buff def
        else if (isDebuffDef)
        {
            if (buffDefSkills.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
                BSM.Input3();
                yield return new WaitForSeconds(0.5f);
                BSM.Input4(buffDefSkills[0]);
                yield return new WaitForSeconds(0.5f);
                BSM.Input2(enemies[0]);
            }
            else
            {
                if (manaAttacks.Count > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input3();
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input4(manaAttacks[0]);
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input2(enemies[0]);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input1();
                    yield return new WaitForSeconds(0.5f);
                    BSM.Input2(enemies[0]);
                }
            }
        }
        // else 
        // select mana atk with highest dmg
        // if no possible mana atk, normal atk
        else
        {
            if (manaAttacks.Count > 0)
            {
                yield return new WaitForSeconds(0.5f);
                BSM.Input3();
                yield return new WaitForSeconds(0.5f);
                BSM.Input4(manaAttacks[0]);
                yield return new WaitForSeconds(0.5f);
                BSM.Input2(enemies[0]);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                BSM.Input1();
                yield return new WaitForSeconds(0.5f);
                BSM.Input2(enemies[0]);
            }
        }

        Debug.Log("auto battle turn done");

        // end coroutine
        yield break;
    }

    // void UpgradeProgressBar()
    // {
    //     cur_cooldown = cur_cooldown + Time.deltaTime;
    //     float calc_cooldown = cur_cooldown / max_cooldown;
    //     progressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

    //     if (cur_cooldown >= max_cooldown)
    //     {
    //         currState = TurnState.ADDTOLIST;
    //     }
    // }

    void UpgradeHealthBar()
    {
        float calc_health = hero.currHp / hero.baseHp;
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(calc_health, 0, 1), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    void UpgradeMagicBar()
    {
        float calc_magic = hero.currMp / hero.baseMp;
        magicBar.transform.localScale = new Vector3(Mathf.Clamp(calc_magic, 0, 1), magicBar.transform.localScale.y, magicBar.transform.localScale.z);
    }

    public void EnableDeadView()
    {
        if (currState == TurnState.DEAD)
        {
            damageCanvas.SetActive(false);
            stats.effectSpacer.gameObject.SetActive(false);

            // change color of gameobject and/or play animation
            // this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
            this.gameObject.SetActive(false);
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnDead);

            // reset heroInput
            BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
        }
    }

    private IEnumerator TimeForAction()
    {
        // Debug.Log(actionStarted);
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        if (EnemyToAttack != null)
        {
            // animate hero near the enemy to attack
            Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z - 1.5f);

            // Debug.Log("action starting");

            // do kick animation
            

            while (MoveTowardsEnemy(enemyPosition))
            {
                yield return null;
            }

            

            // Debug.Log("action starting 1");

            // wait
            yield return new WaitForSeconds(0.5f);

            // Debug.Log("action starting 2");

            // do damage
            DoDamage();

            // magic point increase
            hero.currMp += (increaseMp * hero.baseMp);
            if (hero.currMp > hero.baseMp)
            {
                hero.currMp = hero.baseMp;
            }

            UpdateHeroPanel();

            // animate hero back to start position
            Vector3 firstPosition = startPosition;
            while (MoveTowardsStart(firstPosition))
            {
                yield return null;
            }
        }
        else
        {
            float addHealth = BSM.PerformList[0].chosenAttack.attackDamage * hero.baseHp;

            // increase health
            hero.currHp += addHealth;

            if (hero.currHp >= hero.baseHp)
            {
                hero.currHp = hero.baseHp;
            }

            hero.currMp -= BSM.PerformList[0].chosenAttack.attackCost;

            // magic point increase
            hero.currMp += (increaseMp * hero.baseMp);
            if (hero.currMp > hero.baseMp)
            {
                hero.currMp = hero.baseMp;
            }

            // when healing remove all debuffs
            RemoveDebuffs();

            UpdateHeroPanel();

            damageCanvas.transform.Find("DamageTxt").GetComponent<TMP_Text>().text = "+" + Mathf.Round(addHealth).ToString();
            damageCanvas.SetActive(true);
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnHeal);
            this.gameObject.GetComponent<Animation>().Play("DamageAnimation");
            yield return new WaitForSeconds(1f);
        }

        // BSM.RefreshActionPanel();

        // remove performer from the list in BSM
        BSM.PerformList.RemoveAt(0);

        if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
            // reset BSM to wait
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

            // reset this enemy state
            // cur_cooldown = 0f;
            currState = TurnState.PROCESSING;

            // enemies' turn to attack
            BSM.EnemiesAttack();
        }
        else
        {
            currState = TurnState.WAITING;
        }

        // end coroutine
        actionStarted = false;
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        // Debug.Log("Update -  curPos:" + transform.position + " target: " + target + "dist: " + Vector3.Distance(transform.position, target) + " speed: " + animSpeed + " dt: " + Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime);
        return target != transform.position;
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime);
        return target != transform.position;
    }

    public void TakeDamage(float getDamageAmount)
    {
        hero.currHp -= getDamageAmount;

        damageCanvas.transform.Find("DamageTxt").GetComponent<TMP_Text>().text = "-" + Mathf.Round(getDamageAmount).ToString();
        damageCanvas.SetActive(true);

        this.gameObject.GetComponent<Animation>().Play("DamageAnimation");
        if (BSM.PerformList[0].chosenAttack is SkillAttack)
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnSkillAtk);
        else
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnMeleeAtk);

        if (hero.currHp <= 0)
        {
            hero.currHp = 0;
            currState = TurnState.DEAD;
        }

        UpdateHeroPanel();
    }

    // do damage
    void DoDamage()
    {
        float calc_damage = hero.currAtk + BSM.PerformList[0].chosenAttack.attackDamage;

        EnemyStateMachine targetESM = EnemyToAttack.GetComponent<EnemyStateMachine>();
        float currDef = targetESM.enemy.currDef;

        // apply damage formula
        if (calc_damage >= currDef)
        {
            calc_damage = calc_damage * 2 - currDef;
        }
        else
        {
            calc_damage = calc_damage * calc_damage / currDef;
        }
        // apply variance
        calc_damage *= (1 + Random.Range(-variance, variance));

        // // factor in buff/debuff atk
        // BaseEffect effectAtk = baseEffects.Find(
        //     delegate (BaseEffect e)
        //     {
        //         return e.effectStatType == BaseEffect.EffectStatType.ATK;
        //     }
        // );

        // if (effectAtk != null)
        // {
        //     if (effectAtk.effectType == BaseEffect.EffectType.BUFF)
        //     {
        //         calc_damage *= (1 + effectAtk.effectPercentage);
        //     }
        //     else
        //     {
        //         calc_damage *= (1 - effectAtk.effectPercentage);
        //     }
        // }

        // // factor in buff/debuff def
        // BaseEffect effectDef = baseEffects.Find(
        //     delegate (BaseEffect e)
        //     {
        //         return e.effectStatType == BaseEffect.EffectStatType.DEF;
        //     }
        // );

        // if (effectDef != null)
        // {
        //     if (effectDef.effectType == BaseEffect.EffectType.BUFF)
        //     {
        //         calc_damage *= (1 - effectDef.effectPercentage);
        //     }
        //     else
        //     {
        //         calc_damage *= (1 + effectDef.effectPercentage);
        //     }
        // }

        // decrease MP
        hero.currMp -= BSM.PerformList[0].chosenAttack.attackCost;

        targetESM.TakeDamage(calc_damage);

        if (BSM.PerformList[0].chosenAttack is SkillAttack)
        {
            SkillAttack sk = hero.SpecialSkills.Find(
                delegate (SkillAttack s)
            {
                return s.attackName.CompareTo(BSM.PerformList[0].chosenAttack.attackName) == 0;
            }
            );

            if (sk.effect.effectType == BaseEffect.EffectType.DEBUFF)
            {
                targetESM.TakeEffect(sk.effect);
            }
            else
            {
                TakeEffect(sk.effect);
            }
        }

        UpdateHeroPanel();
    }

    public void TakeEffect(BaseEffect effect)
    {
        // find effect that can be countered e.g. buff with new effect debuff and vice versa
        BaseEffect counterEffect = baseEffects.Find(
            delegate (BaseEffect e)
            {
                return (e.effectStatType == effect.effectStatType && e.effectType == BaseEffect.EffectType.DEBUFF && effect.effectType == BaseEffect.EffectType.BUFF) || (e.effectStatType == effect.effectStatType && e.effectType == BaseEffect.EffectType.BUFF && effect.effectType == BaseEffect.EffectType.DEBUFF);
            }
        );

        // find effect with same stat type and same type
        BaseEffect sameTypeEffect = baseEffects.Find(
            delegate (BaseEffect e)
            {
                return e.effectStatType == effect.effectStatType && e.effectType == effect.effectType;
            }
        );

        // if exist effect that can be countered
        if (counterEffect != null)
        {
            // reverse stat effects
            ReverseEffectToStats(counterEffect);
        }
        // if exist same effect
        else if (sameTypeEffect != null)
        {
            // update turn duration
            sameTypeEffect.durationTurns += effect.durationTurns;
        }
        // otherwise
        else
        {
            // apply effects to stats
            ApplyEffectToStats(effect);
        }
    }

    public void RemoveDebuffs()
    {
        // find debuff effects
        List<BaseEffect> debuffEffects = baseEffects.FindAll(
            delegate (BaseEffect e)
            {
                return e.effectType == BaseEffect.EffectType.DEBUFF;
            }
        );

        for (int i = debuffEffects.Count - 1; i >= 0; i--)
        {
            ReverseEffectToStats(debuffEffects[i]);
        }
    }

    private void ApplyEffectToStats(BaseEffect effect)
    {
        GameObject effectIcon;
        BaseEffect newEffect;

        if (effect.effectType == BaseEffect.EffectType.BUFF)
        {
            newEffect = new BuffEffect(effect.effectStatType, effect.effectPercentage);

            if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
            {
                effectIcon = Instantiate(BSM.BuffAtkIcon) as GameObject;
                effectIcon.name = "BuffAtkIcon";
            }
            else
            {
                effectIcon = Instantiate(BSM.BuffDefIcon) as GameObject;
                effectIcon.name = "BuffDefIcon";
            }
        }
        else
        {
            newEffect = new DebuffEffect(effect.effectStatType, effect.effectPercentage);

            if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
            {
                effectIcon = Instantiate(BSM.DebuffAtkIcon) as GameObject;
                effectIcon.name = "DebuffAtkIcon";
            }
            else
            {
                effectIcon = Instantiate(BSM.DebuffDefIcon) as GameObject;
                effectIcon.name = "DebuffDefIcon";
            }
        }

        Transform effectSpacer = stats.effectSpacer;
        effectIcon.transform.SetParent(effectSpacer, false);
        effectIcons.Add(effectIcon);

        // add effect
        baseEffects.Add(newEffect);

        // apply effects to stats
        if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                hero.currAtk += (hero.baseAtk * effect.effectPercentage);
            }
            else
            {
                hero.currAtk -= (hero.baseAtk * effect.effectPercentage);
            }
        }
        else
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                hero.currDef += (hero.baseDef * effect.effectPercentage);
            }
            else
            {
                hero.currDef -= (hero.baseDef * effect.effectPercentage);
            }
        }
    }

    private void ReverseEffectToStats(BaseEffect effect)
    {
        // reverse stat effects
        if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                hero.currAtk -= (hero.baseAtk * effect.effectPercentage);
            }
            else
            {
                hero.currAtk += (hero.baseAtk * effect.effectPercentage);
            }
        }
        else
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                hero.currDef -= (hero.baseDef * effect.effectPercentage);
            }
            else
            {
                hero.currDef += (hero.baseDef * effect.effectPercentage);
            }
        }

        string name;

        if (effect.effectType == BaseEffect.EffectType.BUFF)
        {
            if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
            {
                name = "BuffAtkIcon";
            }
            else
            {
                name = "BuffDefIcon";
            }
        }
        else
        {
            if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
            {
                name = "DebuffAtkIcon";
            }
            else
            {
                name = "DebuffDefIcon";
            }
        }

        for (int i = effectIcons.Count - 1; i >= 0; i--)
        {
            if (effectIcons[i].name.CompareTo(name) == 0)
            {
                Destroy(effectIcons[i]);
                effectIcons.RemoveAt(i);
            }
        }

        // remove effect
        baseEffects.Remove(effect);
    }

    private void UpdateEffectsList()
    {
        for (int i = baseEffects.Count - 1; i >= 0; i--)
        {
            baseEffects[i].durationTurns -= 1;
            Debug.Log(baseEffects[i].durationTurns + " turns left");
            if (baseEffects[i].durationTurns <= 0)
            {
                ReverseEffectToStats(baseEffects[i]);
            }
        }
    }

    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.performerName;

        // set hero level
        if (CombatLevelManager.instance.worldNum != 0)
        {
            stats.Level.text = "LV " + CombatLevelManager.instance.worldNum;
        }
        else
        {
            // placeholder
            stats.Level.text = "LV 1";
        }

        // stats.HeroHP.text = "HP: " + hero.currHp;
        // + "/" + hero.baseHp;
        // stats.HeroMP.text = "MP: " + hero.currMp;
        // + "/" + hero.baseMp;

        // progressBar = stats.ProgressBar;
        healthBar = stats.HealthBar;
        magicBar = stats.MagicBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }

    void UpdateHeroPanel()
    {
        Debug.Log("updated hero panel!");
        // stats.HeroHP.text = "HP: " + hero.currHp;
        // + "/" + hero.baseHp;
        UpgradeHealthBar();
        // stats.HeroMP.text = "MP: " + hero.currMp;
        // + "/" + hero.baseMp;
        UpgradeMagicBar();
    }
}
