using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)//Damage Enemy
        {
            enemyHealth.TakeDamage(5);
        }
    }

    private void OnCollisionEnter(Collision col)//Make knife stick to collision object
    {
        if (col.collider)
        {
            rb.isKinematic = true;
        }
    }
}

