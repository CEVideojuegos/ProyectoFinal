using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChase : MonoBehaviour
{
    [SerializeField] private  GameObject warrior;
    [SerializeField] private float speed;
    private float distance;
    
    void Start()
    {
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, warrior.transform.position);
        Vector2 direction = warrior.transform.position - transform.position;

        if (distance < 10)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, warrior.transform.position, speed * Time.deltaTime);
        }
    }
}
