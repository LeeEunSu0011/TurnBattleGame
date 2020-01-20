using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMaschine : MonoBehaviour
{
	private BattleStateManager BSM;
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

    public TurnState currnetState;

    //progressBar
    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;
    public Image ProgressBar;
    public GameObject Selector;
    //IeNumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startposition;
    private float animSpeed = 5f;
    //dead
    private bool alive = true;

    //heroPanel
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    void Start()
    {
        //find spacer
        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        //createPanel
        CreateHeroPanel();

        startposition = transform.position;

        cur_cooldown = Random.Range(0, 2.5f);
        //selector off
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateManager>();
		currnetState = TurnState.PROCESSING;
    }

    void Update()
    {
        switch (currnetState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.ADDTOLIST):
				BSM.HerosToManage.Add(this.gameObject);
				currnetState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
				//待機
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadHero";
                    //not attackable by enemy
                    BSM.HerosInBattle.Remove(this.gameObject);
                    //not managable
                    BSM.HerosToManage.Remove(this.gameObject);
                    //deactivate the selector
                    Selector.SetActive(false);
                    //reset gui
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //remove item from performlist
                    for (int i = 0; i < BSM.performList.Count; i++)
                    {
                        if(BSM.performList[i].AttacksGameObject == this.gameObject)
                        {
                            BSM.performList.Remove(BSM.performList[i]);
                        }
                    }
                    //change color / play animation
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset heroinput
                    BSM.HeroInput = BattleStateManager.HeroGuI.ACTIVATE;

                    alive = false;  
                }
                break;
        }
    }

    void UpgradeProgressBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (cur_cooldown >= max_cooldown)
        {
            currnetState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //攻撃アニメ
        Vector3 heroPosition = new Vector3(EnemyToAttack.transform.position.x - 1.5f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveToWardsEnemy(heroPosition))
        {
            yield return null;
        }
        //待機
        yield return new WaitForSeconds(0.5f);

        //ダメージ
        DoDamage();

        //元に戻すアニメ
        Vector3 firstPosition = startposition;
        while (MoveToWardsStart(firstPosition))
        {
            yield return null;
        }

        //リストreset
        BSM.performList.RemoveAt(0);
        //待機
        BSM.battleStates = BattleStateManager.PerformAction.WAIT;

        //end コールテン
        actionStarted = false;

        //敵ステイタスreset
        cur_cooldown = 0f;
        currnetState = TurnState.PROCESSING;
    }
    private bool MoveToWardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveToWardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        hero.curHP -= getDamageAmount;
        if(hero.curHP <= 0)
        {
            hero.curHP = 0;
            currnetState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }

    void DoDamage()
    {
        float calc_damage = hero.curATK + BSM.performList[0].choosenAttack.attackDamage;
        EnemyToAttack.GetComponent<EnemyStateMaschine>().TakeDamage(calc_damage);
    }

    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.thename;
        stats.heroHP.text = "HP : " + hero.curHP + " / " + hero.baseHP;
        stats.HeroMP.text = "MP : " + hero.curMP + " / " + hero.baseMP;

        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }

    void UpdateHeroPanel()
    {
        stats.heroHP.text = "HP : " + hero.curHP + " / " + hero.baseHP;
        stats.HeroMP.text = "MP : " + hero.curMP + " / " + hero.baseMP;
    }
}
