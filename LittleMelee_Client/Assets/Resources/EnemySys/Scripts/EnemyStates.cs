using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates :MonoBehaviour  {

    //animState
    public const int animState_Idle = 0;
    public const int animState_Walk = 1;
    public const int animState_Attack = 2;
    public const int animState_GetHit= 3;
    public const int animState_Die = 4;

    //attackRange
    public const float attackRange = 2.0f;
    public const float GetHitRecoverTime = 2f;

    public int maxHealth = 200;
    public int health = 200;


    public bool isAlive = true; 

    public void TakeDamage()
    { 
        int damageValue = Random.Range(25, 50);
        health -= damageValue;
        //Debug.Log(health);
        if (health <= 0)
        {
            health = 0;
            isAlive = false;
        }
    }
}
