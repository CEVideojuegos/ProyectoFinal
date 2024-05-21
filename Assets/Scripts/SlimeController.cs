using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int enemyHP;


    private Animator runAnimator;
    private bool enElSuelo;
    private bool isJumping = false;
    [SerializeField] private int maxHealthSlime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enElSuelo)
        {
            Saltar();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hacha"))
        {
            maxHealthSlime -= other.gameObject.GetComponent<Hacha>().getAxeDamage();
            Vector2 direction = (this.transform.position - other.transform.position).normalized;
            Debug.Log(direction);
            RecibirDaño(direction);
            if (maxHealthSlime <= 0)
            {
                //Destroy(this.gameObject);
            }
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
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    public void RecibirDaño(Vector2 direction)
    {
        GetComponent<AiChase>().CantMove();
        rb.velocity = new Vector2(direction.x * 10f, 10f);
        Debug.Log(direction);
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<AiChase>().CanMove();
    }

    public void ReceiveDMG()
    {
        enemyHP--;
        Debug.Log(enemyHP);
    }
}
