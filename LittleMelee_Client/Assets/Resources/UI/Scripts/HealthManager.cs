using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour {

    public Image healthbar;
    public Text healthText;
    public Text killNumText;

    private int health;
    private int killNum;
    private PlayerStates playerStates;

	// Use this for initialization
	void Start () {
        playerStates = PlayerManager.instance.player.GetComponent<PlayerStates>();
        SetHealthBar();
        SetKillNumText();
	}
	
	// Update is called once per frame
	void Update () {
        SetHealthBar();
        SetKillNumText();
    }

    void SetHealthBar()
    {
        health =playerStates.health;
        float radio = (float)health / playerStates.maxHealth;
        healthbar.rectTransform.localScale = new Vector3(radio,1,1);
        healthText.text = health.ToString();
    }

    void SetKillNumText()
    {
        killNum = playerStates.scoreNum;
        killNumText.text = killNum.ToString();
    }
}
