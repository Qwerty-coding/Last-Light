using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletDoorMech : MonoBehaviour 
{
    [Header("Door Settings")]
    public Vector3 OpenRotation, CloseRotation;
    public float rotSpeed = 1f;
    public bool doorBool;

    [Header("Jumpscare Settings")]
    public GameObject jumpscareObject;   // Drag your 'JumpscareScreen' here
    public AudioSource jumpscareSound;   // Drag the AudioSource here
    public float scareDuration = 2.0f;   // How long the face stays on screen
    
    private bool hasScared = false;      // To make sure it only happens once

    void Start()
    {
        doorBool = false;
        CloseRotation = transform.rotation.eulerAngles;
    }
        
    void OnTriggerStay(Collider col)
    {
        // Check for Player and E key
        if(col.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (!doorBool)
            {
                // Door is closed and about to OPEN
                doorBool = true;

                // Trigger the jumpscare ONLY if it hasn't happened yet
                if (!hasScared)
                {
                    StartCoroutine(PlayJumpscare());
                }
            }
            else
            {
                // Door is open and about to CLOSE
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

    // This block handles the Jumpscare sequence
    IEnumerator PlayJumpscare()
    {
        hasScared = true; // Lock it so it doesn't happen again

        // 1. Show the scary face
        if(jumpscareObject != null) 
            jumpscareObject.SetActive(true);

        // 2. Play the scream
        if(jumpscareSound != null) 
            jumpscareSound.Play();

        // 3. Wait for X seconds
        yield return new WaitForSeconds(scareDuration);

        // 4. Hide the face
        if(jumpscareObject != null) 
            jumpscareObject.SetActive(false);
    }
}