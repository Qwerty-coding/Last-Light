using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("Tracker UI References")]
    public TextMeshProUGUI trackerText;
    public CanvasGroup trackerCanvasGroup;

    [Header("Popup UI References")]
    public CanvasGroup popupCanvasGroup;
    public float popupDuration = 2f;

    [Header("Settings")]
    public int woodRequired = 10;
    public int zombiesRequired = 10; // 1. Zombie count is 10

    private int currentWood = 0;
    private int zombiesKilled = 0;
    private bool keyGiven = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (trackerText != null) trackerText.text = "";
        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 0;
    }

    public void StartGameLoop()
    {
        UpdateObjective("Exit the house from the main gate");
    }

    // --- WOOD LOGIC ---
    public void StartWoodObjective()
    {
        currentWood = 0;
        ForceWoodObjectiveUI();
    }

    public void ForceWoodObjectiveUI()
    {
        if (trackerText != null)
            trackerText.text = "- Gather Wood (0/" + woodRequired + ")";
    }

    public void AddWood()
    {
        currentWood++;
        if (currentWood > woodRequired) currentWood = woodRequired;

        trackerText.text = "- Gather Wood (" + currentWood + "/" + woodRequired + ")";

        if (currentWood >= woodRequired)
        {
            UpdateObjective("Trade logs with the NPC for a gun");
        }
    }

    // --- ZOMBIE LOGIC ---
    public void StartZombieObjective()
    {
        zombiesKilled = 0;
        keyGiven = false;
        
        // 2. Immediate UI update for 10 zombies
        trackerText.text = "- Kill Zombies (0/" + zombiesRequired + ")";
        UpdateObjective("Kill " + zombiesRequired + " Zombies to find the Key");
    }

    public void OnZombieKilled()
    {
        if (keyGiven) return;

        zombiesKilled++;
        trackerText.text = "- Kill Zombies (" + zombiesKilled + "/" + zombiesRequired + ")";

        if (zombiesKilled >= zombiesRequired)
        {
            keyGiven = true;
            if (SimpleInventory.Instance != null) SimpleInventory.Instance.AddItem("Key", 1);
            
            // This leads the player to the Fire Tower
            UpdateObjective("Key Found! Find the Fire Tower and reach the top");
        }
    }

    // --- PORTAL & BOSS LOGIC ---

    // Call this when player reaches the top of the Fire Tower (Trigger)
    public void StartPortalObjective()
    {
        trackerText.text = "- Enter the Portal";
        UpdateObjective("Enter the Portal");
    }

    // Call this when player steps on the 'Sender' object
    public void StartBossObjective()
    {
        trackerText.text = "- Kill the Boss";
        UpdateObjective("Fight and kill The Boss Zombie");
    }

    // --- UI HELPERS ---
    public void UpdateObjective(string newText)
    {
        StopAllCoroutines();
        StartCoroutine(FadeObjective(newText));
        if (popupCanvasGroup != null) StartCoroutine(ShowPopup());
    }

    IEnumerator FadeObjective(string text)
    {
        trackerCanvasGroup.alpha = 0;
        trackerText.text = "- " + text;
        float t = 0f;
        while (t < 1f) { t += Time.deltaTime; trackerCanvasGroup.alpha = t; yield return null; }
        trackerCanvasGroup.alpha = 1f;
    }

    IEnumerator ShowPopup()
    {
        float t = 0f;
        while (t < 1f) { t += Time.deltaTime * 3f; popupCanvasGroup.alpha = t; yield return null; }
        popupCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(popupDuration);
        t = 1f;
        while (t > 0f) { t -= Time.deltaTime; popupCanvasGroup.alpha = t; yield return null; }
        popupCanvasGroup.alpha = 0f;
    }
}