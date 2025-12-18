using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
 
public class AI_Movement : MonoBehaviour
{
 
    Animator animator;
 
    public float moveSpeed = 0.2f;
 
    Vector3 stopPosition;
 
    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;
 
    int WalkDirection;
 
    public bool isWalking;
    
    // NEW: Collision detection settings
    public float detectionDistance = 1f; // How far ahead to check for obstacles
    public LayerMask obstacleLayer; // Assign layers like "Default" or "Environment" in Inspector
    
    // NEW: Slope restriction settings
    public float maxSlopeAngle = 30f; // Maximum angle the rabbit can walk on
    private bool onValidGround = true;
 
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
 
        //So that all the prefabs don't move/stop at the same time
        walkTime = Random.Range(3,6);
        waitTime = Random.Range(5,7);
 
 
        waitCounter = waitTime;
        walkCounter = walkTime;
 
        ChooseDirection();
    }
 
    // Update is called once per frame
    void Update()
    {
        // NEW: Check if we're on valid ground (not too steep)
        CheckGroundSlope();
        
        if (isWalking && onValidGround)
        {
 
            animator.SetBool("isRunning", true);
 
            walkCounter -= Time.deltaTime;
            
            // NEW: Check for obstacles before moving
            if (DetectObstacle())
            {
                // Hit an obstacle, choose a new direction
                ChooseDirection();
                return;
            }
 
            switch (WalkDirection)
            {
                case  0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case  1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case  2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case  3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }
 
            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;
                //stop movement
                transform.position = stopPosition;
                animator.SetBool("isRunning", false);
                //reset the waitCounter
                waitCounter = waitTime;
            }
 
 
        }
        else if (!onValidGround)
        {
            // If on steep slope, stop and choose new direction
            isWalking = false;
            animator.SetBool("isRunning", false);
            waitCounter = 0; // Immediately try to find new direction
        }
        else
        {
 
            waitCounter -= Time.deltaTime;
 
            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }
 
    // NEW: Detect obstacles in front of the rabbit
    bool DetectObstacle()
    {
        // Cast a ray forward to check for obstacles
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Slightly above ground
        Vector3 rayDirection = transform.forward;
        
        RaycastHit hit;
        
        // Check if there's an obstacle ahead
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, detectionDistance, obstacleLayer))
        {
            // Hit something! Check if it's not the ground
            if (hit.collider != null && !hit.collider.CompareTag("Ground"))
            {
                Debug.DrawRay(rayOrigin, rayDirection * detectionDistance, Color.red, 0.5f);
                return true;
            }
        }
        
        // Also check with a sphere cast for better detection
        if (Physics.SphereCast(rayOrigin, 0.3f, rayDirection, out hit, detectionDistance, obstacleLayer))
        {
            if (hit.collider != null && !hit.collider.CompareTag("Ground"))
            {
                return true;
            }
        }
        
        Debug.DrawRay(rayOrigin, rayDirection * detectionDistance, Color.green, 0.1f);
        return false;
    }
    
    // NEW: Check if the ground slope is too steep
    void CheckGroundSlope()
    {
        RaycastHit hit;
        
        // Cast ray downward from rabbit
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f))
        {
            // Calculate the angle of the ground
            float groundAngle = Vector3.Angle(hit.normal, Vector3.up);
            
            // Check if slope is too steep
            if (groundAngle > maxSlopeAngle)
            {
                onValidGround = false;
                Debug.DrawRay(hit.point, hit.normal, Color.red, 0.5f);
            }
            else
            {
                onValidGround = true;
                Debug.DrawRay(hit.point, hit.normal, Color.green, 0.1f);
            }
        }
        else
        {
            // No ground detected, consider invalid
            onValidGround = false;
        }
    }
 
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);
 
        isWalking = true;
        walkCounter = walkTime;
    }
    
    // NEW: Optional - Visualize detection ranges in Scene view
    void OnDrawGizmosSelected()
    {
        // Draw detection sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f + transform.forward * detectionDistance, 0.3f);
        
        // Draw forward detection line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, 
                       transform.position + Vector3.up * 0.5f + transform.forward * detectionDistance);
    }
}