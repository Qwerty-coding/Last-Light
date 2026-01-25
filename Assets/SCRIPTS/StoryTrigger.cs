using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryTrigger : MonoBehaviour
{
    // Make sure to select 'FireTowerDoor' in the Inspector!
    public enum TriggerType { FireTowerDoor, Portal }
    public TriggerType type;

    private void OnTriggerEnter(Collider other)
    {
        // Only respond to the Player
        if (other.CompareTag("Player"))
        {
            if (type == TriggerType.FireTowerDoor)
            {
                // Check if the player has the key we gave them in ObjectiveManager
                if (SimpleInventory.Instance != null && SimpleInventory.Instance.HasItem("Key"))
                {
                    // 🔥 UPDATED TEXT HERE
                    ObjectiveManager.Instance.UpdateObjective("Door Unlocked! Enter the Portal at the top.");
                    
                    // Deactivate the door/invisible wall
                    gameObject.SetActive(false); 
                }
                else
                {
                    Debug.Log("Player needs a Key to open the Fire Tower.");
                }
            }
            else if (type == TriggerType.Portal)
            {
                // Load your Boss Scene (Ensure 'BossScene' is in Build Settings)
                SceneManager.LoadScene("BossScene"); 
            }
        }
    }
}