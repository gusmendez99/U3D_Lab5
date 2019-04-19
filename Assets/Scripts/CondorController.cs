using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondorController : MonoBehaviour

{

    float timeCounter = 0f;
    float speed;
    float width;
    float height;

    void Start()
    {
        speed = 0.8f;
        width = 40f;
        height = 40f;
    }

    /*
     * Con ayuda de las funciones seno y coseno, se logra obtener el movimiento circular 
     * alrededor del eje Y
     */
    void Update()
    {

        timeCounter += Time.deltaTime * speed;

        float x = (Mathf.Cos(timeCounter) / width) + transform.position.x;
        float z = (Mathf.Sin(timeCounter) / height) + transform.position.z;
        float y = transform.position.y;

        transform.position = new Vector3(x, y, z);


    }
}
