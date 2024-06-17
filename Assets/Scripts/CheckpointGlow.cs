using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointGlow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Cambio renderer");
        if (other.gameObject.CompareTag("Player"))
        {
            m_SpriteRenderer.enabled = true;
        }
    }
}
