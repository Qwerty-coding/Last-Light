using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    // Make sure your Player object has the tag "Player"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // This triggers the specific text you asked for
            ObjectiveManager.Instance.StartBossObjective();
            
            // Optional: Disable this trigger so it doesn't fire twice
            gameObject.SetActive(false);
        }
    }
}