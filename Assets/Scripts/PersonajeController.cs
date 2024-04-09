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
    private Animator runAnimator;
    private bool enElSuelo;
    private bool isJumping;
    private bool isWalking;
    private bool isRunning;
    private bool isAtacking;
    private bool isFalling;
    private bool turnPosition = true;
    private bool invulnerability;


    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(!enElSuelo)
        {
            StartCoroutine(PlayFallingAnimation());
        }
        if(isWalking)
        {
            runAnimator.SetTrigger("IsWalking");
        }
        else
        {
            runAnimator.ResetTrigger("IsWalking");
        }if(isRunning)
        {
            runAnimator.SetTrigger("IsRunning");
        }
        else
        {
            runAnimator.ResetTrigger("IsRunning");
        }
        
        
        runAnimator.SetBool("IsJumping", isJumping);

        if (Input.GetKeyDown(KeyCode.Space) && enElSuelo)
        {
            Saltar();
        }

        if (Input.GetMouseButtonDown(0) && !isAtacking)
        {
            isAtacking = true;
            StartCoroutine(Atacar());
        }
    }

    private void FixedUpdate()
    {
        Moverse();
    }

    void Saltar()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            isJumping = false;
            enElSuelo = true;
            isFalling = false;
            runAnimator.ResetTrigger("IsFalling");
        }
        
        if (other.gameObject.CompareTag("Enemy") && !invulnerability)
        {
            runAnimator.ResetTrigger("IsJumping");
            isJumping = false;
            runAnimator.ResetTrigger("IsWalking");
            runAnimator.ResetTrigger("IsRunning");
            isWalking = false;
            isRunning = false;
            runAnimator.ResetTrigger("IsAtacking");
            invulnerability = true;
            runAnimator.SetTrigger("IsHurt");
            StartCoroutine(PlayInvulnerability());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            enElSuelo = false;
            isJumping = true;
            isWalking = false;
            StartCoroutine(PlayFallingAnimation());
        }
    }

    void Moverse()
    {
    if(!invulnerability)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
            {
                speed = 6;
                isRunning = true;
                runAnimator.SetBool("IsRunning", isRunning);
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
        runAnimator.SetTrigger("IsAtacking");
        yield return new WaitForSeconds(1.20f);
        isAtacking = false;
        runAnimator.ResetTrigger("IsAtacking");
    }
    
    private IEnumerator PlayFallingAnimation()
    {
        isFalling = true;
        runAnimator.SetTrigger("IsFalling");
        yield return new WaitForSeconds(1);
    }

    private IEnumerator PlayInvulnerability()
    {
        maxHealthWarrior--;
        yield return new WaitForSeconds(1.5f);
        runAnimator.ResetTrigger("IsHurt");
        invulnerability = false;
    }
    
    void FlipRigth()
    {
        gameObject.transform.localScale = new Vector3(5, 5, 5);
        if (!turnPosition)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + 0.848f, gameObject.transform.position.y, 0);
            turnPosition = true;
        }
    }
    
    void FlipLeft()
    {
        gameObject.transform.localScale = new Vector3(-5, 5, 5);
        if (turnPosition)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x - 0.729f, gameObject.transform.position.y, 0);
            turnPosition = false;
        }
    }
}
