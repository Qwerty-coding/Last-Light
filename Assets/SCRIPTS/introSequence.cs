using UnityEngine;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup blackScreen;
    public CharacterController playerController; 
    public MouseMovement mouseLookScript; 

    [Header("Settings")]
    public float wakeUpDuration = 4f;

    private void Start()
    {
        // 1. COMPLETELY STOP EVERYTHING
        if (playerController != null) playerController.enabled = false; 
        if (mouseLookScript != null) mouseLookScript.enabled = false;   
        
        if (blackScreen != null) blackScreen.alpha = 1;
        StartCoroutine(WakeUpSequence());
    }

    IEnumerator WakeUpSequence()
    {
        Transform camTransform = Camera.main.transform;
        Transform playerTransform = playerController.transform;

        // POSITION SETUP
        // Use your player's current camera height as the "Standing" goal
        float standingY = camTransform.localPosition.y; 
        Vector3 standingPos = new Vector3(0, standingY, 0);
        Vector3 lyingPos = new Vector3(0, -0.7f, 0);

        // 2. FORCE THE BODY DIRECTION IMMEDIATELY
        // This ensures the player is facing the correct way before the eyes even open
        playerTransform.rotation = Quaternion.Euler(0, -90, 0);

        // 3. SET STARTING CAMERA POSE (Tilted)
        camTransform.localPosition = lyingPos;
        camTransform.localRotation = Quaternion.Euler(0, 0, 85f); 

        yield return new WaitForSeconds(1.5f); 

        float t = 0;
        while (t < wakeUpDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / wakeUpDuration); 

            // Fade UI
            if (blackScreen != null) blackScreen.alpha = 1 - p;

            // Move Camera UP
            camTransform.localPosition = Vector3.Lerp(lyingPos, standingPos, p);
            
            // Rotate Camera HEAD (Straighten the tilt)
            // We rotate from 85 on Z to 0 on Z.
            camTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 85f), Quaternion.identity, p);

            // FORCE body rotation every frame to prevent physics "snapping"
            playerTransform.rotation = Quaternion.Euler(0, -90, 0);

            yield return null;
        }

        // 4. FINAL LOCK
        camTransform.localPosition = standingPos;
        camTransform.localRotation = Quaternion.identity;
        playerTransform.rotation = Quaternion.Euler(0, -90, 0);

        // 5. HAND OVER CONTROL
        if (playerController != null) playerController.enabled = true;
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        
        if (blackScreen != null) blackScreen.gameObject.SetActive(false);
    }
}