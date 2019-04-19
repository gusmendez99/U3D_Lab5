using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    

    public float maxDistance = 1000000;
    RaycastHit hit;
    public GameObject decalHitWall;
    public float floatInfrontOfWall;
    public GameObject bloodEffect;
    public LayerMask ignoreLayer;

    public TextMesh KillsPlayer;

    /*
	* La bala crea un RayCast, y dependiendo si se topa con un ave "Dummie",
    * se activa el metodo Destroy()
	*/

    void Update()
    {

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, ~ignoreLayer))
        {
            if (decalHitWall)
            {
                if (hit.transform.tag == "LevelPart")
                {
                    Instantiate(decalHitWall, hit.point + hit.normal * floatInfrontOfWall, Quaternion.LookRotation(hit.normal));
                    Destroy(gameObject);
                }
                if (hit.transform.tag == "Dummie")
                {
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hit.transform.gameObject);
                    //Actualizando la puntuacion
                    PlayerMovementScript.targetsDestroyed++;

                    if (!KillsPlayer)
                    {
                        try
                        {
                            KillsPlayer = GameObject.Find("KillsPlayer").GetComponent<TextMesh>();
                        }
                        catch (System.Exception ex)
                        {
                            print("Couldnt find the KillsPlayer ->" + ex.StackTrace.ToString());
                        }
                    }

                    if (KillsPlayer) {
                        int currentKills = PlayerMovementScript.targetsDestroyed;
                        KillsPlayer.text = "Targets Destroyed: " + currentKills.ToString();
                    }
                        

                    Destroy(gameObject);

                }
            }
            Destroy(gameObject);
        }
        Destroy(gameObject, 0.1f);
    }

    
    
}
