using System.Collections;
using UnityEngine;
using TMPro;

public class EventVisualizer : MonoBehaviour
{
    [Header("Feedback References")]
    public Renderer planeRenderer;
    public TextMeshProUGUI feedbackText;

    [Header("Color Settings")]
    public Color defaultColor = Color.white;

    void Start()
    {
        if (planeRenderer != null)
            defaultColor = planeRenderer.material.color;

        HideFeedback();
    }

    public void FlashColor(Color flashColor, float duration = 0.5f)
    {
        StartCoroutine(FlashRoutine(flashColor, duration));
    }

    private IEnumerator FlashRoutine(Color color, float duration)
    {
        if (planeRenderer == null) yield break;

        planeRenderer.material.color = color;
        yield return new WaitForSeconds(duration);
        planeRenderer.material.color = defaultColor;
    }

    public void ShowMissionComplete()
    {
        if (feedbackText == null) return;

        feedbackText.text = "Mission Completed!";
        feedbackText.color = Color.green;
        feedbackText.gameObject.SetActive(true);

        StartCoroutine(HideFeedbackAfterSeconds(2f));
    }

    public void HideFeedback()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    private IEnumerator HideFeedbackAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideFeedback();
    }
}
