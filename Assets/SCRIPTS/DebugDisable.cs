using UnityEngine;

public class DebugDisable : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TempAxe started ACTIVE");
    }

    void OnDisable()
    {
        Debug.LogError("TempAxe was DISABLED! Check stack trace below:");
        Debug.LogError(System.Environment.StackTrace);
    }
}