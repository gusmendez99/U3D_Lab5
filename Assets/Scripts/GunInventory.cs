using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuStyle
{
    horizontal, vertical
}

public class GunInventory : MonoBehaviour
{
    public GameObject currentGun;
    private Animator currentHAndsAnimator;
    private int currentGunCounter = 0;

    public List<string> gunsIHave = new List<string>();
    public Texture[] icons;

    [HideInInspector]
    public float switchWeaponCooldown;

    //Sonidos
    public AudioSource weaponChanging;

    /*
	 *Inicializando elementos, tambien creando la primer arma que se utilizara
	 */
    void Awake()
    {
        StartCoroutine("UpdateIconsFromResources");

        StartCoroutine("SpawnWeaponUponStart");

        if (gunsIHave.Count == 0)
            print("No guns in the inventory");
    }

    /*
	*Esperar unos segundos antes de crear el arma
	*/
    IEnumerator SpawnWeaponUponStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine("Spawn", 0);
    }

    /* 
	 * Restringiendo el tiempo de cambio de arma
	 */
    void Update()
    {

        switchWeaponCooldown += 1 * Time.deltaTime;
        if (switchWeaponCooldown > 1.2f && Input.GetKey(KeyCode.LeftShift) == false)
        {
            Create_Weapon();
        }

    }

    /*
	 * Cargando el recurso de textura para el arma
	 */
    IEnumerator UpdateIconsFromResources()
    {
        yield return new WaitForEndOfFrame();

        icons = new Texture[gunsIHave.Count];
        for (int i = 0; i < gunsIHave.Count; i++)
        {
            icons[i] = (Texture)Resources.Load("Weap_Icons/" + gunsIHave[i].ToString() + "_img");
        }

    }

    /*
	 * Cambiar arma cuando se seleccione con el scroll del mouse
	 */
    void Create_Weapon()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            switchWeaponCooldown = 0;

            currentGunCounter++;
            if (currentGunCounter > gunsIHave.Count - 1)
            {
                currentGunCounter = 0;
            }
            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            switchWeaponCooldown = 0;

            currentGunCounter--;
            if (currentGunCounter < 0)
            {
                currentGunCounter = gunsIHave.Count - 1;
            }
            StartCoroutine("Spawn", currentGunCounter);
        }

        /*
		 * Keypad 
		 */
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentGunCounter != 0)
        {
            switchWeaponCooldown = 0;
            currentGunCounter = 0;
            StartCoroutine("Spawn", currentGunCounter);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && currentGunCounter != 1)
        {
            switchWeaponCooldown = 0;
            currentGunCounter = 1;
            StartCoroutine("Spawn", currentGunCounter);
        }

    }

    /*
	 * Metodo para cambiar las armas, verificando si ya existe una creada con anterioridad
	 */
    IEnumerator Spawn(int _redniBroj)
    {
        if (weaponChanging)
            weaponChanging.Play();

        if (currentGun)
        {
            if (currentGun.name.Contains("Gun"))
            {

                currentHAndsAnimator.SetBool("changingWeapon", true);

                yield return new WaitForSeconds(0.8f);
                Destroy(currentGun);

                GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
                currentGun = (GameObject)Instantiate(resource, transform.position, Quaternion.identity);
                AssignHandsAnimator(currentGun);
            }
            else if (currentGun.name.Contains("Sword"))
            {
                currentHAndsAnimator.SetBool("changingWeapon", true);
                yield return new WaitForSeconds(0.25f);//0.5f

                currentHAndsAnimator.SetBool("changingWeapon", false);

                yield return new WaitForSeconds(0.6f);//1
                Destroy(currentGun);

                GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
                currentGun = (GameObject)Instantiate(resource, transform.position, Quaternion.identity);
                AssignHandsAnimator(currentGun);
            }
        }
        else
        {
            GameObject resource = (GameObject)Resources.Load(gunsIHave[_redniBroj].ToString());
            currentGun = (GameObject)Instantiate(resource, transform.position, Quaternion.identity);

            AssignHandsAnimator(currentGun);
        }

    }

    void AssignHandsAnimator(GameObject _currentGun)
    {
        if (_currentGun.name.Contains("Gun"))
        {
            currentHAndsAnimator = currentGun.GetComponent<GunScript>().handsAnimator;
        }
    }

    /*
	 * Metodo de dibujado en Unity3D
	 */
    void OnGUI()
    {

        if (currentGun)
        {
            for (int i = 0; i < gunsIHave.Count; i++)
            {
                DrawCorrespondingImage(i);
            }
        }

    }

    public MenuStyle menuStyle = MenuStyle.horizontal;
    public int spacing = 10;
    public Vector2 beginPosition;
    public Vector2 size;

    void DrawCorrespondingImage(int _number)
    {

        string deleteCloneFromName = currentGun.name.Substring(0, currentGun.name.Length - 7);

        if (menuStyle == MenuStyle.horizontal)
        {
            if (deleteCloneFromName == gunsIHave[_number])
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing)), vec2(beginPosition).y,
                    vec2(size).x, vec2(size).y),
                    icons[_number]);
            }
            else
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x + (_number * position_x(spacing) + 10), vec2(beginPosition).y + 10,
                    vec2(size).x - 20, vec2(size).y - 20),
                    icons[_number]);
            }
        }
        else if (menuStyle == MenuStyle.vertical)
        {
            if (deleteCloneFromName == gunsIHave[_number])
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + (_number * position_y(spacing)),
                    vec2(size).x, vec2(size).y),
                    icons[_number]);
            }
            else
            {
                GUI.DrawTexture(new Rect(vec2(beginPosition).x, vec2(beginPosition).y + 10 + (_number * position_y(spacing)),
                    vec2(size).x - 20, vec2(size).y - 20),
                    icons[_number]);
            }
        }
    }

    /*
	 * Metodo cuando el jugador muere, si lo hiciera
	 */
    public void DeadMethod()
    {
        Destroy(currentGun);
        Destroy(this);
    }


    private float position_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float position_y(float var)
    {
        return Screen.height * var / 100;
    }
    private float size_x(float var)
    {
        return Screen.width * var / 100;
    }
    private float size_y(float var)
    {
        return Screen.height * var / 100;
    }
    private Vector2 vec2(Vector2 _vec2)
    {
        return new Vector2(Screen.width * _vec2.x / 100, Screen.height * _vec2.y / 100);
    }


}
