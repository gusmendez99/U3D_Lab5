using System.Collections;
using UnityEngine;

public class DestroyAfterTimeParticle : MonoBehaviour {
	public float timeToDestroy = 0.8f;
	/*
	*Destruye el efecto creado en la escena, para los disparos
	*/
	void Start () {
		Destroy (gameObject, timeToDestroy);
	}

}
