using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    Rigidbody rb;

    public bool thrown;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(5);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.collider && thrown)
        {
            rb.isKinematic = true;
        }
    }
}

