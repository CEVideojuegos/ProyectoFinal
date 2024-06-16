using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] int SkeletonHP;
    [SerializeField] Animator _animator;

    private bool canCast;
    private bool canAttack;
    private bool isHurt;
    private bool isDead;
    private bool canDealDamage;
    private bool canRecieveDamage;

    [SerializeField] private GameObject warrior;
    [SerializeField] private GameObject spell;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private float distance;
    private bool canMove;

    [SerializeField] private GameObject HitboxAtaque;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        GameObject aux = GameObject.FindGameObjectWithTag("Player");
        warrior = aux;
        canMove = true;
        canAttack = true;
        canCast = true;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, warrior.transform.position);
        Vector2 direction = (warrior.transform.position - transform.position).normalized;

        if (isDead)
        {
            _animator.SetTrigger("IsDead");
            canMove = false;
            canAttack = false;
            Destroy(this.gameObject, 2f);
        } else
        {
            int castChance = Random.Range(0, 15);

            if(distance < 8 && castChance <= 5 && canCast) 
            {
                canCast = false;
                canMove = false;
                canAttack = false;
                _animator.SetTrigger("IsCasting");
                StartCoroutine(AttackSpell());
                Debug.Log("Casting");
            }

            if (direction.x < 0)
            {
                this.transform.localScale = new Vector3(3.6f, 3.6f, 0f);
            }
            else
            {
                this.transform.localScale = new Vector3(-3.6f, 3.6f, 0f);
            }

            
            if (distance < 10 && canMove && !isHurt && !canCast)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * speed, GetComponent<Rigidbody2D>().velocity.y);
                _animator.SetTrigger("IsWalking");
            }

            if (distance < 2 && canAttack && !isHurt)
            {
                canMove = false;
                canAttack = false;
                _animator.SetTrigger("IsAttacking");
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

    public void RecibirDaño(Vector2 direction)
    {
        isHurt = true;

        if (SkeletonHP <= 0)
        {
            isDead = true;
        }

        _animator.SetTrigger("IsHurt");

        rb.velocity = new Vector2(-direction.x * 5f, 5f);

        SkeletonHP--;
        StartCoroutine(HurtCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(2f);
        canMove = true;

        yield return new WaitForSeconds(1f);
        canAttack = true;
    }

    IEnumerator HurtCooldown()
    {
        yield return new WaitForSeconds(3f);
        canMove = true;
        isHurt = false;
    }

    IEnumerator AttackSpell()
    {
        Instantiate(spell, warrior.transform.position + new Vector3(0f, 2f, 0f), warrior.transform.rotation);

        yield return new WaitForSeconds(1f);
        canMove = true;
        canAttack = true;

        yield return new WaitForSeconds(5f);
        canCast = true;
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
