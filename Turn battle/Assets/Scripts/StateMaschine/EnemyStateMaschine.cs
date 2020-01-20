using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMaschine : MonoBehaviour
{
	private BattleStateManager BSM;
    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WATTING,
        ACTION,
        DEAD
    }

    public TurnState currnetState;

    //progressBar
    private float cur_cooldown = 0f;
    private float max_cooldown = 5f;

	//スタートポジション
	private Vector3 startposition;
    public GameObject Selector;
	private bool actionStarted = false;
	public GameObject HeroToAttack;
	private float animSpeed = 5f;
	void Start()
    {
        currnetState = TurnState.PROCESSING;
        Selector.SetActive(false);
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateManager>();
		startposition = transform.position;
    }

    void Update()
    {
        switch (currnetState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
				ChooseAction();
				currnetState = TurnState.WATTING;
				break;
            case (TurnState.WATTING):
				//待機

                break;
            case (TurnState.ACTION):
				StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                break;
        }
    }

    void UpgradeProgressBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;
        if (cur_cooldown >= max_cooldown)
        {
            currnetState = TurnState.CHOOSEACTION;
        }
    }

	//敵攻撃設定
	void ChooseAction()
	{
		HandleTurn myAttack = new HandleTurn();
		myAttack.Attacker = enemy.thename;
		myAttack.Type = "Enemy";
		myAttack.AttacksGameObject = this.gameObject;
		myAttack.AttackerTarget = BSM.HerosInBattle[Random.Range(0, BSM.HerosInBattle.Count)];

        int num = Random.Range(0, enemy.attacks.Count);
        myAttack.choosenAttack = enemy.attacks[num];
        Debug.Log(this.gameObject.name + " has choosen " + myAttack.choosenAttack.attackName + " and do " + myAttack.choosenAttack.attackDamage + " damage!");
        BSM.CollectActions(myAttack);
	}

	private IEnumerator TimeForAction()
	{
		if (actionStarted)
		{
			yield break;
		}

		actionStarted = true;

		//攻撃アニメ
		Vector3 heroPosition = new Vector3(HeroToAttack.transform.position.x + 1.5f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
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

    void DoDamage()
    {
        float calc_damage = enemy.curATK + BSM.performList[0].choosenAttack.attackDamage;
        HeroToAttack.GetComponent<HeroStateMaschine>().TakeDamage(calc_damage);
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.curHP -= getDamageAmount;
        if(enemy.curHP <= 0)
        {
            enemy.curHP = 0;
            currnetState = TurnState.DEAD;
        }
    }
}
