using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int bulletDamage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        // Debug log to see what the bullet is hitting
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

        if (collision.gameObject.CompareTag("Zombie"))
        {
            // 1. Try to find the "Zombie1" script (matches your screenshot)
            Zombie1 z1 = collision.gameObject.GetComponent<Zombie1>();
            
            if (z1 != null)
            {
                Debug.Log("Found 'Zombie1' script! Dealing " + bulletDamage + " damage.");
                z1.TakeDamage(bulletDamage);
            }
            // 2. Fallback: If you ever switch back to the "Zombie" script
            else 
            {
                Zombie z = collision.gameObject.GetComponent<Zombie>();
                if (z != null)
                {
                    Debug.Log("Found 'Zombie' script! Dealing " + bulletDamage + " damage.");
                    z.TakeDamage(bulletDamage);
                }
                else
                {
                    Debug.LogError("CRITICAL ERROR: Bullet hit an object tagged 'Zombie', but NO 'Zombie1' or 'Zombie' script was found on it!");
                }
            }

            Destroy(gameObject);
        }
    }
}