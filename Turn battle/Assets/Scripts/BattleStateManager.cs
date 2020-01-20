using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateManager : MonoBehaviour
{

    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
    }

    public PerformAction battleStates;

    public List<HandleTurn> performList = new List<HandleTurn>();

    public List<GameObject> HerosInBattle = new List<GameObject>();//player
    public List<GameObject> EnemyInBattle = new List<GameObject>();//Enemy


	public enum HeroGuI
	{
		ACTIVATE,
		WAITING,
		INPUT1,//攻撃選択
		INPUT2,//敵選択
		DONE,
	}

	public HeroGuI HeroInput;

	public List<GameObject> HerosToManage = new List<GameObject>();
	private HandleTurn HeroChoise;

	public GameObject enemyButton;
	public Transform Spacer;

    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;
    public GameObject MagicPanel;

    //magic attack
    public Transform actionSpacer;
    public Transform magicSpacer;
    public GameObject actionButton;
    public GameObject magicButton;
    private List<GameObject> atkBtns = new List<GameObject>();
    
    void Start()
    {
        //バトル待機
        battleStates = PerformAction.WAIT;

        //heroとEnemyの数を追加
		HerosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
		EnemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        
        //GUIActive設定
        HeroInput = HeroGuI.ACTIVATE;

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        MagicPanel.SetActive(false);
		EnemyButtons();
	}

    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):
				if(performList.Count > 0)
				{
					battleStates = PerformAction.TAKEACTION;
				}
                break;
            case (PerformAction.TAKEACTION):
				GameObject performer = GameObject.Find(performList[0].Attacker);
				if(performList[0].Type == "Enemy")
				{
					EnemyStateMaschine ESM = performer.GetComponent<EnemyStateMaschine>();
                    for (int i = 0; i < HerosInBattle.Count; i++)
                    {
                        if(performList[0].AttackerTarget == HerosInBattle[i])
                        {
                            ESM.HeroToAttack = performList[0].AttackerTarget;
                            ESM.currnetState = EnemyStateMaschine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            performList[0].AttackerTarget = HerosInBattle[Random.Range(0, HerosInBattle.Count)];
                            ESM.HeroToAttack = performList[0].AttackerTarget;
                            ESM.currnetState = EnemyStateMaschine.TurnState.ACTION;
                        }
                    }
					ESM.HeroToAttack = performList[0].AttackerTarget;
					ESM.currnetState = EnemyStateMaschine.TurnState.ACTION;
				}

				if (performList[0].Type == "Hero")
				{
                    Debug.Log("Hero is hero to perform");
                    HeroStateMaschine HSM = performer.GetComponent<HeroStateMaschine>();
                    HSM.EnemyToAttack = performList[0].AttackerTarget;
                    HSM.currnetState = HeroStateMaschine.TurnState.ACTION;
				}

				battleStates = PerformAction.PERFORMACTION;
				break;
            case (PerformAction.PERFORMACTION):
                break;
        }

        switch (HeroInput)
        {
            case (HeroGuI.ACTIVATE):
                if(HerosToManage.Count > 0)
                {
                    //選択したheroにselector表示
                    HerosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    HeroChoise = new HandleTurn();

                    //GUIActive
                    AttackPanel.SetActive(true);

                    CreateAttackButton();
                    HeroInput = HeroGuI.WAITING;
                }
                break;
            case (HeroGuI.WAITING):
                //待機
                break;
            case (HeroGuI.DONE):
                HeroInputDone();
                break;
        }
    }

	public void CollectActions(HandleTurn input)
	{
		performList.Add(input);
	}

	void EnemyButtons()
	{
		foreach(GameObject enemy in EnemyInBattle)
		{
			GameObject newButton = Instantiate(enemyButton) as GameObject;
			EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

			EnemyStateMaschine cur_enemy = enemy.GetComponent<EnemyStateMaschine>();

			Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
			buttonText.text = cur_enemy.enemy.thename;

			button.EnemyPrefab = enemy;

			newButton.transform.SetParent(Spacer, false);
		}
	}

    //攻撃ボタン
    public void Input1()
    {
        HeroChoise.Attacker = HerosToManage[0].name;
        HeroChoise.AttacksGameObject = HerosToManage[0];
        HeroChoise.Type = "Hero";
        HeroChoise.choosenAttack = HerosToManage[0].GetComponent<HeroStateMaschine>().hero.attacks[0];
        //HeroGUIsetting
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    //敵選択
    public void Input2(GameObject choosenEnemy)
    {
        HeroChoise.AttackerTarget = choosenEnemy;
        HeroInput = HeroGuI.DONE;
    }

    //choosen magic attack
    public void Input4(BaseAttack choosenMagic)
    {
        HeroChoise.Attacker = HerosToManage[0].name;
        HeroChoise.AttacksGameObject = HerosToManage[0];
        HeroChoise.Type = "Hero";

        //magic choosen
        HeroChoise.choosenAttack = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    //switching to magic attcks
    public void Input3()
    {
        AttackPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }

    void HeroInputDone()
    {
        Debug.Log("Done");
        //値を返す
        performList.Add(HeroChoise);
        //HeroGUI off
        EnemySelectPanel.SetActive(false);

        //clean the attackPanel
        foreach(GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();

        //selector off
        HerosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        //ListRemove
        HerosToManage.RemoveAt(0);
        //実行
        HeroInput = HeroGuI.ACTIVATE;
    }

    //create ActionButtonsPanel
    void CreateAttackButton()
    {
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        AttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(AttackButton);

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "Magic";
        //input3実行
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        MagicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(MagicAttackButton);

        if(HerosToManage[0].GetComponent<HeroStateMaschine>().hero.MagicAttacks.Count > 0)
        {
            foreach(BaseAttack magicAtk in HerosToManage[0].GetComponent<HeroStateMaschine>().hero.MagicAttacks)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;
                Text MagicButtonText = magicButton.transform.Find("Text").gameObject.GetComponent<Text>();
                MagicButtonText.text = magicAtk.attackName;
                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.magicAttackToPerform = magicAtk;
                MagicButton.transform.SetParent(magicSpacer, false);
                atkBtns.Add(MagicButton);
            }
        }
        else
        {
            //button false
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }
    }


}
