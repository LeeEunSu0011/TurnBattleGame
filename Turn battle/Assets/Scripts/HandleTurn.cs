using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string Attacker;//名前
	public string Type;
    public GameObject AttacksGameObject;//攻撃するターゲット
    public GameObject AttackerTarget;//誰を攻撃するか

    public BaseAttack choosenAttack;
}
