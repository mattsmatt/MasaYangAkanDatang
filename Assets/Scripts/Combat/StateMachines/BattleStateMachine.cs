using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> HeroesInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        DONE
    }

    public HeroGUI HeroInput;

    public List<GameObject> HeroesToManage = new List<GameObject>();
    private HandleTurn HeroChoice;

    public GameObject enemyButton;
    public Transform Spacer;

    public GameObject ActionPanel;
    public GameObject EnemySelectPanel;
    public GameObject MagicPanel;

    // For Heroes' Attacks
    public Transform actionSpacer;
    public Transform magicSpacer;
    public GameObject actionButton;
    public GameObject magicButton;

    private List<GameObject> atkBtns = new List<GameObject>();

    // enemy buttons
    private List<GameObject> enemyBtns = new List<GameObject>();

    // spawn points
    public List<Transform> enemySpawnPoints = new List<Transform>();
    public List<Transform> heroSpawnPoints = new List<Transform>();

    public bool isAutoFight = false;

    public GameObject DebuffAtkIcon, DebuffDefIcon, BuffAtkIcon, BuffDefIcon;

    public CombatGameManager cgm;

    private GameObject autoFight;

    private void Awake()
    {
        autoFight = GameObject.FindGameObjectWithTag("autoFight");

        cgm = GameObject.Find("CombatGameManager").GetComponent<CombatGameManager>();

        Transform enemyContainer = GameObject.Find("EnemyContainer").transform;
        Transform heroContainer = GameObject.Find("HeroContainer").transform;

        for (int i = 0; i < CombatLevelManager.instance.currentLevel.enemies.Count; i++)
        {
            GameObject newEnemy = Instantiate(CombatLevelManager.instance.currentLevel.enemies[i].enemyObject, enemySpawnPoints[i].position, Quaternion.identity) as GameObject;
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.performerName = CombatLevelManager.instance.currentLevel.enemies[i].enemyName;
            newEnemy.transform.SetParent(enemyContainer);
            newEnemy.transform.rotation = new Quaternion();
        }

        for (int i = 0; i < CombatLevelManager.instance.heroList.Count; i++)
        {
            GameObject newHero = Instantiate(CombatLevelManager.instance.heroList[i], heroSpawnPoints[i].position, Quaternion.identity) as GameObject;
            newHero.name = newHero.GetComponent<HeroStateMachine>().hero.performerName;
            newHero.transform.SetParent(heroContainer);
            newHero.transform.rotation = new Quaternion();

            if (CombatLevelManager.instance.worldNum == 3 && CombatLevelManager.instance.currentLevel.levelNum == 5)
            {
                newHero.transform.Find("Partner").gameObject.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        battleStates = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("enemy"));
        EnemiesInBattle.Sort((e1, e2) => e1.transform.position.x.CompareTo(e2.transform.position.x));
        // foreach (GameObject enemy in EnemiesInBattle)
        // {
        //     Debug.Log(enemy.name);
        // }

        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("hero"));
        HeroInput = HeroGUI.ACTIVATE;

        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        MagicPanel.SetActive(false);

        EnemyButtons();
    }

    public void CheckEnemysTurn()
    {
        if (PerformList.Count == 0)
        {
            // heroes' turn to attack
            HeroesAttack();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):
                {
                    if (PerformList.Count > 0)
                    {
                        battleStates = PerformAction.TAKEACTION;
                    }

                    break;
                }
            case (PerformAction.TAKEACTION):
                {
                    GameObject performer = GameObject.Find(PerformList[0].Attacker);

                    Debug.Log(PerformList[0].Type);
                    if (PerformList[0].Type == "Enemy")
                    {
                        Debug.Log("processing enemy attack");
                        EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();

                        for (int i = 0; i < HeroesInBattle.Count; i++)
                        {
                            if (PerformList[0].AttackersTarget == HeroesInBattle[i])
                            {
                                ESM.HeroToAttack = PerformList[0].AttackersTarget;
                                ESM.currState = EnemyStateMachine.TurnState.ACTION;
                                break;
                            }
                            else
                            {
                                PerformList[0].AttackersTarget = HeroesInBattle[UnityEngine.Random.Range(0, HeroesInBattle.Count)];
                                ESM.HeroToAttack = PerformList[0].AttackersTarget;
                                ESM.currState = EnemyStateMachine.TurnState.ACTION;
                            }
                        }
                    }

                    if (PerformList[0].Type == "Hero")
                    {
                        Debug.Log("Hero is here to perform");
                        HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                        HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                        HSM.currState = HeroStateMachine.TurnState.ACTION;
                    }

                    battleStates = PerformAction.PERFORMACTION;

                    break;
                }
            case (PerformAction.PERFORMACTION):
                {
                    // Idle
                    break;
                }
            case (PerformAction.CHECKALIVE):
                {
                    if (HeroesInBattle.Count < 1)
                    {
                        battleStates = PerformAction.LOSE;
                        // Lose Game
                    }
                    else if (EnemiesInBattle.Count < 1)
                    {
                        battleStates = PerformAction.WIN;
                        //  Win Game
                    }
                    else
                    {
                        ClearActionPanel();
                        // call function

                        HeroInput = HeroGUI.ACTIVATE;
                    }
                    break;
                }
            case (PerformAction.WIN):
                {

                    if (isAutoFight)
                    {
                        SwitchAutoBattle();
                    }

                    for (int i = 0; i < HeroesInBattle.Count; i++)
                    {
                        HeroesInBattle[i].GetComponent<HeroStateMachine>().currState = HeroStateMachine.TurnState.WAITING;
                    }


                    cgm.LevelComplete();
                    break;
                }
            case (PerformAction.LOSE):
                {
                    if (isAutoFight)
                    {
                        SwitchAutoBattle();
                    }

                    cgm.GameOver();
                    break;
                }
        }

        switch (HeroInput)
        {
            case (HeroGUI.ACTIVATE):
                {
                    if (HeroesToManage.Count > 0)
                    {
                        // HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);

                        // Create new HandleTurn instance
                        HeroChoice = new HandleTurn();
                        ActionPanel.SetActive(true);

                        // Populate action buttons
                        CreateAttackButtons();

                        HeroInput = HeroGUI.WAITING;
                    }
                    break;
                }
            // Idle State
            case (HeroGUI.WAITING):
                {
                    break;
                }
            case (HeroGUI.DONE):
                {
                    HeroInputDone();
                    break;
                }
        }
    }

    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }

    public void EnemyButtons()
    {
        // Spacer.transform.GetComponent<VerticalLayoutGroup>().enabled = false;

        // Clean up
        foreach (GameObject enemyBtn in enemyBtns)
        {
            Debug.Log("Success deleted enemy btn");
            Destroy(enemyBtn);
        }
        enemyBtns.Clear();

        // Create Buttons
        foreach (GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine currEnemy = enemy.GetComponent<EnemyStateMachine>();

            // Debug.Log(currEnemy.enemy.name);

            TMP_Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
            buttonText.text = currEnemy.enemy.performerName;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer);
            newButton.transform.localScale = Vector3.one;

            enemyBtns.Add(newButton);
        }

        // Debug.Log("menu success");
        // StartCoroutine(refresh());
    }

    // From the Attack Button
    public void Input1()
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);

        HeroStateMachine currHero = HeroesToManage[0].GetComponent<HeroStateMachine>();
        HeroChoice.Attacker = currHero.hero.performerName;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";
        HeroChoice.chosenAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0];

        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void BackToActionPanelFromEnemyPanel()
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);
        ActionPanel.SetActive(true);
        EnemySelectPanel.SetActive(false);
    }

    public void BackToActionPanelFromSkillsPanel()
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);
        ActionPanel.SetActive(true);
        MagicPanel.SetActive(false);
    }

    // From the Enemy Selection
    public void Input2(GameObject chosenEnemy)
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);
        HeroChoice.AttackersTarget = chosenEnemy;
        HeroInput = HeroGUI.DONE;
    }

    void HeroInputDone()
    {
        PerformList.Add(HeroChoice);

        // Clean the attackPanel
        ClearActionPanel();

        // HeroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        HeroesToManage.RemoveAt(0);
        HeroInput = HeroGUI.WAITING;
    }

    public void EnemiesAttack()
    {
        // StartCoroutine(StartEnemiesAttack());

        foreach (GameObject enemy in EnemiesInBattle)
        {
            enemy.GetComponent<EnemyStateMachine>().currState = EnemyStateMachine.TurnState.CHOOSEACTION;
        }
    }

    // private IEnumerator StartEnemiesAttack()
    // {
    //     foreach (GameObject enemy in EnemiesInBattle)
    //     {
    //         enemy.GetComponent<EnemyStateMachine>().currState = EnemyStateMachine.TurnState.CHOOSEACTION;

    //         // // wait until current enemy is done
    //         // yield return new WaitForSeconds(1.667f);
    //     }
    // }

    public void HeroesAttack()
    {
        foreach (GameObject hero in HeroesInBattle)
        {
            hero.GetComponent<HeroStateMachine>().currState = HeroStateMachine.TurnState.ADDTOLIST;
        }
    }

    void ClearActionPanel()
    {
        EnemySelectPanel.SetActive(false);
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(false);

        foreach (GameObject atkBtn in atkBtns)
        {
            Debug.Log("Success deleted attack btn");
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }

    // Create action buttons
    void CreateAttackButtons()
    {
        GameObject attackButton = Instantiate(actionButton) as GameObject;
        attackButton.name = "AttackButton";
        TMP_Text AttackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        AttackButtonText.text = "Attack";

        attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());

        attackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(attackButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.currMp < HeroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0].attackCost)
        {
            attackButton.GetComponent<Button>().interactable = false;
        }

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        TMP_Text MagicAttackButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        MagicAttackButtonText.text = "Skills";

        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());

        MagicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(MagicAttackButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.SpecialSkills.Count > 0)
        {
            foreach (BaseAttack magicAtk in HeroesToManage[0].GetComponent<HeroStateMachine>().hero.SpecialSkills)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;
                TMP_Text MagicButtonText = MagicButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                MagicButtonText.text = magicAtk.attackName;

                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.attackToPerform = magicAtk;
                MagicButton.transform.SetParent(magicSpacer, false);
                atkBtns.Add(MagicButton);

                if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.currMp < magicAtk.attackCost)
                {
                    MagicButton.GetComponent<Button>().interactable = false;
                }
            }
        }
        else
        {
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }

        GameObject HealButton = Instantiate(actionButton) as GameObject;
        TMP_Text HealButtonText = HealButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        HealButtonText.text = "Purity Potion";
        HealButton.name = "HealButton";

        HealButton.GetComponent<Button>().onClick.AddListener(() => Input5());

        HealButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(HealButton);

        if (HeroesToManage[0].GetComponent<HeroStateMachine>().hero.currMp < HeroesToManage[0].GetComponent<HeroStateMachine>().hero.HealingSkills[0].attackCost)
        {
            HealButton.GetComponent<Button>().interactable = false;
        }
    }

    // Choose healing skill
    public void Input5()
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);
        HeroStateMachine currHero = HeroesToManage[0].GetComponent<HeroStateMachine>();
        HeroChoice.Attacker = currHero.hero.performerName;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";
        HeroChoice.chosenAttack = HeroesToManage[0].GetComponent<HeroStateMachine>().hero.HealingSkills[0];

        ActionPanel.SetActive(false);

        HeroChoice.AttackersTarget = null;
        HeroInput = HeroGUI.DONE;
    }

    // Chosen a magic attack
    public void Input4(BaseAttack chosenMagic)
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);
        HeroStateMachine currHero = HeroesToManage[0].GetComponent<HeroStateMachine>();
        HeroChoice.Attacker = currHero.hero.performerName;
        HeroChoice.AttackersGameObject = HeroesToManage[0];
        HeroChoice.Type = "Hero";
        HeroChoice.chosenAttack = chosenMagic;

        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    // Switching to magic attacks
    public void Input3()
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnClickCombatMenu);
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }

    public void ToggleAutoBattle()
    {
        cgm.audioManager.PlaySFX(cgm.audioManager.OnAutoBattle);
        SwitchAutoBattle();
    }

    private void SwitchAutoBattle()
    {
        isAutoFight = !isAutoFight;
        if (isAutoFight)
        {
            autoFight.GetComponent<Image>().color = new Color32(207, 0, 255, 255);
            autoFight.transform.Find("Text (TMP)").GetComponent<TMP_Text>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            autoFight.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            autoFight.transform.Find("Text (TMP)").GetComponent<TMP_Text>().color = new Color32(50, 50, 50, 255);
        }
    }

    // private IEnumerator refresh()
    // {
    //     yield return new WaitForSeconds(0.01f);
    //     Spacer.transform.GetComponent<VerticalLayoutGroup>().enabled = true;
    // }
}
