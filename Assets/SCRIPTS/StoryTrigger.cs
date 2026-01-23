using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryTrigger : MonoBehaviour
{
    public enum TriggerType { TerraceDoor, Portal }
    public TriggerType type;

    private void OnTriggerEnter(Collider other)
    {
        // Only respond to the Player
        if (other.CompareTag("Player"))
        {
            if (type == TriggerType.TerraceDoor)
            {
                // Check if the player has the key we gave them in ObjectiveManager
                if (SimpleInventory.Instance != null && SimpleInventory.Instance.HasItem("Key"))
                {
                    ObjectiveManager.Instance.UpdateObjective("The Terrace is open! Enter the Portal.");
                    // Optional: Deactivate a door visual or "Invisible Wall" here
                    gameObject.SetActive(false); 
                }
            }
            else if (type == TriggerType.Portal)
            {
                // Load your Boss Scene (Make sure it's in Build Settings!)
                SceneManager.LoadScene("BossScene"); 
            }
        }
    }
}