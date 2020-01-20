using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseClass
{
    public int stamina;//スタミナ
    public int intellect;//
    public int dexterity;//
    public int agility;//アジリティ

    public List<BaseAttack> MagicAttacks = new List<BaseAttack>();
}
