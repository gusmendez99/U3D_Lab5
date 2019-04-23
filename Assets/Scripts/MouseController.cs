using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{

    public Color startColor;
    public Color mouseOverColor;
    bool isMouseOver = false;
    private int clickForce = 125;
    private Rigidbody rb;
    public Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        //Set the initial color (0f,0f,0f,0f)
        startColor = rend.material.color;
    }
    

    private void OnMouseEnter()
    {
        isMouseOver = true;
        GetComponent<Renderer>().material.SetColor("_Color", mouseOverColor);
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        GetComponent<Renderer>().material.SetColor("_Color", startColor);

    }

    private void OnMouseDown()
    {
        if (isMouseOver)
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

           

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag != "Plane")
                    //hit.rigidbody.AddForce(-hit.normal * clickForce);
                    rb.AddForce(-hit.normal * clickForce, ForceMode.Impulse);
                //hit.rigidbody.AddForceAtPosition(-hit.normal * clickForce);

            }


        }
    }

}
