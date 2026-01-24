using System.Collections;
using UnityEngine;

public class LightningRandomizer : MonoBehaviour
{
    [Header("Assign from Scene")]
    public GameObject lightningObject;  // The actual Lightning object in your scene
    public ParticleSystem lightningVFX; // The particle system on that object
    public AudioSource audioSource;     // The audio source for the thunder

    [Header("Settings")]
    public float minInterval = 3f;      // Minimum time between strikes
    public float maxInterval = 8f;      // Maximum time between strikes
    
    [Header("Area Settings")]
    public Vector2 areaSize = new Vector2(100f, 100f); // Size of the area to randomize
    public float heightY = 50f;         // Keep this at your current Y height (50)

    private void Start()
    {
        // Start the routine
        StartCoroutine(StrikeRoutine());
    }

    IEnumerator StrikeRoutine()
    {
        while (true)
        {
            // 1. Wait for random interval
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // 2. Teleport the lightning object
            TeleportLightning();

            // 3. Play Visuals
            // We stop it first just in case, then play it fresh
            lightningVFX.Stop(); 
            lightningVFX.Play();

            // 4. Play Sound
            if (audioSource != null)
            {
                audioSource.Play();
                // If you strictly want it to stop after 2 seconds (cutting it off):
                // Invoke("StopSound", 2f); 
            }
        }
    }

    void TeleportLightning()
    {
        // Calculate a random X and Z position
        float randomX = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float randomZ = Random.Range(-areaSize.y / 2, areaSize.y / 2);

        // Update the position (keeping the Y height consistent)
        // We use 'transform.position' of this script as the center point
        Vector3 newPos = new Vector3(transform.position.x + randomX, heightY, transform.position.z + randomZ);
        
        lightningObject.transform.position = newPos;
    }

    void StopSound()
    {
        audioSource.Stop();
    }

    // Visualize the spawn area in the Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(transform.position.x, heightY, transform.position.z);
        Gizmos.DrawWireCube(center, new Vector3(areaSize.x, 20f, areaSize.y));
    }
}