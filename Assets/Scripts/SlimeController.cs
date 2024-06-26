using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private GameObject warrior;
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    private Animator runAnimator;
    private bool enElSuelo;
    private bool isJumping = false;
    private bool isDead;
    [SerializeField] private int maxHealthSlime;

    void Start()
    {
        GameObject aux = GameObject.FindGameObjectWithTag("Player");
        warrior = aux;
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enElSuelo)
        {
            Saltar();
        }

        if (isDead)
        {
            runAnimator.SetTrigger("IsDead");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hacha"))
        {
            //maxHealthSlime -= other.gameObject.GetComponent<Hacha>().getAxeDamage();
            Vector2 direction = (this.transform.position - other.transform.position).normalized;
            //Debug.Log(direction);
            RecibirDaño(direction);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            isJumping = false;
            enElSuelo = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            enElSuelo = false;
            isJumping = true;
        }
    }
    
    void Saltar()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    public void RecibirDaño(Vector2 direction)
    {
        maxHealthSlime--;

        if (maxHealthSlime <= 0)
        {
            GetComponent<AiChase>().CantMove();
            isDead = true;
            Destroy(this.gameObject, 1f);
        }

        GetComponent<AiChase>().CantMove();
        runAnimator.SetTrigger("IsHurt");
        rb.velocity = new Vector2(direction.x * 3f, 3f);
        //Debug.Log(direction);
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<AiChase>().CanMove();
    }
}
