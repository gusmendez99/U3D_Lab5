using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementScript : MonoBehaviour
{
    Rigidbody rb;
    public static int targetsDestroyed = 0;

    public float currentSpeed;
    [HideInInspector] public Transform cameraMain;
    public float jumpForce = 500;
    [HideInInspector] public Vector3 cameraPosition;


    RaycastHit hitInfo;
    private float meleeAttack_cooldown;
    private string currentWeapo;
    private LayerMask ignoreLayer;
    Ray ray1, ray2, ray3, ray4, ray5, ray6, ray7, ray8, ray9;
    private float rayDetectorMeeleSpace = 0.15f;
    private float offsetStart = 0.05f;
    [HideInInspector]
    public Transform bulletSpawn;


    public AudioSource _jumpSound;
    public AudioSource _freakingZombiesSound;
    public AudioSource _hitSound;
    public AudioSource _walkSound;
    public AudioSource _runSound;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraMain = transform.Find("Main Camera").transform;
        bulletSpawn = cameraMain.Find("BulletSpawn").transform;
        ignoreLayer = 1 << LayerMask.NameToLayer("Player");

    }
    private Vector3 slowdownV;
    private Vector2 horizontalMovement;
    /*
	* Raycasting para los ataques del arma
	*/
    void FixedUpdate()
    {
        RaycastForMeleeAttacks();
        PlayerMovementLogic();
    }


    void PlayerMovementLogic()
    {
        currentSpeed = rb.velocity.magnitude;
        horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);
        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }
        rb.velocity = new Vector3(
            horizontalMovement.x,
            rb.velocity.y,
            horizontalMovement.y
        );
        if (grounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity,
                new Vector3(0, rb.velocity.y, 0),
                ref slowdownV,
                deaccelerationSpeed);
        }

        if (grounded)
        {
            rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed * Time.deltaTime);
        }
        else
        {
            rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime);

        }
        /*
		 * Problemas resbaladizos arreglados aquí
		 */
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }
    }
    /*
	*Maneja saltando y agrega la fuerza y los sonidos.
	*/
    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddRelativeForce(Vector3.up * jumpForce);
            if (_jumpSound)
                _jumpSound.Play();
            else
                print("Missig jump sound.");
            _walkSound.Stop();
            _runSound.Stop();
        }
    }
    /*
	* Update
	*/
    void Update()
    {
        Jumping();

        Crouching();

        WalkingSound();


    }

    /*
     * Verifica si el jugador esta caminando o no
     */
    void WalkingSound()
    {
        if (_walkSound && _runSound)
        {

            if (RayCastGrounded() || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                //for walk sounsd using this because surface is not straigh			
                if (currentSpeed > 1)
                {
                    if (maxSpeed == 3)
                    {
                        if (!_walkSound.isPlaying)
                        {
                            _walkSound.Play();
                            _runSound.Stop();
                        }
                    }
                    else if (maxSpeed == 5)
                    {
                        if (!_runSound.isPlaying)
                        {
                            _walkSound.Stop();
                            _runSound.Play();
                        }
                    }
                }
                else
                {
                    _walkSound.Stop();
                    _runSound.Stop();
                }
            }
            else
            {
                _walkSound.Stop();
                _runSound.Stop();
            }
        }
        else
        {
            print("Missing walk and running sounds.");
        }

    }
    /*
	* Raycasts solo si el jugador esta tocando suelo
	*/
    private bool RayCastGrounded()
    {
        RaycastHit groundedInfo;
        if (Physics.Raycast(transform.position, transform.up * -1f, out groundedInfo, 1, ~ignoreLayer))
        {
            Debug.DrawRay(transform.position, transform.up * -1f, Color.red, 0.0f);


            if (groundedInfo.transform != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }


    void Crouching()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);

        }
    }


    public int maxSpeed = 5;
    public float deaccelerationSpeed = 15.0f;
    public float accelerationSpeed = 50000.0f;
    public bool grounded;

    void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector2.Angle(contact.normal, Vector3.up) < 60)
            {
                grounded = true;
            }
        }
    }

    void OnCollisionExit()
    {
        grounded = false;
    }


    /*
	* Son lanzados 9 rayos en diferentes direcciones, para mayor precision
	*/


    public bool been_to_meele_anim = false;
    private void RaycastForMeleeAttacks()
    {

        if (meleeAttack_cooldown > -5)
        {
            meleeAttack_cooldown -= 1 * Time.deltaTime;
        }


        if (GetComponent<GunInventory>().currentGun)
        {
            if (GetComponent<GunInventory>().currentGun.GetComponent<GunScript>())
                currentWeapo = "gun";
        }

        //middle row
        ray1 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace));
        ray2 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace));
        ray3 = new Ray(bulletSpawn.position, bulletSpawn.forward);
        //upper row
        ray4 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
        ray5 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
        ray6 = new Ray(bulletSpawn.position + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.up * rayDetectorMeeleSpace));
        //bottom row
        ray7 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
        ray8 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
        ray9 = new Ray(bulletSpawn.position - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.up * rayDetectorMeeleSpace));

        Debug.DrawRay(ray1.origin, ray1.direction, Color.cyan);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.cyan);
        Debug.DrawRay(ray3.origin, ray3.direction, Color.cyan);
        Debug.DrawRay(ray4.origin, ray4.direction, Color.red);
        Debug.DrawRay(ray5.origin, ray5.direction, Color.red);
        Debug.DrawRay(ray6.origin, ray6.direction, Color.red);
        Debug.DrawRay(ray7.origin, ray7.direction, Color.yellow);
        Debug.DrawRay(ray8.origin, ray8.direction, Color.yellow);
        Debug.DrawRay(ray9.origin, ray9.direction, Color.yellow);

        if (GetComponent<GunInventory>().currentGun)
        {
            if (GetComponent<GunInventory>().currentGun.GetComponent<GunScript>().meeleAttack == false)
            {
                been_to_meele_anim = false;
            }
            if (GetComponent<GunInventory>().currentGun.GetComponent<GunScript>().meeleAttack == true && been_to_meele_anim == false)
            {
                been_to_meele_anim = true;
                //	if (isRunning == false) {
                StartCoroutine("MeeleAttackWeaponHit");
                //	}
            }
        }

    }


    IEnumerator MeeleAttackWeaponHit()
    {
        if (Physics.Raycast(ray1, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray2, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray3, out hitInfo, 2f, ~ignoreLayer)
            || Physics.Raycast(ray4, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray5, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray6, out hitInfo, 2f, ~ignoreLayer)
            || Physics.Raycast(ray7, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray8, out hitInfo, 2f, ~ignoreLayer) || Physics.Raycast(ray9, out hitInfo, 2f, ~ignoreLayer))
        {


            print("Shooting: " + hitInfo.transform.tag);

            if (hitInfo.transform.tag == "Dummie")
            {
                Transform _other = hitInfo.transform.root.transform;
                if (_other.transform.tag == "Dummie")
                {
                    print("hit a dummie");
                }
                print("hit a dummie");
                InstantiateBlood(hitInfo, false);
                //Destroying object 
                Destroy(hitInfo.transform.gameObject);
            }

        }
        yield return new WaitForEndOfFrame();
    }

    RaycastHit hit;
    public GameObject bloodEffect;
    private GameObject myBloodEffect;

    void InstantiateBlood(RaycastHit _hitPos, bool swordHitWithGunOrNot)
    {

        if (currentWeapo == "gun")
        {
            GunScript.HitMarkerSound();

            if (_hitSound)
                _hitSound.Play();

            if (!swordHitWithGunOrNot)
            {
                if (bloodEffect)
                    Instantiate(bloodEffect, _hitPos.point, Quaternion.identity);

            }
        }
    }





}

