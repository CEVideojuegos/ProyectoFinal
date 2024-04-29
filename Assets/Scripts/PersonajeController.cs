using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float slideVelocity;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject renderImage;
    [SerializeField] private int maxHealthWarrior;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider colliderNormal;
    [SerializeField] private Collider colliderSlide;

    private Animator runAnimator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private bool IsOnGround;
    [SerializeField] private float VelY;

    private bool isIdle;
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

    private float speedRun = 6f;
    private float speedWalk = 3.7f;

    [SerializeField] private float knockbackX;
    [SerializeField] private float knockbackY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
        colliderNormal = GetComponent<Collider>();
        colliderSlide = GetComponent<Collider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead)
        {
            runAnimator.SetTrigger("IsDead");
            
        } else if (!isDead) {

            runAnimator.SetBool("IsGrounded", IsOnGround);
            VelY = rb.velocity.y;

            if(rb.velocity.y !=0) 
            {
                runAnimator.SetFloat("velY", VelY);
            }

            //Salto----------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.Space) && IsOnGround && !isAttacking)
            {
                Saltar();
            }


            //Ataque basico-------------------------------------------------------------
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isRunning && !IsOnGround)
            {
                isWalking = false;
                runAnimator.SetBool("IsWalking", false);
                runAnimator.SetBool("IsRunning", false);
                isAttacking = true;
                runAnimator.SetBool("IsAttacking", true);
                runAnimator.SetTrigger("Attack");
                StartCoroutine(Atacar());
            }

            //Ataque en carrera/caida--------------------
            if (Input.GetMouseButtonDown(0) && !isAttacking && (isRunning || !IsOnGround))
            {
                isWalking = false;
                runAnimator.SetBool("IsWalking", false);
                runAnimator.SetBool("IsRunning", false);
                isAttacking = true;
                runAnimator.SetBool("IsAttacking", true);
                runAnimator.SetTrigger("DashAttack");
                StartCoroutine(Atacar());
            }

            /*
            if (Input.GetMouseButtonDown(0) && (isJumping || isFalling))
            {
                jumpAttack = true;
                StartCoroutine(Atacar());
            }*/

            //Deslizarse-----------------------------------------
            if (Input.GetKey(KeyCode.C) && IsOnGround && canSlide)
            {
                if (Input.GetKey(KeyCode.D))
                {
                    StartCoroutine(Slide(true));
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    StartCoroutine(Slide(false));
                }
            }

            //Movimiento en suelo-------------------------------------
            if (!isHurt && !isAttacking && !sprintAttack && !isSliding)  
            {
                //Correr
                if(Input.GetKey(KeyCode.LeftShift))
                {
                        isRunning = true;
                        runAnimator.SetBool("IsRunning", true);
                }

                //Dejar de correr
                if (Input.GetKeyUp(KeyCode.RightShift))
                {
                    isRunning = false;
                    runAnimator.SetBool("IsRunning", false);
                }

                //Caminar
                if (Input.GetKeyUp(KeyCode.D))
                {
                        isWalking = true;
                        runAnimator.SetBool("IsWalking", true);
                        
                    if(isRunning)
                    {
                        speed = speedRun;
                    }
                    else
                    {
                        speed = speedWalk;
                    }

                    transform.position = Vector3.right * speed * Time.deltaTime;
                }

                if (Input.GetKeyUp(KeyCode.A))
                {
                    isWalking = true;
                    runAnimator.SetBool("IsWalking", true);

                    if (isRunning)
                    {
                        speed = speedRun;
                    }
                    else
                    {
                        speed = speedWalk;
                    }

                    transform.position = Vector3.right * speed * Time.deltaTime;
                }

                //Parar movimiento
                if ((Input.GetKeyUp(KeyCode.A)) || (Input.GetKeyUp(KeyCode.D)))
                {
                    isWalking = false;
                    runAnimator.SetBool("IsWalking", false) ;
                    runAnimator.SetBool("IsRunning", false);
                }
            }
        }
    }

    void Saltar()
    {
        rb.velocity = new Vector2 (0f, jumpForce);
        StartCoroutine(EsperaCaida());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            IsOnGround = true;
            isFalling = false;
        }

        
        if (other.gameObject.CompareTag("Enemy") && !isHurt)
        {
            StartCoroutine(PlayInvulnerability(other.transform));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            IsOnGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            IsOnGround = false;
        }
    }

    private IEnumerator Atacar()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        runAnimator.SetBool("IsAttacking", isAttacking);
    }  

    private IEnumerator PlayInvulnerability(Transform enemigo)
    {
        maxHealthWarrior--;

        if (maxHealthWarrior <= 0)
        {
            isDead = true;
            colliderNormal.enabled = false;
            colliderSlide.enabled = false;
        }
        else
        {
            isHurt = true;
            isWalking = false;
            isFalling = false;
            isRunning = false;
            IsOnGround = false;
            isIdle = false;
            isAttacking = false;
            isSliding = false;

            runAnimator.SetBool("IsHurt", true);

            if(this.transform.position.x > enemigo.position.x)
            {
                rb.velocity = new Vector2(knockbackX, knockbackY);
            }
            else
            {
                rb.velocity = new Vector2(-knockbackX, knockbackY);
            }

            yield return new WaitForSeconds(1.5f);
            runAnimator.SetBool("IsHurt", true);

            isHurt = false;
        }
    }

    private IEnumerator Slide(bool direccion)
    {
        colliderNormal.enabled = false;
        colliderSlide.enabled = true;

        isSliding = true;
        canSlide = false;
        runAnimator.SetTrigger("Slide");
        runAnimator.SetBool("IsSliding", true);

        if(direccion)
        {
            rb.velocity = new Vector2(slideVelocity, 0f);
        }
        else
        {
            rb.velocity = new Vector2(-slideVelocity, 0f);
        }

        yield return new WaitForSeconds(0.5f);

        colliderNormal.enabled = true;
        colliderSlide.enabled = false;

        isSliding = false;
        runAnimator.SetBool("IsSliding", false);

        yield return new WaitForSeconds(1.0f);
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
