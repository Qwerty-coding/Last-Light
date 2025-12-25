using UnityEngine;

public class KeyDoorMech : MonoBehaviour
{
    [Header("Door Rotation Settings")]
    public Vector3 openRotation = new Vector3(0, 90, 0); 
    public Vector3 closeRotation = Vector3.zero;        
    public float rotSpeed = 2f;

    [Header("Lock Settings")]
    public bool isLocked = false; 

    private bool isOpen = false;
    private bool isPlayerInRange = false;

    private void Update()
    {
        // 1. INPUT: Player presses E
        if (Input.GetKeyDown(KeyCode.E))
        {
            // --- DEBUGGING ---
            // If the door doesn't open, check the Console for these values!
            Debug.Log($"[Door Check] Range: {isPlayerInRange} | LookTarget: {SelectionManager.instance.onTarget} | Key: {FixedInventory.Instance.HasKey}");

            // 2. CHECK: Player is close AND looking at the door
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
            if (FixedInventory.Instance.HasKey)
            {
                Debug.Log("Key Used! Opening door.");
                isLocked = false; // Unlock it forever
                isOpen = !isOpen; 
            }
            else
            {
                Debug.Log("Locked! You need the Key.");
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