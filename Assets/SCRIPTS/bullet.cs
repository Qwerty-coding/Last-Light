using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public int bulletDamage = 20;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            print("hit"+ collision.gameObject.name + "!");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            print("hit a wall");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Zombie"))
        {
            collision.gameObject.GetComponent<Zombie>().TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }
}
