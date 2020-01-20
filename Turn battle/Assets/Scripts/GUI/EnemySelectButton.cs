using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
	public GameObject EnemyPrefab;
    private bool showSelector;

	public void selectEnemy()
	{
		//選択した敵を保存
		GameObject.Find("BattleManager").GetComponent<BattleStateManager>().Input2(EnemyPrefab);
	}

    public void ToggleSelector()
    {
        if (showSelector)
        {
            EnemyPrefab.transform.Find("Selector").gameObject.SetActive(true);
            showSelector = !showSelector;
        }
    }
}
