using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    private EnemyStats stats;
    public GameObject damageCanvas;
    public GameObject statsCanvas;

    public Transform effectSpacer;

    public TurnState currState;

    // private float cur_cooldown = 0f;
    // private float max_cooldown = 10f;

    public GameObject Selector;

    // this gameObject
    private Vector3 startPosition;

    // IEnumerator-related for action
    private bool actionStarted = false;
    public GameObject HeroToAttack;
    private float animSpeed = 9f;

    // Alive
    private bool isAlive = true;

    private float variance = 0.1f; // 10% variance (10% below, 10% above)
    private float increaseMp = 0.15f; // 15% of base MP

    public List<BaseEffect> baseEffects = new List<BaseEffect>();
    private List<GameObject> effectIcons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        stats = this.GetComponent<EnemyStats>();

        currState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
        Selector.SetActive(false);

        UpdateEnemyStats();

        damageCanvas.SetActive(false);

        // actionStarted = false;
        // isAlive = true;
    }

    // Update is called once per frame
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
            case TurnState.CHOOSEACTION:
                {
                    ChooseAction();
                    currState = TurnState.WAITING;
                    break;
                }
            // Idle State
            case TurnState.WAITING:
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
                        // change tag of enemy
                        this.gameObject.tag = "deadEnemy";

                        // not attackable by hero
                        BSM.EnemiesInBattle.Remove(this.gameObject);

                        // deactivate selector
                        Selector.SetActive(false);

                        // remove all inputs enemy attacks
                        if (BSM.EnemiesInBattle.Count > 0)
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
                                        BSM.PerformList[i].AttackersTarget = BSM.EnemiesInBattle[Random.Range(0, BSM.EnemiesInBattle.Count)];
                                    }
                                }
                            }
                        }

                        // set alive false
                        isAlive = false;

                        // reset enemyButton
                        BSM.EnemyButtons();
                    }

                    break;
                }
        }
    }

    // void UpgradeProgressBar()
    // {
    //     cur_cooldown = cur_cooldown + Time.deltaTime;

    //     if (cur_cooldown >= max_cooldown)
    //     {
    //         currState = TurnState.CHOOSEACTION;
    //     }
    // }

    void ChooseAction()
    {
        UpdateEffectsList();

        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.performerName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];

        // check if there are special skills
        if (enemy.SpecialSkills.Count > 0)
        {
            // filter list by attack cost
            List<SkillAttack> resultAtk = enemy.SpecialSkills.FindAll(
                delegate (SkillAttack sk)
                {
                    return sk.attackCost <= enemy.currMp;
                }
            );

            // check if filtered list is filled
            if (resultAtk.Count != 0)
            {
                // select random atk
                int num = Random.Range(0, resultAtk.Count);
                myAttack.chosenAttack = resultAtk[num];
            }
            else
            {
                // select normal atk
                myAttack.chosenAttack = enemy.attacks[0];
            }
        }
        else
        {
            // select normal atk
            myAttack.chosenAttack = enemy.attacks[0];
        }

        Debug.Log(this.gameObject.name + " has choosen " + myAttack.chosenAttack.attackName + " and do " + myAttack.chosenAttack.attackDamage + " damage!");

        BSM.CollectActions(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // animate enemy near the hero to attack
        Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z + 1.5f);

        while (MoveTowardsEnemy(heroPosition))
        {
            yield return null;
        }

        // wait
        yield return new WaitForSeconds(0.5f);

        // do damage
        DoDamage();

        // magic point increase
        enemy.currMp += (increaseMp * enemy.baseMp);
        if (enemy.currMp > enemy.baseMp)
        {
            enemy.currMp = enemy.baseMp;
        }

        UpdateEnemyStats();

        // animate enemy back to start position
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition))
        {
            yield return null;
        }

        // wait until current enemy is done
        yield return new WaitForSeconds(0.5f);

        // remove performer from the list in BSM
        BSM.PerformList.RemoveAt(0);
        BSM.CheckEnemysTurn();

        // reset BSM to wait
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        // end coroutine
        actionStarted = false;

        // reset this enemy state
        // cur_cooldown = 0f;
        currState = TurnState.PROCESSING;

        // // heroes' turn to attack
        // BSM.HeroesAttack();
    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime);
        return target != transform.position;
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime);
        return target != transform.position;
    }

    void DoDamage()
    {
        float calc_damage = enemy.currAtk + BSM.PerformList[0].chosenAttack.attackDamage;

        HeroStateMachine targetHSM = HeroToAttack.GetComponent<HeroStateMachine>();
        float currDef = targetHSM.hero.currDef;

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

        // decrease MP
        enemy.currMp -= BSM.PerformList[0].chosenAttack.attackCost;

        targetHSM.TakeDamage(calc_damage);

        if (BSM.PerformList[0].chosenAttack is SkillAttack)
        {
            SkillAttack sk = enemy.SpecialSkills.Find(
                delegate (SkillAttack s)
            {
                return s.attackName.CompareTo(BSM.PerformList[0].chosenAttack.attackName) == 0;
            }
            );

            if (sk.effect.effectType == BaseEffect.EffectType.DEBUFF)
            {

                targetHSM.TakeEffect(sk.effect);
            }
            else
            {
                TakeEffect(sk.effect);
            }
        }

        UpdateEnemyStats();
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.currHp -= getDamageAmount;

        damageCanvas.transform.Find("DamageTxt").GetComponent<TMP_Text>().text = "-" + Mathf.Round(getDamageAmount).ToString();
        damageCanvas.SetActive(true);

        this.gameObject.GetComponent<Animation>().Play("DamageAnimation");
        if (BSM.PerformList[0].chosenAttack is SkillAttack)
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnSkillAtk);
        else
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnMeleeAtk);

        if (enemy.currHp <= 0)
        {
            enemy.currHp = 0;
            currState = TurnState.DEAD;
        }

        UpdateEnemyStats();
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
            Debug.Log("counter effect");
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

        effectIcon.transform.SetParent(effectSpacer, false);
        effectIcons.Add(effectIcon);

        // add effect
        baseEffects.Add(newEffect);

        // apply effects to stats
        if (effect.effectStatType == BaseEffect.EffectStatType.ATK)
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                enemy.currAtk += (enemy.baseAtk * effect.effectPercentage);
            }
            else
            {
                enemy.currAtk -= (enemy.baseAtk * effect.effectPercentage);
            }
        }
        else
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                enemy.currDef += (enemy.baseDef * effect.effectPercentage);
            }
            else
            {
                enemy.currDef -= (enemy.baseDef * effect.effectPercentage);
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
                enemy.currAtk -= (enemy.baseAtk * effect.effectPercentage);
            }
            else
            {
                enemy.currAtk += (enemy.baseAtk * effect.effectPercentage);
            }
        }
        else
        {
            if (effect.effectType == BaseEffect.EffectType.BUFF)
            {
                enemy.currDef -= (enemy.baseDef * effect.effectPercentage);
            }
            else
            {
                enemy.currDef += (enemy.baseDef * effect.effectPercentage);
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

    public void EnableDeadView()
    {
        if (currState == TurnState.DEAD)
        {
            statsCanvas.SetActive(false);
            damageCanvas.SetActive(false);
            effectSpacer.gameObject.SetActive(false);

            // change color of gameobject and/or play animation
            // this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
            this.gameObject.SetActive(false);
            BSM.cgm.audioManager.PlaySFX(BSM.cgm.audioManager.OnDead);

            // check alive
            BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
        }

    }

    void UpgradeHealthBar()
    {
        float calc_health = enemy.currHp / enemy.baseHp;
        stats.HealthBar.transform.localScale = new Vector3(Mathf.Clamp(calc_health, 0, 1), stats.HealthBar.transform.localScale.y, stats.HealthBar.transform.localScale.z);
    }

    void UpgradeMagicBar()
    {
        float calc_magic = enemy.currMp / enemy.baseMp;
        stats.MagicBar.transform.localScale = new Vector3(Mathf.Clamp(calc_magic, 0, 1), stats.MagicBar.transform.localScale.y, stats.MagicBar.transform.localScale.z);
    }

    void UpdateEnemyStats()
    {
        UpgradeHealthBar();
        UpgradeMagicBar();
    }
}
