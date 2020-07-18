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

    bool hasKnife;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        knife = gameObject;

        hasKnife = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (hasKnife)
        {
            if (Input.GetMouseButtonDown(0))
            {
                anim.SetTrigger("Stab");
            }

            if (Input.GetMouseButtonDown(1) && hasKnife)
            {
                anim.SetTrigger("Throw");
            }
        }      
    }

    void ThrowKnife()
    {
        knife.transform.parent = null;
        rb.AddForce(knife.transform.forward * throwForce, ForceMode.Impulse);

        hasKnife = false;
    }

    void OnCollisionEnter(Collision col)
    {
            EnemyHealth enemyHealth = col.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(5);
            }

            if (col.collider && hasKnife)
            {
                rb.isKinematic = true;
            }
        }
    }

