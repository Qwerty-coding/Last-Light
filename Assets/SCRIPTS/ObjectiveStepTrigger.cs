using UnityEngine;

public class ObjectiveStepTrigger : MonoBehaviour
{
    [Header("Objective Text")]
    [TextArea]
    public string objectiveText;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.UpdateObjective(objectiveText);
        }

        triggered = true;
        Destroy(gameObject); // fire once only
    }
}
