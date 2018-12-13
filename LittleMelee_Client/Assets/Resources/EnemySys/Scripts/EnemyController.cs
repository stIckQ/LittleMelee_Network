using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController :MonoBehaviour{

    [Header("Move")]
    public float lookRadius = 10f;
    public float moveSpeed = 1f;
    public float moveRadius = 5f;

    [Header("WanderAndPatrol")]
    public bool canWander = false;
    public bool canPatrol = true;
    public float waitTime = 2f;
    public Vector2[] patrolPostionXZ;

    [Header("Attack")]
    public bool canAttack = true;
    public GameObject attackParticle;

    private Transform target;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private NavMeshHit navMeshHit;
    private Vector3 wanderPosition;
    private Vector3 patrolPosition;
    private int patrolNum;
    private Coroutine coroutine;
    private int coroutineCount;
    private bool isGettingHit;
    private EnemyStates enemyStates;
    private GameObject gameManager;
    private GameController gameController;

    // Use this for initialization
    void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = PlayerManager.instance.player.transform;
        animator = GetComponent<Animator>();
        navMeshAgent.speed = moveSpeed;
        wanderPosition = GetRandomWanderPosition();
        patrolNum = 0;
        patrolPosition = GetPatrolPosition(patrolNum);
        coroutineCount = 0;
        isGettingHit = false;
        enemyStates = GetComponent<EnemyStates>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameController = gameManager.GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
       
        if(gameController.isPlaying)
        {
            if (enemyStates.isAlive)
            {
                float distance = Vector3.Distance(target.position, transform.position);

                if (distance <= EnemyStates.attackRange)
                {
                    if (canAttack)
                    {
                        transform.LookAt(target.position);
                        animator.SetInteger("Condition", EnemyStates.animState_Attack);
                    }
                }
                else
                {
                    //while playing attack animation ,wait 
                    AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (!(animatorStateInfo.IsName("Attack") && animatorStateInfo.normalizedTime % 1f < 0.9f))
                    {
                        //track Branch
                        if (distance <= lookRadius)
                        {
                            TrackPlayer();
                        }
                        else
                        {
                            //Patrol Branch
                            if (canPatrol&&patrolPostionXZ.Length!=0)
                            {
                                Patrol();
                                
                            }

                            //Wander Branch
                            else if (canWander||(canPatrol&&patrolPostionXZ.Length==0)) Wander();

                        }

                        //set animator state
                        if (navMeshAgent.velocity.magnitude > 0.2f)
                        {
                            if (!isGettingHit)
                            {
                                animator.SetInteger("Condition", EnemyStates.animState_Walk);
                            }
                        }
                        else
                        {
                            if (!isGettingHit)
                            {
                                animator.SetInteger("Condition", EnemyStates.animState_Idle);
                            }
                        }
                    }
                }
            }
            else
            {
                Die();
            }
        }
        else
        {
            animator.SetInteger("Condition", EnemyStates.animState_Idle);
        }
	}

    public void TrackPlayer()
    {
        if (!isGettingHit)
        {
            navMeshAgent.stoppingDistance = EnemyStates.attackRange;
            navMeshAgent.SetDestination(target.position);
        }
    }

    //wander
    public void Wander()
    {
        navMeshAgent.stoppingDistance = 0f;
        if (Vector3.Distance(transform.position, wanderPosition) < 0.1f)
        {
            StartCoroutine(WanderWait());
            wanderPosition = GetRandomWanderPosition();
        }
        else
        {
            navMeshAgent.SetDestination(wanderPosition);
            if (!navMeshAgent.hasPath)
            {
                wanderPosition = GetRandomWanderPosition();
            }
        }
        //Debug.Log(transform.position.ToString()+wanderPosition.ToString()+ Vector3.Distance(transform.position, wanderPosition).ToString());
    }

    //patrol
    public void Patrol()
    {
        navMeshAgent.stoppingDistance = 0f;

        if(Vector3.Distance(transform.position,patrolPosition)<0.1f)
        {
            StartCoroutine(PatrolWait());

            patrolNum = (patrolNum + 1) % patrolPostionXZ.Length;
            patrolPosition =GetPatrolPosition(patrolNum);  

        }
        else
        {
            navMeshAgent.SetDestination(patrolPosition);
            if (!navMeshAgent.hasPath)
            {
                patrolNum = (patrolNum + 1) % patrolPostionXZ.Length;
                patrolPosition = GetPatrolPosition(patrolNum);
            }
        }
    }

    //get random wander positon
    public Vector3 GetRandomWanderPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * moveRadius + transform.position;
        NavMesh.SamplePosition(randomPosition, out navMeshHit, moveRadius, -1);
        randomPosition.y = Terrain.activeTerrain.SampleHeight(randomPosition);

        return new Vector3(randomPosition.x, randomPosition.y, randomPosition.z);

    }

    //GetPatrolPosition
    private Vector3 GetPatrolPosition(int i)
    {
        float height = Terrain.activeTerrain.SampleHeight(new Vector3(patrolPostionXZ[i].x, 0, patrolPostionXZ[i].y));
        Vector3 patrolPosition = new Vector3(patrolPostionXZ[i].x, height, patrolPostionXZ[i].y);

        return patrolPosition;
    }

    //after wander wait
    private IEnumerator WanderWait()
    {
        canWander = false;
        yield return new WaitForSeconds(waitTime);
        canWander = true;

    }

    //afte patrol wait
    private IEnumerator PatrolWait()
    {
        canPatrol = false;
        yield return new WaitForSeconds(waitTime);
        canPatrol = true;

    }

    //get hit
    public void GetHit(Vector3 attackPosition)
    {
        if(enemyStates.isAlive)
        {
            //Damage
            GetComponent<EnemyStates>().TakeDamage();
            animator.SetInteger("Condition", EnemyStates.animState_GetHit);

            //HitSound
            string soundPath = "PlayerSys/Sound/Hit_axe_2";
            SoundManager hitSound=new SoundManager(gameObject,soundPath);
            hitSound.PlaySoundAtPosition(attackPosition);

            //attack effect
            GameObject particle = Instantiate(attackParticle, attackPosition, Quaternion.identity)as GameObject;
            ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule main=particleSystem.main;
            main.startSize= 0.1f;
            Destroy(particle, 2);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(GetHitRecover(coroutineCount++));
        }
    }

    //after get hit, need recover
    IEnumerator GetHitRecover(int i)
    {
        canAttack = false;
        isGettingHit = true;
        yield return new WaitForSeconds(EnemyStates.GetHitRecoverTime);
        isGettingHit = false;
        canAttack = true;
    }

    public void Die()
    {
        animator.SetInteger("Condition", EnemyStates.animState_Die);
    }

    //test range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
