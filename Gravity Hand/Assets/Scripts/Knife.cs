using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    private GameObject knife;

    public Camera cam;

    Rigidbody rb;

    public float throwForce;

    Animator anim;

    public Transform weaponPos;

    bool thrown;

    private Vector3 knifeDir;

    public float range = 2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        knife = gameObject;

        knife.transform.position = weaponPos.position;

        knifeDir = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
    }

    // Update is called once per frame
    void Update()
    {
        knife.transform.LookAt(knifeDir);

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Stab");
        }

        if (Input.GetMouseButtonDown(1))
        {
            /*thrown = true;
            anim.SetTrigger("Throw");*/
            ThrowKnife();
        }

        if (thrown)
        {
            ThrowKnife();
        }
    }

    void Stab()
    {
       
    }

    void ThrowKnife()
    {
        rb.isKinematic = false;
        knife.transform.parent = null;
        rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        
        thrown = false;
    }

    void OnCollisionEnter(Collision col)
    {
            EnemyHealth enemyHealth = col.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(5);
            }

            if (col.collider && thrown)
            {
                rb.isKinematic = true;
            }
        }
    }

