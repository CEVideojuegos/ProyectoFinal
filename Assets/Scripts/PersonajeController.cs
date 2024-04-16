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
    private bool IsOnGround;
    private bool isIdle;
    private bool isJumping;
    private bool isWalking;
    private bool isRunning;
    private bool isAtacking;
    private bool isFalling;
    private bool isHurt;
    private bool turnPosition = true;
    private bool invulnerability;

    private String[] NombreAnimaciones = new String[] {"IsJumping", "IsFalling", "IsWalking", "IsRunning", "IsAtacking", "IsHurt", "IsIdle"} ;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(!IsOnGround && !isJumping)
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
        }
        if(isRunning)
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

        //runAnimator.SetBool("IsJumping", isJumping);

        if (Input.GetKeyDown(KeyCode.Space) && IsOnGround)
        {
            Saltar();
        }

        if (Input.GetMouseButtonDown(0) && !isAtacking)
        {
            isAtacking = true;
            StartCoroutine(Atacar());
        }

        //Moverse();
        if (!isHurt && !isAtacking)  //he recibido un ataque hace poco y no estoy atacando
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
            {
                speed = 6;
                isRunning = true;    
                isWalking = false;
                isJumping = false;
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

    
    void Saltar()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo") || other.gameObject.CompareTag("Enemy"))
        {
            isJumping = false;
            IsOnGround = true;
            isFalling = false;
            runAnimator.ResetTrigger("IsFalling");
            Debug.Log("isJumping = " + isJumping);
            Debug.Log("IsOnGround = " + IsOnGround);
            Debug.Log("isFalling = " + isFalling);
        }

        /*
        if (other.gameObject.CompareTag("Enemy") && !invulnerability)
        {
            runAnimator.ResetTrigger("IsFalling");
            isFalling = false;
            runAnimator.ResetTrigger("IsJumping");
            isJumping = false;
            runAnimator.ResetTrigger("IsWalking");
            runAnimator.ResetTrigger("IsRunning");
            isWalking = false;
            isRunning = false;
            runAnimator.ResetTrigger("IsAtacking");
            

            for(int i = 0; i < NombreAnimaciones.Length; i++)
            {
                runAnimator.ResetTrigger(NombreAnimaciones[i]);
            }

            invulnerability = true;
            runAnimator.SetTrigger("IsHurt");
            StartCoroutine(PlayInvulnerability());
        }*/
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            IsOnGround = false;
            isJumping = true;
            isWalking = false;
            StartCoroutine(PlayFallingAnimation());
        }
    }

    void Moverse()
    {
    if(!invulnerability && !isAtacking)
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
        runAnimator.SetTrigger("IsAtacking");
        speed = 0.0f;
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
        isHurt = true;
        maxHealthWarrior--;
        yield return new WaitForSeconds(1.5f);
        runAnimator.ResetTrigger("IsHurt");
        invulnerability = false;
        isHurt = false;
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
