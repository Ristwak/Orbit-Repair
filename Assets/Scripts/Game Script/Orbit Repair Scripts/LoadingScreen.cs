using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup canvasGroup;     // Fade this in/out
    public Slider progressBar;          // Optional
    public TMP_Text progressLabel;      // Optional "Loading 73%"

    [Header("Timings")]
    public float fadeInTime = 0.25f;
    public float fadeOutTime = 0.2f;
    public float minShowTime = 0.5f;    // avoid blink

    void Awake()
    {
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    public void BeginLoad(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        // Fade in
        if (canvasGroup)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            float t = 0f;
            while (t < fadeInTime)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, t / fadeInTime);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        float shown = 0f;

        // Async load
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f) // Unity reports up to 0.9 until ready
        {
            shown += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(op.progress / 0.9f);
            if (progressBar) progressBar.value = p;
            if (progressLabel) progressLabel.text = $"Loading {Mathf.RoundToInt(p * 100f)}%";
            yield return null;
        }

        // Reached 0.9, now finalize display
        shown += Time.unscaledDeltaTime;
        if (progressBar) progressBar.value = 1f;
        if (progressLabel) progressLabel.text = "Loading 100%";

        // Ensure screen is shown at least a brief moment
        while (shown < minShowTime) { shown += Time.unscaledDeltaTime; yield return null; }

        // Activate the scene
        op.allowSceneActivation = true;
    }
}
