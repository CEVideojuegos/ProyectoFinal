using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject renderImage;
    [SerializeField] private int maxHealthWarrior;
    [SerializeField] private Camera mainCamera;

    private Animator runAnimator;
    private Collider2D colliderBody;
    private SpriteRenderer spriteRenderer;

    private bool IsOnGround;
    private bool isIdle;
    private bool isJumping;
    private bool isWalking;
    private bool isRunning;
    private bool isAttacking;
    private bool isSliding;
    private bool sprintAttack;
    private bool jumpAttack;
    private bool isFalling;
    private bool isHurt;
    private bool isDead;

    private bool canSlide = true;
    private bool turnPosition = true;
    private bool invulnerability;

    private float speedRun = 6f;
    private float speedWalk = 3.7f;

    private String[] NombreAnimaciones = new String[] {"IsJumping", "IsFalling", "IsWalking", "IsRunning", "IsAtacking", "IsHurt", "IsIdle"} ;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
        colliderBody = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //Debug.Log("IsOnGround = " + IsOnGround);
        //Debug.Log("IsJumping = " + isJumping);
        //Debug.Log("IsFalling = " + isFalling);

        /*if (isJumping || isFalling || isWalking || isRunning || isAttacking || isHurt)
        {
            runAnimator.ResetTrigger("IsIdle");
        }
        else
        {
            runAnimator.SetTrigger("IsIdle");
        }*/

        if (isDead)
        {
            runAnimator.SetTrigger("IsDead");
            
        } else if (!isDead) {
            if (isRunning)
            {
                speed = speedRun;
            }
            else
            {
                speed = speedWalk;
            }

            if (IsOnGround && !isJumping && !isFalling && !isWalking && !isRunning
                && !isAttacking && !isHurt && !sprintAttack && !jumpAttack && !isSliding)
            {
                runAnimator.SetTrigger("IsIdle");
            }
            else
            {
                runAnimator.ResetTrigger("IsIdle");
            }

            if (!IsOnGround && !isJumping && !jumpAttack)
            {
                //StartCoroutine(PlayFallingAnimation());
                runAnimator.SetTrigger("IsFalling");
            }
            else
            {
                runAnimator.ResetTrigger("IsFalling");
            }

            if (isWalking)
            {
                runAnimator.SetTrigger("IsWalking");
            }
            else
            {
                runAnimator.ResetTrigger("IsWalking");
            }

            if (isRunning)
            {
                runAnimator.SetTrigger("IsRunning");
            }
            else
            {
                runAnimator.ResetTrigger("IsRunning");
            }

            if (isJumping)
            {
                runAnimator.SetTrigger("IsJumping");
            }
            else
            {
                runAnimator.ResetTrigger("IsJumping");
            }

            if (isHurt)
            {
                runAnimator.SetTrigger("IsHurt");
            }
            else
            {
                runAnimator.ResetTrigger("IsHurt");
            }

            if (isAttacking)
            {
                runAnimator.SetTrigger("IsAtacking");
            }
            else
            {
                runAnimator.ResetTrigger("IsAtacking");
            }

            if (sprintAttack || jumpAttack)
            {
                runAnimator.SetTrigger("SpecialAttack");
            }
            else
            {
                runAnimator.ResetTrigger("SpecialAttack");
            }

            if (isSliding)
            {
                runAnimator.SetTrigger("IsSliding");
            }
            else
            {
                runAnimator.ResetTrigger("IsSliding");
            }


            //runAnimator.SetBool("IsJumping", isJumping);

            if (Input.GetKeyDown(KeyCode.Space) && IsOnGround && !isAttacking)
            {
                Saltar();
            }

            if (Input.GetMouseButtonDown(0) && !isAttacking && !isRunning && !isJumping)
            {
                isAttacking = true;
                StartCoroutine(Atacar());
            }

            if (Input.GetMouseButtonDown(0) && isRunning)
            {
                sprintAttack = true;
                StartCoroutine(Atacar());
            }

            if (Input.GetMouseButtonDown(0) && (isJumping || isFalling))
            {
                jumpAttack = true;
                StartCoroutine(Atacar());
            }

            if (Input.GetKey(KeyCode.C) && IsOnGround && canSlide)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    colliderBody.enabled = false;
                    isSliding = true;
                    canSlide = false;
                    rb.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
                    StartCoroutine(Slide());
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    colliderBody.enabled = false;
                    isSliding = true;
                    canSlide = false;
                    rb.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
                    StartCoroutine(Slide());
                }
            }

            //Moverse();
            if (!isHurt && !isAttacking && !sprintAttack && !isSliding)  //he recibido un ataque hace poco y no estoy atacando
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
                {
                    //speed = 6;
                    isRunning = true;
                    isWalking = false;
                    isJumping = false;
                    //runAnimator.SetTrigger("IsRunning");
                }
                else
                {
                    //speed = 3.7f;
                    isRunning = false;
                    //runAnimator.ResetTrigger("IsRunning");
                }

                if (!(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)))
                {
                    isWalking = false;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    if (!isJumping && !isRunning)
                    {
                        isWalking = true;
                    }
                    transform.position += Vector3.right * speed * Time.deltaTime;
                    FlipRigth();
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    if (!isJumping && !isRunning)
                    {
                        isWalking = true;
                    }
                    transform.position += Vector3.right * -speed * Time.deltaTime;
                    FlipLeft();
                }
            }
        }
    }

        

    
    void Saltar()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        StartCoroutine(EsperaCaida());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo") || other.gameObject.CompareTag("Enemy"))
        {
            isJumping = false;
            IsOnGround = true;
            isFalling = false;
            jumpAttack = false;
            //runAnimator.ResetTrigger("IsFalling");
        }

        
        if (other.gameObject.CompareTag("Enemy") && !invulnerability)
        {
            /*
            runAnimator.ResetTrigger("IsFalling");
            isFalling = false;
            runAnimator.ResetTrigger("IsJumping");
            isJumping = false;
            runAnimator.ResetTrigger("IsWalking");
            runAnimator.ResetTrigger("IsRunning");
            isWalking = false;
            isRunning = false;
            runAnimator.ResetTrigger("IsAtacking");
            */
            

            for(int i = 0; i < NombreAnimaciones.Length; i++)
            {
                runAnimator.ResetTrigger(NombreAnimaciones[i]);
            }

            invulnerability = true;
            //runAnimator.SetTrigger("IsHurt");
            StartCoroutine(PlayInvulnerability());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            IsOnGround = false;
            isJumping = true;
            isWalking = false;
            isRunning = false;
            //StartCoroutine(PlayFallingAnimation());
        }
    }

    void Moverse()
    {
    if(!invulnerability && !isAttacking)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
            {
                speed = 6;
                isRunning = true;
                runAnimator.SetTrigger("IsRunning");
            }
            else
            {
                speed = 3.7f;
                isRunning = false;
                runAnimator.ResetTrigger("IsRunning");
            }
            
            if (!(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)))
            {
                isWalking = false;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!isJumping && !isRunning)
                {
                    isWalking = true;
                }
                transform.position += Vector3.right * speed * Time.deltaTime;
                FlipRigth();
            }
            else if (Input.GetKey(KeyCode.A))
            {
                if (!isJumping && !isRunning)
                {
                    isWalking = true;
                }
                transform.position += Vector3.right * -speed * Time.deltaTime;
                FlipLeft();
            }
        }
    }

    private IEnumerator Atacar()
    {
        //runAnimator.SetTrigger("IsAtacking");
        //isWalking = false;
        //isFalling = false;
        //isRunning = false;
        //isJumping = false;
        //isHurt = false; 
        //isIdle = false; 
        yield return new WaitForSeconds(0.9f);
        sprintAttack = false;
        jumpAttack = false;
        isAttacking = false;
        //runAnimator.ResetTrigger("IsAtacking");
    }
    

    private IEnumerator EsperaCaida()
    {
        isWalking = false;
        isAttacking = false;
        isRunning = false;
        isJumping = false;
        isHurt = false;
        isIdle = false;
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
    }

    private IEnumerator PlayFallingAnimation() //este ya no se usa
    {
        isFalling = true;
        //runAnimator.SetTrigger("IsFalling");
        yield return new WaitForSeconds(1);
    }

    private IEnumerator PlayInvulnerability()
    {
        maxHealthWarrior--;

        if (maxHealthWarrior <= 0)
        {
            isDead = true;
        }
        else
        {
            isWalking = false;
            isFalling = false;
            isRunning = false;
            isJumping = false;
            isIdle = false;
            isAttacking = false;

            isHurt = true;

            yield return new WaitForSeconds(1.5f);
            //runAnimator.ResetTrigger("IsHurt");
            invulnerability = false;
            isHurt = false;
        }
    }

    private IEnumerator Slide()
    {
        isRunning = false;
        isWalking = false;
        yield return new WaitForSeconds(0.5f);
        colliderBody.enabled = true;
        isSliding = false;
        yield return new WaitForSeconds(1f);
        canSlide = true;
    }
    
    void FlipRigth()
    {
        gameObject.transform.localScale = new Vector3(5, 5, 5);
        if (!turnPosition)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + 0.848f, gameObject.transform.position.y, 0);
            //mainCamera.transform.position = new Vector3(gameObject.transform.position.x - 0.848f, gameObject.transform.position.y, mainCamera.transform.position.z);
            turnPosition = true;
        }
    }
    
    void FlipLeft()
    {
        gameObject.transform.localScale = new Vector3(-5, 5, 5);
        if (turnPosition)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - 0.729f, gameObject.transform.position.y, 0);
            //mainCamera.transform.position = new Vector3(gameObject.transform.position.x + 0.729f, gameObject.transform.position.y, mainCamera.transform.position.z);
            turnPosition = false;
        }
    }
}
