using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class PersonajeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float slideVelocity;
    [SerializeField] private float attackDMG;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int maxHealthWarrior;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider2D colliderNormal;
    [SerializeField] private Collider2D colliderSlide;
    [SerializeField] private GameObject HitboxAtaque;
    [SerializeField] private Slider sliderHP;
    [SerializeField] private GameObject pantallaMuerte;

    [SerializeField] GameObject hacha;

    private Animator runAnimator;

    [SerializeField] private bool IsOnGround;
    [SerializeField] private float VelY;


    [SerializeField] private bool isRunning;
    [SerializeField] private bool isWalking;
    private bool isAttacking;
    private bool isSliding;
    private bool sprintAttack;
    private bool isHurt;
    private bool isDead;

    private bool canDealDamage;

    private GameObject hachaLanzada;

    private bool canSlide = true;
    private bool hasAxe = true;

    private float speedRun = 6f;
    private float speedWalk = 3.7f;

    [SerializeField] private float knockbackX;
    [SerializeField] private float knockbackY;

    [SerializeField] Transform puntoLanzamiento;
    [SerializeField] Vector2 lastCheckpoint;

    [SerializeField] AudioSource source;
    [SerializeField] List<AudioClip> walkSounds;
    [SerializeField] List<AudioClip> runSounds;
    [SerializeField] List<AudioClip> swordSounds;
    [SerializeField] List<AudioClip> jumpSounds;
    [SerializeField] List<AudioClip> landSounds;
    [SerializeField] List<AudioClip> hurtSounds;
    [SerializeField] AudioClip deathSound;
    private bool canProduceSoundRun;
    private bool canProduceSoundWalk;
    private bool canProduceSoundAttack;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        colliderNormal.enabled = true;
        colliderSlide.enabled = false;
        hasAxe = true;
        canProduceSoundRun = true;
        canProduceSoundWalk = true;
        canProduceSoundAttack = true;
        HitboxAtaque.SetActive(false);
    }

    void Update()
    {
        sliderHP.value = maxHealthWarrior;

        if (isDead)
        {
            runAnimator.SetBool("IsDead", true);
            StartCoroutine(DeathTransition());

        } else if (!isDead) {

            runAnimator.SetBool("IsGrounded", IsOnGround);
            VelY = rb.velocity.y;

            if (rb.velocity.y !=0) 
            {
                runAnimator.SetFloat("VelY", VelY);
            }
            
            if ((isRunning && IsOnGround && canProduceSoundRun))
            {
                StartCoroutine(RunSound());
            }

            if ((isWalking && IsOnGround && canProduceSoundWalk))
            {
                StartCoroutine(WalkSound());
            }

            if (isAttacking && canProduceSoundAttack)
            {
                StartCoroutine(AttackSoundNormal());
            } 
            
            if (isAttacking && (isRunning || !IsOnGround) && canProduceSoundAttack)
            {
                StartCoroutine(AttackSoundSpecial());
            }

            //Salto----------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.Space) && IsOnGround && !isAttacking)
            {
                Saltar();
            }

            //Ataque basico-------------------------------------------------------------
            if (Input.GetMouseButtonDown(0) && !isAttacking && !isRunning && IsOnGround)
            {
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
                runAnimator.SetBool("IsWalking", false);
                runAnimator.SetBool("IsRunning", false);
                isAttacking = true;
                runAnimator.SetBool("IsAttacking", true);
                runAnimator.SetTrigger("DashAttack");
                StartCoroutine(Atacar());
                isRunning = false;
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
                        isWalking = false;
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
                        runAnimator.SetBool("IsWalking", true);
                        
                        
                    if(isRunning)
                    {
                        speed = speedRun;
                    }
                    else
                    {
                        speed = speedWalk;
                        isWalking = true;
                    }

                    transform.position += Vector3.right * speed * Time.deltaTime;
                    FlipRigth();
                }

                if (Input.GetKey(KeyCode.A))
                {
                    runAnimator.SetBool("IsWalking", true);

                    if (isRunning)
                    {
                        speed = speedRun;
                    }
                    else
                    {
                        speed = speedWalk;
                        isWalking = true;
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
        int r = Random.Range(0, jumpSounds.Count);
        source.PlayOneShot(jumpSounds[r]);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo") || other.gameObject.CompareTag("Hacha"))
        {
            int r = Random.Range(0, landSounds.Count);
            source.PlayOneShot(landSounds[r]);
            IsOnGround = true;
        }

        
        if ((other.gameObject.CompareTag("Slime") || other.gameObject.CompareTag("RedSlime") || other.gameObject.CompareTag("Skeleton")) && !isHurt && !isAttacking)
        {
            if(maxHealthWarrior > 1)
            {
                int r = Random.Range(0, hurtSounds.Count);
                source.PlayOneShot(hurtSounds[r]);
            }
            StartCoroutine(PlayInvulnerability(other.transform));
        }

        if (other.gameObject.CompareTag("DeadZone"))
        {
            maxHealthWarrior--;

            if (maxHealthWarrior <= 0)
            {
                isDead = true;
                colliderNormal.enabled = false;
                colliderSlide.enabled = true;
            }

            this.transform.position = lastCheckpoint;
        }

        if (other.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint actualizado");
            lastCheckpoint = other.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Suelo") || other.gameObject.CompareTag("Hacha"))
        {
            IsOnGround = true;
        } 
    }

    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo") || other.gameObject.CompareTag("Hacha"))
        {
            IsOnGround = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Slime") && canDealDamage)
        {
            Vector2 direction = (collision.transform.position - this.transform.position).normalized;
            CantDealDamage();
            collision.gameObject.GetComponent<SlimeController>().RecibirDaño(direction);
        }

        if (collision.gameObject.CompareTag("RedSlime") && canDealDamage)
        {
            Vector2 direction = (collision.transform.position - this.transform.position).normalized;
            CantDealDamage();
            collision.gameObject.GetComponent<RedSlimeController>().RecibirDaño(direction);
        }

        if (collision.gameObject.CompareTag("Skeleton") && canDealDamage)
        {
            Vector2 direction = (collision.transform.position - this.transform.position).normalized;
            CantDealDamage();
            collision.gameObject.GetComponent<SkeletonControler>().RecibirDaño(direction);
        }
    }

    private IEnumerator Atacar()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        runAnimator.SetBool("IsAttacking", isAttacking);
        HitboxAtaque.SetActive(false);
    }  

    public IEnumerator PlayInvulnerability(Transform enemigo)
    {
        maxHealthWarrior--;

        if (maxHealthWarrior <= 0)
        {
            isDead = true;
            colliderNormal.enabled = false;
            colliderSlide.enabled = true;
        }
        else
        {
            isHurt = true;
            isRunning = false;
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

    private IEnumerator RunSound()
    {
        int r;
        r = Random.Range(0, runSounds.Count);
        source.PlayOneShot(runSounds[r]);
        canProduceSoundRun = false;
        yield return new WaitForSeconds(0.35f);
        canProduceSoundRun = true;
    }

    private IEnumerator WalkSound()
    {
        int r;
        r = Random.Range(0, walkSounds.Count);
        source.PlayOneShot(walkSounds[r]);
        canProduceSoundWalk= false;
        yield return new WaitForSeconds(0.45f);
        canProduceSoundWalk = true;
    }

    private IEnumerator AttackSoundNormal()
    {
        int r;
        r = Random.Range(0, swordSounds.Count);
        source.PlayOneShot(swordSounds[r]);
        canProduceSoundAttack = false;
        yield return new WaitForSeconds(0.4f);
        r = Random.Range(0, swordSounds.Count);
        canProduceSoundAttack = true;
        source.PlayOneShot(swordSounds[r]);
        canProduceSoundAttack = false;
        yield return new WaitForSeconds(0.8f);
        canProduceSoundAttack = true;
    }

    private IEnumerator AttackSoundSpecial()
    {
        int r;
        r = Random.Range(0, swordSounds.Count);
        source.PlayOneShot(swordSounds[r]);
        canProduceSoundAttack = false;
        yield return new WaitForSeconds(1.2f);
        canProduceSoundAttack= true;
    }

    private IEnumerator DeathTransition()
    {
        source.PlayOneShot(deathSound);
        isDead = false;
        pantallaMuerte.SetActive(true);
        runAnimator.SetBool("IsDead", false);
        colliderNormal.enabled = true;
        colliderSlide.enabled = false;
        maxHealthWarrior = 5;
        yield return new WaitForSeconds(5f);
        this.transform.position = lastCheckpoint;
        pantallaMuerte.SetActive(false);
    }


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
    
    public void RecibirAtaque(Transform enemigo)
    {
        StartCoroutine(PlayInvulnerability(enemigo));
    }

    public void RecogerHacha()
    {
        hasAxe = true;
    }

    public void CanDealDamage ()
    {
        canDealDamage = true;
        HitboxAtaque.SetActive(true);
    }

    public void CantDealDamage()
    {
        canDealDamage = false;
        HitboxAtaque.SetActive(false);
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
