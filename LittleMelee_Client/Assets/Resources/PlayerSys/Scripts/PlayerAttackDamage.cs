using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDamage : MonoBehaviour {

    
    private List<GameObject> lineCastObj;
  
    private PlayerController playerController;
    private Vector3[] linecastPos_lastFrame;
    private Vector3[] linecastPos_currentFrame;

    private EnemyStates enemyStats;
    private bool canDamage = true;
    private int frameCount = 0;
    private Vector3 attackPosition;
    private PlayerStates playerStates;
    
	// Use this for initialization
	void Start () {
        playerController = GetComponent<PlayerController>();
        lineCastObj = new List<GameObject>();

        lineCastObj.Add(PlayerManager.instance.LineCastObject_0);
        lineCastObj.Add(PlayerManager.instance.LineCastObject_1);
        lineCastObj.Add(PlayerManager.instance.LineCastObject_2);
        lineCastObj.Add(PlayerManager.instance.LineCastObject_3);
        lineCastObj.Add(PlayerManager.instance.LineCastObject_4);

        linecastPos_lastFrame = new Vector3[lineCastObj.Count];
        linecastPos_currentFrame = new Vector3[lineCastObj.Count];

        playerStates = GetComponent<PlayerStates>();

    }
	
	// Update is called once per frame
	void Update () {
		if(playerController.attacking&&canDamage)
        {
            frameCount++;
            if(frameCount==1)
            {
                GameObject AttackedEnemy = LineCastCollision(out attackPosition);
                if (AttackedEnemy!=null)
                {
                    //hit
                    AttackedEnemy.GetComponent<EnemyController>().GetHit(attackPosition);
                    if(!AttackedEnemy.GetComponent<EnemyStates>().isAlive)
                    {
                        playerStates.ScoreAdd();
                    }
                    canDamage = false;
                }
                frameCount = 0;
            }
        }
        if (!playerController.attacking)
        {
            canDamage = true;
        }
	}

    GameObject LineCastCollision(out Vector3 attackPosition)
    {
        RaycastHit hit;
        for (int i = 0; i < lineCastObj.Count; i++)
        {
            linecastPos_lastFrame[i] = linecastPos_currentFrame[i];
            linecastPos_currentFrame[i] = lineCastObj[i].transform.position;
           
        }
        for (int i = 0; i < lineCastObj.Count; i++) 
        {
            if (Physics.Linecast(linecastPos_lastFrame[i], linecastPos_currentFrame[i], out hit, 1 << 0))
            {
                if (hit.collider.gameObject.tag == "Enemy")
                {
                    attackPosition = lineCastObj[i].transform.position;
                    return hit.collider.gameObject;
                }
            }
        }
        attackPosition = new Vector3(0, 0, 0);
        return null;
    }
}
