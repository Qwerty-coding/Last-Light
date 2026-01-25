using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int bulletDamage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit object: " + collision.gameObject.name + " with Tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Target"))
        {
            print("hit " + collision.gameObject.name + "!");
            Destroy(gameObject);
        }
        
        if (collision.gameObject.CompareTag("wall"))
        {
            print("hit a wall");
            Destroy(gameObject);
        }

        // REGULAR ZOMBIES
        if (collision.gameObject.CompareTag("Zombie"))
        {
            Zombie1 z1 = collision.gameObject.GetComponent<Zombie1>();
            
            if (z1 != null)
            {
                Debug.Log("Found 'Zombie1' script! Dealing " + bulletDamage + " damage.");
                z1.TakeDamage(bulletDamage);
            }
            else 
            {
                Zombie z = collision.gameObject.GetComponent<Zombie>();
                if (z != null)
                {
                    Debug.Log("Found 'Zombie' script! Dealing " + bulletDamage + " damage.");
                    z.TakeDamage(bulletDamage);
                }
            }

            Destroy(gameObject);
        }

        // BOSS ZOMBIE
        if (collision.gameObject.CompareTag("Boss"))
        {
            BossZombie boss = collision.gameObject.GetComponent<BossZombie>();
            
            if (boss != null)
            {
                Debug.Log("🎯 HIT BOSS! Dealing " + bulletDamage + " damage.");
                boss.TakeDamage(bulletDamage);
            }
            else
            {
                Debug.LogError("Boss tagged object has no BossZombie script!");
            }

            Destroy(gameObject);
        }
    }
}