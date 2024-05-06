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
    [SerializeField] private int maxHealthWarrior;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider2D colliderNormal;
    [SerializeField] private Collider2D colliderSlide;

    [SerializeField] GameObject hacha;

    private Animator runAnimator;

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
    private bool miraDerecha;
    private bool miraIzquierda;
    private bool hasAxe;

    private float speedRun = 6f;
    private float speedWalk = 3.7f;

    [SerializeField] private float knockbackX;
    [SerializeField] private float knockbackY;

    [SerializeField] Transform puntoLanzamiento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
        
        colliderNormal.enabled = true;
        colliderSlide.enabled = false;
        hasAxe = true;

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
                runAnimator.SetFloat("VelY", VelY);
            }

            //Salto----------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.Space) && IsOnGround && !isAttacking)
            {
                Saltar();
            }

            //Ataque basico-------------------------------------------------------------
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isRunning && IsOnGround)
            {
                isWalking = false;
                runAnimator.SetBool("IsWalking", false);
                runAnimator.SetBool("IsRunning", false);
                isAttacking = true;
                runAnimator.SetBool("IsAttacking", true);
                runAnimator.SetTrigger("Attack");
                Debug.Log("Input recibido");
                StartCoroutine(Atacar());
            }

            //Lanzar hacha---------------------------------
            if (Input.GetMouseButtonDown(1) && !isAttacking)
            {
                if(hasAxe) 
                {
                    GameObject aux;

                    aux = Instantiate(hacha, puntoLanzamiento.position, puntoLanzamiento.rotation);

                    if (miraDerecha)
                    {
                        aux.GetComponent<Rigidbody2D>().AddForce(new Vector2 (10f, 10f), ForceMode2D.Impulse);
                    } else
                    {
                        aux.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10f, 10f), ForceMode2D.Impulse);
                    }
                }

                //hasAxe = false;
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
                    if(IsOnGround)
                    {
                        isRunning = true;
                        runAnimator.SetBool("IsRunning", true);
                    }
                }

                //Dejar de correr
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    isRunning = false;
                    runAnimator.SetBool("IsRunning", false);
                }

                //Caminar
                if (Input.GetKey(KeyCode.D))
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

                    transform.position += Vector3.right * speed * Time.deltaTime;
                    FlipRigth();
                }

                if (Input.GetKey(KeyCode.A))
                {
                    Debug.Log("Pulsando A");
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

                    transform.position += Vector3.right * -speed * Time.deltaTime;
                    FlipLeft();
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
        runAnimator.SetTrigger("Jump");
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
            runAnimator.SetBool("IsHurt", false);

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
        miraDerecha = true; //1 Derecha
        miraIzquierda = false;
    }
    
    void FlipLeft()
    {
        gameObject.transform.localScale = new Vector3(-5, 5, 5);
        miraIzquierda = true; //1 Izquierda
        miraDerecha = false;
    }
}
