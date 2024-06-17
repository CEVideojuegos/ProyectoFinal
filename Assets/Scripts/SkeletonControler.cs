using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonControler : MonoBehaviour
{
    [SerializeField] int SkeletonHP;
    [SerializeField] Animator _animator;

    private bool canAttack;
    private bool isHurt;
    private bool isDead;
    private bool canDealDamage;
    private bool canRecieveDamage;

    [SerializeField] private GameObject warrior;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private float distance;
    private bool canMove;

    [SerializeField] private GameObject HitboxAtaque;

    void Start()
    {
        GameObject aux = GameObject.FindGameObjectWithTag("Player");
        warrior = aux;
        _animator = GetComponent<Animator>();

        canMove = true;
        canAttack = true;
        HitboxAtaque.SetActive(false);
    }

    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, warrior.transform.position);
        Vector2 direction = (warrior.transform.position - transform.position).normalized;

        if(direction.x > 0)
        {
            this.transform.localScale = new Vector3(3.6f, 3.6f, 0f);
        } else
        {
            this.transform.localScale = new Vector3(-3.6f, 3.6f, 0f);
        }

        if(isDead)
        {
            Debug.Log("SkeletonHP");
            _animator.SetTrigger("IsDead");
            isDead = true;
            canMove = false;
            canAttack = false;
            Destroy(this.gameObject, 1f);
        }
        else
        {
            if (distance < 10 && canMove)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * speed, GetComponent<Rigidbody2D>().velocity.y);
                _animator.SetTrigger("IsWalking");
            }

            if (distance < 2 && canAttack)
            {
                Debug.Log(canAttack);
                canMove = false;
                canAttack = false;
                _animator.SetTrigger("Attack1");
                StartCoroutine(AttackCooldown());
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canDealDamage)
        {
            CantDealDamage();
            collision.gameObject.GetComponent<PersonajeController>().RecibirAtaque(this.transform);
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
        int blockChance = Random.Range(0, 9);

        if (blockChance <= 3) 
        {
            canMove = false;
            _animator.SetTrigger("IsBlocking");
            StartCoroutine(AttackCooldown());
        } 
        else
        {
            if (SkeletonHP <= 0)
            {
                isDead = true;
            }

            _animator.SetTrigger("IsHurt");

            rb.velocity = new Vector2(-direction.x * 5f, 5f);


            SkeletonHP--;
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(3f);
        canMove = true;

        yield return new WaitForSeconds(1f);
        canAttack = true;
    }

    IEnumerator HurtCooldown()
    {
        yield return new WaitForSeconds(4f);
        canMove = true;
    }

    public void CanDealDamage()
    {
        canDealDamage = true;
        HitboxAtaque.SetActive(true);
    }

    public void CantDealDamage()
    {
        canDealDamage = false;
        HitboxAtaque.SetActive(false);
    }
}
