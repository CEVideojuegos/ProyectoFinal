using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    [SerializeField] private GameObject warrior;
    [SerializeField] Animator _animator;
    [SerializeField] private Rigidbody2D rb;
    private Animator runAnimator;
    private bool isDead;
    private bool canMove;
    private float distance;
    [SerializeField] private int EyeMaxHealth;
    [SerializeField] private float speed;

    void Start()
    {
        GameObject aux = GameObject.FindGameObjectWithTag("Player");
        warrior = aux;
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        runAnimator = GetComponent<Animator>();
        canMove = true;
    }

    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, warrior.transform.position);
        Vector2 direction = (warrior.transform.position - transform.position).normalized;

        if (direction.x > 0)
        {
            this.transform.localScale = new Vector3(3.6f, 3.6f, 0f);
        }
        else
        {
            this.transform.localScale = new Vector3(-3.6f, 3.6f, 0f);
        }

        if(isDead)
        {
            canMove = false;
            _animator.SetTrigger("IsDead");
            Destroy(this.gameObject, 1);
        }

        if (distance < 10 && canMove)
        {
            //GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * speed, direction.y * GetComponent<Rigidbody2D>().velocity.y);
            transform.position = Vector2.MoveTowards(transform.position, warrior.transform.position, speed * Time.deltaTime);
            _animator.SetTrigger("IsFlying");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hacha"))
        {
            Vector2 direction = (this.transform.position - other.transform.position).normalized;
            RecibirDaño(direction);
        }
    }

    public void RecibirDaño(Vector2 direction)
    {
            if (EyeMaxHealth <= 0)
            {
                isDead = true;
            }
            canMove = false;
            _animator.SetTrigger("IsHurt");
            EyeMaxHealth--;
        StartCoroutine(HurtCooldown());
    }

    IEnumerator HurtCooldown()
    {
        yield return new WaitForSeconds(3f);
        canMove = true;
    }
}