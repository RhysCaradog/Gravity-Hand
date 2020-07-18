using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour
{
    public GameObject knife;

    public Camera cam;

    Rigidbody rb;

    public float throwForce;

    Animator anim;

    public Transform weaponPos;

    bool hasKnife;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        anim = GetComponent<Animator>();

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

    public void ThrowKnife()
    {
        anim.enabled = false;
        knife.transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);

        hasKnife = false;
    }

    void OnCollisionEnter(Collision col)
    {
            EnemyHealth enemyHealth = col.collider.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(5);
            }

            if (col.collider && )
            {
                rb.isKinematic = true;
            }
        }
    }

