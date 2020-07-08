using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    private GameObject gun;

    public Camera cam;
    private Animator anim;

    public Transform player;

    public float damage;
    public float shotForce = 10f;

    public float range;

    public GameObject crosshair;
    public GameObject shotIcon;

    public ParticleSystem gunShot;
    public GameObject shotHit;
    public GameObject bloodSplat;

    private Rigidbody objectRb;

    private Vector3 cursorPos;


    private void Start()
    {
        anim = GetComponent<Animator>();
        crosshair.SetActive(true);
        shotIcon.SetActive(false);

        gun = gameObject;
    }

    private void Update()
    {
        SetCrosshair();

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Shoot");
            Shoot();
        }
    }

    private void Shoot()
    {
        gunShot.Play();

        Ray ray = cam.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log(hit.transform.name);

            cursorPos = hit.point;

            EnemyHealth enemyHealth = hit.transform.GetComponent<EnemyHealth>();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            if(hit.collider.CompareTag("Armour"))
            {
                hit.collider.gameObject.transform.SetParent(null);
                objectRb = hit.collider.gameObject.GetComponent<Rigidbody>();
                objectRb.isKinematic = false;
            }


            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * shotForce);
            }

            if(hit.collider.CompareTag("Enemy"))
            {
                Instantiate(bloodSplat, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bloodSplat, 1f);
            }

            else
            {
                GameObject hitGO = Instantiate(shotHit, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(hitGO, 1f);
            }
        }

        else
        {
            cursorPos = ray.GetPoint(range);
        }

        gun.transform.LookAt(hit.point);
    }

    void SetCrosshair()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            if(hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Armour"))
            {
                crosshair.SetActive(false);
                shotIcon.SetActive(true);
            }
            else
            {
                crosshair.SetActive(true);
                shotIcon.SetActive(false);
            }
        }     
    }
}
