using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacha : MonoBehaviour
{
    private Rigidbody2D rb2;
    [SerializeField] bool volando;
    [SerializeField] float velSpin;
    [SerializeField] float returnSpin;
    [SerializeField] int axeDamage;
    private bool canAttack;
    private bool recogiendo;
    private Transform player;
    private bool enPared;

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        canAttack = true;
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
        }else if (other.gameObject.CompareTag("Enemy") && canAttack)
        {
            canAttack = false;
            //Golpea al enemigo
            Vector2 direction = (other.transform.position - this.transform.position).normalized;
            other.gameObject.GetComponent<SlimeController>().RecibirDaño(direction);
            //Vuelve al personaje a hacha
            GameObject aux = GameObject.FindWithTag("Player");
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
            StartCoroutine(LlamarHachaDelay(aux.transform));
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

    public int getAxeDamage()
    {
        return axeDamage;
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

    IEnumerator LlamarHachaDelay(Transform aux)
    {
        yield return new WaitForSeconds(0.25f);
        LlamarHacha(aux);
    }
}
