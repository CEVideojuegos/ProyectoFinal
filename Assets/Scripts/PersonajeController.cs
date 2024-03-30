using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int numeroDeSaltos;
    private Animator runAnimator;
    private bool enElSuelo;
    private bool isMoving;
    private bool isJumping;
    private bool isAtacking;
    private bool isFalling;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        runAnimator.SetBool("IsMoving", isMoving);
        runAnimator.SetBool("IsJumping", isJumping);
        if (Input.GetKeyDown(KeyCode.Space) && enElSuelo)
        {
            Saltar();
        }

        if (Input.GetMouseButtonDown(0))
        {
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            isJumping = false;
            enElSuelo = true;
            isFalling = false;
        }
        
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            enElSuelo = false;
        }
        
        if (other.gameObject.CompareTag("Suelo") && Input.GetKey(KeyCode.Space))
        {
            enElSuelo = false;
            isJumping = true;
            isMoving = false;
            StartCoroutine(PlayFallingAnimation());
        }
    }

    void Moverse()
    {
        if (!(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)))
        {
            isMoving = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!isJumping)
            {
                isMoving = true;
            }
            transform.position += Vector3.right * speed * Time.deltaTime;
            FlipRigth();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (!isJumping)
            {
                isMoving = true;
            }
            transform.position += Vector3.right * -speed * Time.deltaTime;
            FlipLeft();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.up * -speed * Time.deltaTime;
        }
    }

    private IEnumerator Atacar()
    {
        runAnimator.SetBool("IsAtacking", true);
        yield return new WaitForSeconds(1.20f);
        runAnimator.SetBool("IsAtacking", false);
    }
    
    private IEnumerator PlayFallingAnimation()
    {
        yield return new WaitForSeconds(2);
        isFalling = true;
        runAnimator.SetBool("IsFalling", isFalling);
    }

    void FlipRigth()
    {
        gameObject.transform.localScale = new Vector3(5, 5, 5);
    }
    
    void FlipLeft()
    {
        gameObject.transform.localScale = new Vector3(-5, 5, 5);
    }

    void PlayAnimarSalto(Collision2D other, bool saltar)
    {
        
    }
    /*void Atacar()
    {
            isAtacking = true;
    }*/
}
