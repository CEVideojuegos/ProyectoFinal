using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacha : MonoBehaviour
{
    private Rigidbody2D rb2;
    [SerializeField] bool volando;
    [SerializeField] float velSpin;
    [SerializeField] float returnSpin;

    private bool recogiendo;
    private Transform player;
    private bool enPared;

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

        if(recogiendo)
        {
            transform.Rotate(Vector3.forward, velSpin);
            transform.position = Vector3.MoveTowards(this.transform.position, this.player.position, Time.deltaTime * returnSpin);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            Debug.Log("Suelo");
            volando = false;
            Debug.Log(volando);
            ContactPoint2D aux = other.contacts[0];
            this.GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, -aux.normal);
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

            gameObject.layer = 0;
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player");
            if (!enPared)
            {
                other.gameObject.GetComponent<PersonajeController>().RecogerHacha();
                Debug.Log("Recogida");
                Destroy(this.gameObject);
            }

        }else if (other.gameObject.CompareTag("Pared"))
        {
            Debug.Log("Pared");
            volando = false;
            ContactPoint2D aux = other.contacts[0];
            this.GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, -aux.normal);
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

            gameObject.layer = 0;

            enPared = true;
        }
    }

    public void LlamarHacha(Transform other)
    {
        enPared = false;
        volando = false;
        gameObject.layer = 0;
        recogiendo = true;
        player = other;
        rb2.bodyType = RigidbodyType2D.Kinematic;
    }

    public bool EnPared()
    {
        return enPared;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!enPared)
            {
                collision.gameObject.GetComponent<PersonajeController>().RecogerHacha();
                Destroy(this);
            }
        }
    }
}
