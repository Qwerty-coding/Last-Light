using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class bullet : MonoBehaviour
{
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
    }
}
