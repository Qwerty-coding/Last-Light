using UnityEngine;

public class BossTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destinationPoint;
    public BossZombie bossScript;

    [Header("Atmosphere")]
    public bool turnOffFog = true;
    public Material skyboxChange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Teleport Player
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            
            other.transform.position = destinationPoint.position;
            other.transform.rotation = destinationPoint.rotation;
            
            if (cc) cc.enabled = true;

            // 2. Turn off fog
            if (turnOffFog)
            {
                RenderSettings.fog = false;
            }

            if (skyboxChange != null)
            {
                RenderSettings.skybox = skyboxChange;
            }

            // 3. STOP ZOMBIE SPAWNING
            ZombieSpawner spawner = FindObjectOfType<ZombieSpawner>();
            if (spawner != null)
            {
                spawner.StopSpawning();
            }

            // 4. Wake up boss
            if (bossScript != null && !bossScript.battleStarted)
            {
                bossScript.StartBossFight();
            }
        }
    }
}