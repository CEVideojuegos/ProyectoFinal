using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private int level;

    public void SeleccionarNivel()
    {
        SceneManager.LoadScene(level);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            SceneManager.LoadScene(level);
        }
    }
}
