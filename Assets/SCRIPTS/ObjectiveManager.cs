using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI popupText;       // Drag "Msg_Popup" here
    public CanvasGroup popupCanvasGroup;    // Drag "Msg_Popup" here
    public TextMeshProUGUI trackerText;     // Drag "Msg_Tracker" here
    public CanvasGroup trackerCanvasGroup;  // Drag "Msg_Tracker" here

    [Header("Settings")]
    public float popupDuration = 2.0f;
    public float fadeSpeed = 2.0f;

    [Header("Game State")]
    public int woodCollected = 0;
    public int woodRequired = 10;

    private bool isGameStarted = false; // Prevents objectives during wakeup

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Ensure Popup is invisible at start
        if(popupCanvasGroup != null) popupCanvasGroup.alpha = 0;
        
        // Ensure Tracker is empty at start
        if(trackerText != null) trackerText.text = "";
    }

    // --- STEP 1: Call this when Wakeup Animation Finishes ---
    public void StartGameLoop()
    {
        isGameStarted = true;
        // Phase 1, Step 4: Look around for weapon
        UpdateObjective("Look around for a weapon");
    }

    // --- Main Function to Change Objectives ---
    public void UpdateObjective(string newObjectiveDescription)
    {
        if (!isGameStarted) return;

        StartCoroutine(ShowObjectiveSequence(newObjectiveDescription));
    }

    // Logic for Wood Gathering
    public void AddWood()
    {
        if (!isGameStarted) return;

        woodCollected++;
        
        // Update ONLY the tracker text (no popup animation for counting)
        if (woodCollected < woodRequired)
        {
            trackerText.text = $"- Gather wood outside ({woodCollected}/{woodRequired})";
        }
        else if (woodCollected == woodRequired)
        {
            // Wood done, trigger major update to find Gun
            UpdateObjective("Find the Gun at the Crash Site");
        }
    }

    // --- Animation Logic ---
    IEnumerator ShowObjectiveSequence(string newText)
    {
        // 1. Show "OBJECTIVE UPDATED" Popup
        yield return StartCoroutine(FadeCanvasGroup(popupCanvasGroup, 0, 1)); // Fade In
        
        // 2. Fade OUT old persistent text
        yield return StartCoroutine(FadeCanvasGroup(trackerCanvasGroup, 1, 0));

        // 3. Change the persistent text while it's invisible
        trackerText.text = "- " + newText;

        // 4. Wait a moment for player to read popup
        yield return new WaitForSeconds(popupDuration);

        // 5. Fade OUT Popup & Fade IN new Persistent text
        StartCoroutine(FadeCanvasGroup(popupCanvasGroup, 1, 0));
        StartCoroutine(FadeCanvasGroup(trackerCanvasGroup, 0, 1));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        float counter = 0f;
        while (counter < 1f)
        {
            counter += Time.deltaTime * fadeSpeed;
            cg.alpha = Mathf.Lerp(start, end, counter);
            yield return null;
        }
        cg.alpha = end;
    }
}