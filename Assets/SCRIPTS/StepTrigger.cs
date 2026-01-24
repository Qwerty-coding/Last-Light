using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move to the next objective
            ObjectiveManager.Instance.UpdateObjective("Find the Axe in the outside storeroom");
            Destroy(gameObject); // Remove the trigger so it only happens once
        }
    }
} 