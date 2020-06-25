using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float health;

    public Slider healthBar;

    public Transform player;

    private void Start()
    {
        healthBar.value = health;
    }

    private void Update()
    {
        healthBar.transform.LookAt(player);
        healthBar.value = health;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
