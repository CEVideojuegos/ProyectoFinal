using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChase : MonoBehaviour
{
    [SerializeField] private  GameObject warrior;
    [SerializeField] private float speed;
    private float distance;
    public bool canMove;
    
    void Start()
    {
        canMove = true;
    }

    void FixedUpdate()
    {
        distance = Vector2.Distance(transform.position, warrior.transform.position);
        Vector2 direction = (warrior.transform.position - transform.position).normalized;

        if (distance < 10 && canMove)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * speed, GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    public void CanMove()
    {
        canMove = true;
    }

    public void CantMove()
    {
        canMove = false;
    }
}
