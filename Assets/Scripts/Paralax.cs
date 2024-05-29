using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] float moveDiff;
    private Transform camTransform;
    private Vector3 camPosition;


    void Start()
    {
        camTransform = Camera.main.transform;
        camPosition = camTransform.position;
    }

    void LateUpdate()
    {
        float posX = camTransform.position.x - camPosition.x;
        posX = posX * moveDiff;
        this.transform.Translate(new Vector3(posX, 0, 0));
        camPosition = camTransform.position;
    }
}
