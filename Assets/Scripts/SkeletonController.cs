using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int maxHealthSkeleton;
    private Animator runAnimator;
    private bool enElSuelo;
    private bool isJumping = false;
    private bool isDead;
    private bool isWalking;
    private bool isBlocking;
    private bool isHurt;
    private bool isIdle;

    void Start()
    {
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

    void Saltar()
    {
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Hacha"))
        {
            maxHealthSkeleton -= other.gameObject.GetComponent<Hacha>().getAxeDamage();
            Vector2 direction = (this.transform.position - other.transform.position).normalized;
            Debug.Log(direction);
            RecibirDaño(direction);
            if (maxHealthSkeleton <= 0)
            {
                GetComponent<AiChase>().CantMove();
                isDead = true;
                Destroy(this.gameObject, 3f);
            }
        }
    }
    
    public void RecibirDaño(Vector2 direction)
    {
        GetComponent<AiChase>().CantMove();
        runAnimator.SetTrigger("IsHurt");
        rb.velocity = new Vector2(direction.x * 10f, 10f);
        Debug.Log(direction);
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<AiChase>().CanMove();
    }
}