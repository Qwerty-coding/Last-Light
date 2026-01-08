using UnityEngine;
using System.Collections; // Required for the Shake timer

public class GhostDash : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostModel;      
    public Transform targetDestination; 
    public float dashSpeed = 15f;
    
    [Header("Sound Settings")]
    public AudioSource scareSound;
    public float soundDuration = 1.0f; // How long sound plays

    [Header("Shake Settings")]
    public Transform playerCamera;     // Drag your Main Camera here
    public float shakeDuration = 0.5f; // How long the screen shakes
    public float shakeMagnitude = 0.3f;// How violent the shake is (0.1 is small, 0.5 is huge)

    private bool hasTriggered = false;
    private Vector3 originalCameraPos;

    void Update()
    {
        if (hasTriggered && ghostModel.activeSelf)
        {
            // Move the ghost
            ghostModel.transform.position = Vector3.MoveTowards(
                ghostModel.transform.position, 
                targetDestination.position, 
                dashSpeed * Time.deltaTime
            );

            // Hide ghost when it reaches destination
            if (Vector3.Distance(ghostModel.transform.position, targetDestination.position) < 0.1f)
            {
                ghostModel.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            // 1. Play Sound
            if (scareSound != null)
            {
                scareSound.Play();
                Invoke("StopSound", soundDuration); 
            }

            // 2. Start Screen Shake
            if (playerCamera != null)
            {
                StartCoroutine(Shake());
            }
        }
    }

    void StopSound()
    {
        if (scareSound != null) scareSound.Stop();
    }

    // The Shake Logic
    IEnumerator Shake()
    {
        // Remember where the camera belongs relative to the player body
        Vector3 originalLocalPos = playerCamera.localPosition;
        
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Pick a random point inside a small sphere
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            // Apply the shake relative to the original local position
            playerCamera.localPosition = new Vector3(originalLocalPos.x + x, originalLocalPos.y + y, originalLocalPos.z);

            elapsed += Time.deltaTime;
            
            // Wait for the next frame
            yield return null;
        }

        // Reset camera exactly to where it started so it doesn't get stuck offset
        playerCamera.localPosition = originalLocalPos;
    }
}