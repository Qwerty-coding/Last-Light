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
    public CanvasGroup popupCanvasGroup; // Drag your 'ObjectivePopupText' object here
    public float popupDuration = 2f;     // How long the popup stays visible

    [Header("Settings")]
    public int woodRequired = 10;
    public int zombiesRequired = 3;

    private int currentWood = 0;
    private int zombiesKilled = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (trackerText != null)
            trackerText.text = "";
            
        // Ensure popup is invisible at start
        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 0;
    }

    // Needed by IntroSequence
    public void StartGameLoop()
    {
        UpdateObjective("Exit the house from the main gate");
    }

    public void ForceWoodObjectiveUI()
    {
        if (trackerText == null) return;
        trackerText.text = "- Gather Wood (0/" + woodRequired + ")";
    }

    public void StartWoodObjective()
    {
        currentWood = 0;
        ForceWoodObjectiveUI();
    }

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
            if (SimpleInventory.Instance != null)
                SimpleInventory.Instance.AddItem("Key", 1);
            
            UpdateObjective("Key Found! Find the Fire Tower and reach the top");
        }
    }

    public void UpdateObjective(string newText)
    {
        // Stop any currently running fades so they don't clash
        StopAllCoroutines();
        
        // 1. Update the side tracker
        StartCoroutine(FadeObjective(newText));

        // 2. Show the "Objective Updated" Popup
        if (popupCanvasGroup != null)
        {
            StartCoroutine(ShowPopup());
        }
    }

    // --- COROUTINES ---

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
        trackerCanvasGroup.alpha = 1f;
    }

    IEnumerator ShowPopup()
    {
        // 1. Fade In
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f; // Multiply speed to fade in fast
            popupCanvasGroup.alpha = t;
            yield return null;
        }
        popupCanvasGroup.alpha = 1f;

        // 2. Wait
        yield return new WaitForSeconds(popupDuration);

        // 3. Fade Out
        t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime; // Fade out slower
            popupCanvasGroup.alpha = t;
            yield return null;
        }
        popupCanvasGroup.alpha = 0f;
    }
}