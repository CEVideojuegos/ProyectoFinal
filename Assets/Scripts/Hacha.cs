using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacha : MonoBehaviour
{
    private Rigidbody2D rb2;
    [SerializeField] bool volando;
    [SerializeField] float velSpin;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();

        volando = true;
    }

    void Update()
    {
        if (volando)
        {
            transform.Rotate(Vector3.forward, -velSpin);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(volando);

        if (other.gameObject.CompareTag("Suelo"))
        {
            volando = false;
            ContactPoint2D aux = other.contacts[0];
            this.GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, -aux.normal);
            rb2.bodyType = RigidbodyType2D.Static;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(this);
        }
    }
}
