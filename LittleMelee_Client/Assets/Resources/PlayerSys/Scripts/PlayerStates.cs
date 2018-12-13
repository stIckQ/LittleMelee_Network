using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour {

    [Header("HealthState")]
    public int health; 
    public int maxHealth = 100;
    public bool isAlive = true;

    [Header("Score")]
    public int scoreNum=0;

    //animState
    public const int animState_Idle = 0;
    public const int animState_Walk = 1;
    public const int animState_Run = 2;
    public const int animState_Attack_Slight_1 = 3;
    public const int animState_Attack_Slight_2 = 4;
    public const int animState_Attack_Heavy_1 = 5;
    public const int animState_Attack_Heavy_2 = 6;

    

    // Use this for initialization
    void Start () {
        health = 100;
	}
	
    /*Take Damage Function */
	public void TakeDamage()
    {
        //random damage value
        int damageValue = Random.Range(5, 15);
        health -= damageValue;

        //Debug.Log(health);
        if (health <= 0)
        {
            //set game state
            GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
            GameController gameController = gameManager.GetComponent<GameController>();
            gameController.canGameOver = true;

            //set health state
            health = 0;
            isAlive = false;
        }
    }

    public void ScoreAdd()
    {
        scoreNum++;
    }
}
