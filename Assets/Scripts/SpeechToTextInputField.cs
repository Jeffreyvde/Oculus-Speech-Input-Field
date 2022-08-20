using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Ui element to get the Speech To Text input field
/// </summary>
public class SpeechToTextInputField : MonoBehaviour
{
    [SerializeField] private Transcribe transcribe;
    [SerializeField] private Image micIndicator;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Button button;

    private bool active;

    /// <summary>
    /// Set up the UI
    /// </summary>
    private void OnEnable()
    {
        SetButtonMethod(Activate);
        micIndicator.color = Color.white;
    }

    /// <summary>
    /// Unsubscribe to transcribe events
    /// </summary>
    private void OnDisable()
    {
        if(!active)
            return;

        DeActivate();
    }

    /// <summary>
    /// Activate Speech to Text
    /// </summary>
    public void Activate()
    {
        if (transcribe.Active)
            return;

        SetButtonMethod(DeActivate); 
        micIndicator.color = Color.red;

        transcribe.OnTranscribeFinished += OnFullTranscribe;
        transcribe.OnTranscribeUpdate += OnPartialTranscribe;

        transcribe.Activate();
        active = true;
    }

    /// <summary>
    /// Deactivate Speech to Text
    /// </summary>
    public void DeActivate()
    {
        if (!active)
            return;

        SetButtonMethod(Activate);
        micIndicator.color = Color.white;
        transcribe.Deactivate();
        active = false;

        transcribe.OnTranscribeFinished -= OnFullTranscribe;
        transcribe.OnTranscribeUpdate -= OnPartialTranscribe;
    }

    /// <summary>
    /// Remove all code added listeners and only add the new action
    /// </summary>
    /// <param name="action">The action you want to replace with the button listener</param>
    private void SetButtonMethod(UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    /// <summary>
    /// Update the text when we have the partial transcribe
    /// </summary>
    /// <param name="text">The latest transcribed text</param>
    private void OnPartialTranscribe(string text)
    {
        textMesh.text = text;
    }

    /// <summary>
    /// Final update when the text has been fully transcribed
    /// </summary>
    /// <param name="text">The transcribe text</param>
    private void OnFullTranscribe(string text)
    {
        textMesh.text = text;
        DeActivate();
    }
}
