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

        if (enemyHealth != null)//Damages Enemy
        {
            enemyHealth.TakeDamage(5);
        }
    }

    private void OnCollisionEnter(Collision col)//Sticks knife in object that it collides with when thrown
    {
        if (col.collider && thrown)
        {
            gameObject.transform.SetParent(col.collider.transform);
            rb.isKinematic = true;
        }
    }
}

