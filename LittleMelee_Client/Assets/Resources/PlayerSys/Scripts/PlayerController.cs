using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0.1f, 10.0f)]
    public float moveSpeed = 1;

    [Range(50.0f,500.0f)]
    public float rotateSpeed = 250.0f;
    public float gravity = 10;
    public bool attacking = false;
    // public bool canAttack = true;

    public GameObject gameManager;

    private Animator animator;
    private CharacterController characterController;
    private PlayerStates playerStates;
    private GameController gameController;
    private Vector3 moveDirection = Vector3.zero;
    private bool canMove = true;
    private bool ShiftPressed = false;
    private int attackSlightNum = 1;
    private int attackHeavyNum = 1;
    private int condition=0;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerStates = GetComponent<PlayerStates>();
        gameController = gameManager.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStates.isAlive&&gameController.isPlaying)
        {
            if (characterController.isGrounded)
            {
                //caculate move for camera direction
                moveDirection = Camera.main.transform.forward * Input.GetAxis("Vertical")
                              + Camera.main.transform.right * Input.GetAxis("Horizontal");

                moveDirection = transform.InverseTransformDirection(moveDirection);

                //rotate player by x and z axis value
                float rotateAngle = Mathf.Atan2(moveDirection.x, moveDirection.z);
                transform.Rotate(0.0f, rotateAngle * rotateSpeed * Time.deltaTime, 0.0f);

                //set movedirection for player direction
                moveDirection = transform.forward * moveDirection.magnitude;

                #region Previous Way
                //float translation = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
                //float rotation = Input.GetAxis("Horizontal") * rotationRate * Time.deltaTime;

                //transform.Translate(0, 0, translation);
                //transform.Rotate(0, rotation, 0);

                //if (translation != 0)
                //{
                //    animator.SetInteger("Condition", 1);
                //}
                //else
                //{
                //    animator.SetInteger("Condition", 0);
                //}
                #endregion

                //set animator condition
                condition = animator.GetInteger("Condition");
                attacking = IsAttacking();
                if (canMove && (moveDirection.x != 0 || moveDirection.z != 0))
                {
                    //run branch
                    if (ShiftPressed)
                    {
                        Run();
                    }
                    //walk branch
                    else
                    {
                        Walk();
                    }

                }
                //idle branch
                else
                {
                    if (!attacking)
                    {
                        animator.SetInteger("Condition", PlayerStates.animState_Idle);
                    }
                }

                //LMB attack
                if (Input.GetMouseButtonDown(0))
                {
                    Attack(0);
                }

                //RMB attack
                if (Input.GetMouseButtonDown(1))
                {
                    Attack(1);
                }

                //Shift pressed state
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    ShiftPressed = true;
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    ShiftPressed = false;
                }

                // Debug.Log(condition);

                //attack
                if (attacking)
                {
                    //while attack animation play to the end ,set animator state
                    AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    // Debug.Log(animatorStateInfo.normalizedTime);
                    if ((animatorStateInfo.IsName("Attack_Slight_1")
                      || animatorStateInfo.IsName("Attack_Heavy_1")
                      || animatorStateInfo.IsName("Attack_Slight_2")
                      || animatorStateInfo.IsName("Attack_Heavy_2")) && animatorStateInfo.normalizedTime >= 0.9f)
                    {
                        animator.SetInteger("Condition", PlayerStates.animState_Idle);
                        canMove = true;
                        //if(animatorStateInfo.IsName("Attack_Heavy_1"))
                        //{
                        //    Debug.Log("Attack_Heavy_1"+condition);
                        //}
                        //else if(animatorStateInfo.IsName("Attack_Heavy_2"))
                        //{
                        //    Debug.Log("Attack_Heavy_2"+condition);
                        //}
                        //Debug.Log("Change to 0");
                    }
                }
            }

            moveDirection.y -= gravity * Time.deltaTime;

            //move by charactor controller
            if (canMove)
            {
                characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            animator.SetInteger("Condition", PlayerStates.animState_Idle);
        }
    }

    //attack
    void Attack(int attackType)
    {
        if(!attacking)
        {
            canMove = false;
            if (attackType == 0)
            {
                if (attackSlightNum == 1)
                {
                    animator.SetInteger("Condition", PlayerStates.animState_Attack_Slight_1);
                    attackSlightNum = 2;
                }
                else if (attackSlightNum == 2)
                {
                    animator.SetInteger("Condition", PlayerStates.animState_Attack_Slight_2);
                    attackSlightNum = 1;
                }

            }
            else if (attackType == 1)
            {
                if (attackHeavyNum == 1)
                {
                    animator.SetInteger("Condition", PlayerStates.animState_Attack_Heavy_1);
                    attackHeavyNum = 2;
                }
                else if (attackHeavyNum == 2)
                {
                    animator.SetInteger("Condition", PlayerStates.animState_Attack_Heavy_2);
                    attackHeavyNum = 1;
                }
            }
        }
    }

    //run
    void Run()
    {
        moveSpeed = 3;
        animator.SetInteger("Condition", PlayerStates.animState_Run);
    }

    //walk
    void Walk()
    {
        moveSpeed = 1;
        animator.SetInteger("Condition", PlayerStates.animState_Walk);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (other.gameObject.tag == "Enemy")
        //{
        //    playerStates.TakeDamage();
        //}
    }

    //isattacking
    bool IsAttacking()
    {
        if(condition == PlayerStates.animState_Attack_Slight_1
        || condition == PlayerStates.animState_Attack_Heavy_1
        || condition == PlayerStates.animState_Attack_Slight_2
        || condition == PlayerStates.animState_Attack_Heavy_2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //GetHit
    public void GetHit(Vector3 attackPosition)
    {
        PlayerStates playerStates = GetComponent<PlayerStates>();
        if (playerStates.isAlive)
        {
            //damage
            GetComponent<PlayerStates>().TakeDamage();

            //HitEffect

            //HitSound
            string soundPath = "EnemySys/Sound/HitBody";
            SoundManager hitSound = new SoundManager(gameObject, soundPath);
            hitSound.PlaySoundAtPosition(attackPosition);
        }
    }
}
