using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlesChange : MonoBehaviour
{
    [SerializeField] private int level;

    void Start()
    {
        StartCoroutine(CambioConRetraso());
    }

    IEnumerator CambioConRetraso()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(level);
    }
}
