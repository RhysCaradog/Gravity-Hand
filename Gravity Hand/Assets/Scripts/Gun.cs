using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Camera cam;
    private Animator anim;

    public float damage = 10f;
    public float shotForce = 10f;

    public float range;

    public GameObject crosshair;
    public GameObject shotIcon;

    public ParticleSystem gunShot;
    public GameObject shotHit;

    private void Start()
    {
        anim = GetComponent<Animator>();
        crosshair.SetActive(true);
        shotIcon.SetActive(false);
    }

    private void Update()
    {
        SetCrosshair();

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("Shoot");
            Shoot();
        }
    }

    private void Shoot()
    {
        gunShot.Play();

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }


            if(hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * shotForce);
            }

            Instantiate(shotHit, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    void SetCrosshair()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            if(hit.collider.CompareTag("Enemy"))
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
