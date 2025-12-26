using UnityEngine;

public class KeyDoorMech : MonoBehaviour
{
    [Header("Door Rotation Settings")]
    public Vector3 openRotation = new Vector3(0, 90, 0); 
    public Vector3 closeRotation = Vector3.zero;        
    public float rotSpeed = 2f;

    [Header("Lock Settings")]
    public bool isLocked = false;
    // MATCH THIS NAME EXACTLY TO YOUR PICKUP SCRIPT
    public string keyID = "Key"; 

    private bool isOpen = false;
    private bool isPlayerInRange = false;

    private void Update()
    {
        // 1. INPUT: Player presses E
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 2. CHECK: Player is close AND looking at the door
            // (Assumes you still have SelectionManager in your scene)
            if (isPlayerInRange && SelectionManager.instance.onTarget)
            {
                TryToOpen();
            }
        }

        // 3. ANIMATION
        Quaternion target = isOpen ? Quaternion.Euler(openRotation) : Quaternion.Euler(closeRotation);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * rotSpeed);
    }

    void TryToOpen()
    {
        if (isLocked)
        {
            // --- NEW CHECK: Ask SimpleInventory ---
            if (SimpleInventory.Instance.HasItem(keyID))
            {
                Debug.Log($"Key '{keyID}' Used! Opening door.");
                
                // Optional: Remove key after use? 
                // SimpleInventory.Instance.RemoveItem(keyID); 
                
                isLocked = false; // Unlock forever
                isOpen = !isOpen; 
            }
            else
            {
                Debug.Log($"Locked! You need the item: {keyID}");
            }
        }
        else
        {
            // Not locked, just toggle open/close
            isOpen = !isOpen;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}