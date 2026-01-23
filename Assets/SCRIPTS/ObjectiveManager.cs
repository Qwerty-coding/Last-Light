using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI trackerText;
    public CanvasGroup trackerCanvasGroup;

    [Header("Settings")]
    public int woodRequired = 10;   // 🔥 CHANGED FROM 5 → 10
    public int zombiesRequired = 3;

    private int currentWood = 0;
    private int zombiesKilled = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (trackerText != null)
            trackerText.text = "";
    }

    // Needed by IntroSequence (DO NOT REMOVE)
    public void StartGameLoop()
    {
        UpdateObjective("Exit the house from the main gate");
    }

    // 🔥 FORCE correct wood UI (kills 0/5 & 0/10 bugs)
    public void ForceWoodObjectiveUI()
    {
        if (trackerText == null) return;

        trackerText.text = "- Gather Wood (0/" + woodRequired + ")";
    }

    // Called when Axe is picked up
    public void StartWoodObjective()
    {
        currentWood = 0;
        ForceWoodObjectiveUI();
    }

    // Called when a log is collected
    public void AddWood()
    {
        currentWood++;

        if (currentWood > woodRequired)
            currentWood = woodRequired;

        trackerText.text = "- Gather Wood (" + currentWood + "/" + woodRequired + ")";

        if (currentWood >= woodRequired)
        {
            UpdateObjective("Trade logs with the NPC for a gun");
        }
    }

    public void OnZombieKilled()
    {
        zombiesKilled++;

        trackerText.text = "- Kill Zombies (" + zombiesKilled + "/" + zombiesRequired + ")";

        if (zombiesKilled >= zombiesRequired)
        {
            SimpleInventory.Instance.AddItem("Key", 1);
            UpdateObjective("Key Found! Unlock the Terrace Door");
        }
    }

    public void UpdateObjective(string newText)
    {
        StopAllCoroutines();
        StartCoroutine(FadeObjective(newText));
    }

    IEnumerator FadeObjective(string text)
    {
        trackerCanvasGroup.alpha = 0;
        trackerText.text = "- " + text;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            trackerCanvasGroup.alpha = t;
            yield return null;
        }
    }
}
