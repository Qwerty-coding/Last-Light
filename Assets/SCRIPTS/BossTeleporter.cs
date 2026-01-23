using UnityEngine;

public class BossTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destinationPoint; // The empty object in the arena
    public BossZombie bossScript;      // The Boss GameObject

    [Header("Atmosphere")]
    public bool turnOffFog = true;     // Check this box to clear the air!
    public Material skyboxChange;      // (Optional) Drag a different skybox here if you want

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Teleport Player
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc) cc.enabled = false; // Disable physics briefly
            
            other.transform.position = destinationPoint.position;
            other.transform.rotation = destinationPoint.rotation;
            
            if (cc) cc.enabled = true; // Re-enable physics

            // 2. TURN OFF FOG
            if (turnOffFog)
            {
                RenderSettings.fog = false;
            }

            // (Optional) Change Skybox for epic boss fight feel
            if (skyboxChange != null)
            {
                RenderSettings.skybox = skyboxChange;
            }

            // 3. Wake Up Boss
            if (bossScript != null && !bossScript.battleStarted)
            {
                bossScript.StartBossFight();
            }
        }
    }
}