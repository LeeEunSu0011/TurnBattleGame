using System.Collections;
using UnityEngine;

public class Fire1Spell : BaseAttack
{
    public Fire1Spell()
    {
        attackName = "Fire1";
        attackDiscription = "Basic Fire Spell which burns nothing";
        attackDamage = 20f;
        attackCost = 10f;
    }
}