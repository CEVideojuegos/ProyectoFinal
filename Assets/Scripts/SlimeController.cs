using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int slimeHP;


    private Animator runAnimator;
    private bool enElSuelo;
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
    }

    /*
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hacha"))
        {
            slimeHP -= other.gameObject.GetComponent<Hacha>().getAxeDamage();
            Vector2 direction = (this.transform.position - other.transform.position).normalized;
            GetComponent<AiChase>().CantMove();
            rb.velocity = new Vector2(direction.x * 10f, 10f);
            StartCoroutine(Cooldown());
            Debug.Log(slimeHP);
            if (slimeHP <= 0)
            {
                Destroy(this.gameObject, 1f);
            }
        }
    }*/

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
    
    public void RecibirDaño(Vector2 direction)
    {
        slimeHP--;
        GetComponent<AiChase>().CantMove();
        rb.velocity = new Vector2(direction.x * 10f, 10f);
        StartCoroutine(Cooldown());
    }

    public void RecieveDMG()
    {
        slimeHP--;

    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<AiChase>().CanMove();
    }
}
