using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
    [SerializeField] private Slider sliderHP;

    [SerializeField] GameObject hacha;

    //[SerializeField] Vector2 mousePos;
    //private Vector2 screenMousePos;

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

    private GameObject hachaLanzada;

    private bool canSlide = true;
    private bool hasAxe = true;

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
        sliderHP.value = maxHealthWarrior;

        if (isDead)
        {
            runAnimator.SetTrigger("IsDead");
            
        } else if (!isDead) {

            runAnimator.SetBool("IsGrounded", IsOnGround);
            VelY = rb.velocity.y;

            //Capturar posicion del mouse y convertirla en punto dentro del WorldView
            //mousePos = Input.mousePosition;
            //screenMousePos = Camera.main.ScreenToViewportPoint(mousePos);

            if (rb.velocity.y !=0) 
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
                    LanzarHacha();
                    hasAxe = false;
                }else if (hachaLanzada != null) 
                {
                    hachaLanzada.GetComponent<Hacha>().LlamarHacha(this.transform);
                }
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
                    //Debug.Log("Pulsando A");
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

    /*
    private IEnumerator LanzarHacha()
    {
        GameObject aux;

        Debug.Log("PosX: " + screenMousePos.x);
        Debug.Log("Posy: " + screenMousePos.y);

        Vector2 invertScreenMousePos1;
        invertScreenMousePos1.x = (screenMousePos.x - 1f);
        invertScreenMousePos1.y = screenMousePos.y;

        Vector2 invertScreenMousePos2;
        invertScreenMousePos2.x = screenMousePos.x;
        invertScreenMousePos2.y = (screenMousePos.y - 0.5f);

        Vector2 invertScreenMousePos3;

        invertScreenMousePos3.x = (screenMousePos.x - 1);
        invertScreenMousePos3.y = (screenMousePos.y - 0.5f);

        if (screenMousePos.x > 0.5f)
        {
            FlipRigth();
            yield return new WaitForSeconds(0.1f);
            if (screenMousePos.y > 0.5f)
            {
                aux = Instantiate(hacha, puntoLanzamiento.position, puntoLanzamiento.rotation);
                aux.GetComponent<Rigidbody2D>().AddForce(screenMousePos.normalized * 10, ForceMode2D.Impulse);
            }
            else
            {
                aux = Instantiate(hacha, puntoLanzamiento.position, puntoLanzamiento.rotation);
                aux.GetComponent<Rigidbody2D>().AddForce(invertScreenMousePos2 * 10, ForceMode2D.Impulse);
            }
        }
        else
        {
            FlipLeft();
            yield return new WaitForSeconds(0.1f);
            if (screenMousePos.y > 0.5f)
            {
                aux = Instantiate(hacha, puntoLanzamiento.position, puntoLanzamiento.rotation);
                aux.GetComponent<Rigidbody2D>().AddForce(invertScreenMousePos1 * 10, ForceMode2D.Impulse);
            } 
            else
            {
                aux = Instantiate(hacha, puntoLanzamiento.position, puntoLanzamiento.rotation);
                aux.GetComponent<Rigidbody2D>().AddForce(invertScreenMousePos3 * 10, ForceMode2D.Impulse);
            }
                
        }
    }
    */

    private void LanzarHacha()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direccion = (mousePos - this.transform.position);

        direccion.z = 0;
        direccion = direccion.normalized;

        GameObject aux;

        aux = Instantiate(hacha, puntoLanzamiento.position, puntoLanzamiento.rotation);
        aux.GetComponent<Rigidbody2D>().AddForce(direccion * 10, ForceMode2D.Impulse);
        hachaLanzada = aux;
    }

    public void RecogerHacha()
    {
        hasAxe = true;
    }

    void FlipRigth()
    {
        gameObject.transform.localScale = new Vector3(5, 5, 5);
    }
    
    void FlipLeft()
    {
        gameObject.transform.localScale = new Vector3(-5, 5, 5);
    }
}
