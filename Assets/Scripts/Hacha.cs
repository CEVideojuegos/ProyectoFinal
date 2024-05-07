using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacha : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2;
    [SerializeField] private Animator animator;
    [SerializeField] float hachaVel;
    [SerializeField] GameObject player;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        hachaVel = rb2.velocity.x;
        animator.SetFloat("VelX", hachaVel);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            rb2.bodyType = RigidbodyType2D.Static;
            animator.SetTrigger("HachaStop");
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(this);
        }
    }
}
