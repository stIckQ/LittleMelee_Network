using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDamage : MonoBehaviour {

    public GameObject LineCastObject_0;
    public GameObject LineCastObject_1;
    public GameObject LineCastObject_2;

    private List<GameObject> lineCastObj;

    private Vector3[] linecastPos_lastFrame;
    private Vector3[] linecastPos_currentFrame;

    private PlayerController playerController;
    private bool canDamage = true;
    private int frameCount = 0;
    private Vector3 attackPosition;

    public const float damageCDTime = 1.5f;

    // Use this for initialization
    void Start()
    {
        lineCastObj = new List<GameObject>();

        lineCastObj.Add(LineCastObject_0);
        lineCastObj.Add(LineCastObject_1);
        lineCastObj.Add(LineCastObject_2);

        linecastPos_lastFrame = new Vector3[lineCastObj.Count];
        linecastPos_currentFrame = new Vector3[lineCastObj.Count];

        playerController = PlayerManager.instance.player.GetComponent<PlayerController>();


    }

    // Update is called once per frame
    void Update()
    {
        if (canDamage)
        {
            frameCount++;
            if (frameCount == 1)
            {
                if (LineCastCollision(out attackPosition))
                {
                    playerController.GetComponent<PlayerController>().GetHit(attackPosition);
                    StartCoroutine(DamageCD());
                }
                frameCount = 0;
            }
        }
    }

    bool LineCastCollision(out Vector3 attackPosition)
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
                if (hit.collider.gameObject.tag == "Player")
                {
                    attackPosition = lineCastObj[i].transform.position;
                    return true;
                }
            }
        }
        attackPosition=new Vector3(0,0,0);
        return false;
    }

    IEnumerator DamageCD()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCDTime);
        canDamage = true;
    }
}
