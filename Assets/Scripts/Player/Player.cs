using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public int health = 100;
    public PlayerMovement playerMovement;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        DamageScreenEffects.instance.TakeHit(health);
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

    }
}
