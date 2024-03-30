using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    private Animator runAnimator;
    private bool enElSuelo;
    private bool isMoving;
    private bool isJumping;
    private bool isAtacking;
    
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
            StartCoroutine(PlayOneShot());
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
            isJumping = true;
            isMoving = false;
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
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (!isJumping)
            {
                isMoving = true;
            }
            transform.position += Vector3.right * -speed * Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.up * -speed * Time.deltaTime;
        }
    }

    private IEnumerator PlayOneShot ()
    {
        runAnimator.SetBool("IsAtacking", true);
        yield return new WaitForSeconds(1.20f);
        runAnimator.SetBool("IsAtacking", false);
    }
    
    void Atacar()
    {
            isAtacking = true;
    }
}
