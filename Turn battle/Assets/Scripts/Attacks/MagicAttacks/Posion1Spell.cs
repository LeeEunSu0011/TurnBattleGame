using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posion1Spell : BaseAttack
{
    public Posion1Spell()
    {
        attackName = "Poison1";
        attackDiscription = "Basic Poison Spell which drags damage over time";
        attackDamage = 5f;
        attackCost = 5f;
    }
}
