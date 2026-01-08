using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToiletDoorMech : MonoBehaviour 
{
    [Header("Door Settings")]
    public Vector3 OpenRotation, CloseRotation;
    public float rotSpeed = 1f;
    public bool doorBool;

    [Header("Jumpscare Settings")]
    public GameObject jumpscareObject;   
    public AudioSource jumpscareSound;   
    public float scareDuration = 1.5f; // Shorter is often scarier (try 1.5)
    
    [Header("Shake Settings")]
    public Transform playerCamera;       
    public float cameraShakeAmount = 0.3f;  // Increased shake intensity
    public RectTransform faceImageRect;  
    public float uiShakeAmount = 50f;       // Increased UI jitter

    [Header("Horror Lunge Effects")]
    // The face starts at 20% size, and grows to 200% size very quickly
    public float startFaceScale = 0.2f;  // NEW: Start very small
    public float maxFaceScale = 2.0f;    // NEW: End massive
    public float zoomInFOV = 35f;        // Tight zoom
    public float effectSpeed = 15f;      // NEW: Very fast speed for sudden growth

    private bool hasScared = false;      
    private Camera camComponent;        

    void Start()
    {
        doorBool = false;
        CloseRotation = transform.rotation.eulerAngles;
        
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        if(playerCamera != null)
            camComponent = playerCamera.GetComponent<Camera>();
    }
        
    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (!doorBool)
            {
                doorBool = true;
                if (!hasScared)
                {
                    StartCoroutine(PlayJumpscare());
                }
            }
            else
            {
                doorBool = false;
            }
        }
    }

    void Update()
    {
        if (doorBool)
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (OpenRotation), rotSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (CloseRotation), rotSpeed * Time.deltaTime); 
    }

    IEnumerator PlayJumpscare()
    {
        hasScared = true; 

        // 1. Activate Face and Sound
        if(jumpscareObject != null) 
        {
            jumpscareObject.SetActive(true);
            // --- KEY CHANGE HERE ---
            // Force the face to start TINY right when it appears
            faceImageRect.localScale = new Vector3(startFaceScale, startFaceScale, 1f); 
        }

        if(jumpscareSound != null) 
            jumpscareSound.Play();

        Vector3 camOriginalPos = Vector3.zero;
        Vector2 uiOriginalPos = Vector2.zero;
        float originalFOV = 60f;

        if(playerCamera != null) camOriginalPos = playerCamera.localPosition;
        if(faceImageRect != null) uiOriginalPos = faceImageRect.anchoredPosition;
        if(camComponent != null) originalFOV = camComponent.fieldOfView;

        float elapsed = 0.0f;

        // --- THE CHAOS LOOP ---
        while (elapsed < scareDuration)
        {
            // A. Shake 3D Camera
            if (playerCamera != null)
            {
                float x = Random.Range(-1f, 1f) * cameraShakeAmount;
                float y = Random.Range(-1f, 1f) * cameraShakeAmount;
                playerCamera.localPosition = new Vector3(camOriginalPos.x + x, camOriginalPos.y + y, camOriginalPos.z);
            }

            // B. Shake UI Image
            if (faceImageRect != null)
            {
                float uiX = Random.Range(-1f, 1f) * uiShakeAmount;
                float uiY = Random.Range(-1f, 1f) * uiShakeAmount;
                faceImageRect.anchoredPosition = new Vector2(uiOriginalPos.x + uiX, uiOriginalPos.y + uiY);

                // C. EXPLOSIVE LUNGE EFFECT
                // Smoothly but quickly move from current scale towards maxScale
                faceImageRect.localScale = Vector3.Lerp(faceImageRect.localScale, new Vector3(maxFaceScale, maxFaceScale, 1f), Time.deltaTime * effectSpeed);
            }

            // D. VERTIGO EFFECT (Zoom Camera In)
            if (camComponent != null)
            {
                camComponent.fieldOfView = Mathf.Lerp(camComponent.fieldOfView, zoomInFOV, Time.deltaTime * effectSpeed);
            }

            elapsed += Time.deltaTime;
            yield return null; 
        }

        // --- RESET EVERYTHING ---
        if (playerCamera != null) playerCamera.localPosition = camOriginalPos;
        if (faceImageRect != null) 
        {
            faceImageRect.anchoredPosition = uiOriginalPos;
            faceImageRect.localScale = Vector3.one; // Reset size to normal for next time
        }
        if (camComponent != null) camComponent.fieldOfView = originalFOV; 

        if(jumpscareObject != null) 
            jumpscareObject.SetActive(false);
    }
}