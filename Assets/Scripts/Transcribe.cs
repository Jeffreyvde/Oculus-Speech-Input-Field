using System;
using UnityEngine;
using Oculus.Voice;

/// <summary>
/// Transcribe tool to extract longer texts from the Voice SDK
/// </summary>
[RequireComponent(typeof(AppVoiceExperience))]
public class Transcribe : MonoBehaviour
{
    private AppVoiceExperience voice;

    private string fullTranscribe;
    private string partialTranscribe;

    /// <summary>
    /// Is the transcribe currently running.
    /// </summary>
    public bool Active { get; private set; } = false;

    /// <summary>
    /// When the transcribe updates this event is called with the current estimated transcribe
    /// </summary>
    public event Action<string> OnTranscribeUpdate;

    /// <summary>
    /// When the full transcribe is finished this event with the final transcribe
    /// </summary>
    public event Action<string> OnTranscribeFinished;


    /// <summary>
    /// Get the voice app experience
    /// </summary>
    private void Start()
    {
        if (voice == null)
        {
            voice = GetComponent<AppVoiceExperience>();
        }
    }


    /// <summary>
    /// Start a longer transcribe session
    /// </summary>
    public void Activate()
    {
        if (Active)
        {
            return;
        }

        fullTranscribe = string.Empty;
        partialTranscribe = string.Empty;

        Active = true;
        voice.VoiceEvents.OnFullTranscription.AddListener(OnFullTranscript);
        voice.VoiceEvents.OnError.AddListener(((arg0, s) =>
        {
            Debug.LogWarning(arg0 + s);
            Deactivate();
            OnFullTranscript(partialTranscribe);
        }));
        voice.VoiceEvents.OnPartialTranscription.AddListener(OnPartialTranscript);

        voice.Activate();
    }

    /// <summary>
    /// End the transcribe session
    /// </summary>
    public void Deactivate()
    {
        if (!Active)
        {
            return;
        }

        Active = false;
        voice.Deactivate();
    }

    /// <summary>
    /// When we have received a short transcript we want to add it to the existing text.
    /// We then update the current text.
    /// </summary>
    /// <param name="fullTranscript">The short transcript we received</param>
    private void OnFullTranscript(string fullTranscript)
    {
        if (Active)
        {
            voice.ActivateImmediately();
        }
        

        // Fix basic punctuation issues
        if (!string.IsNullOrWhiteSpace(fullTranscribe) && !string.IsNullOrWhiteSpace(fullTranscript))
        {
            if (EndsWithPunctuation(fullTranscribe))  
            {
                fullTranscribe += " ";
            }
            else
            {
                fullTranscribe += ". ";
            }
        }

        fullTranscribe += fullTranscript;
        partialTranscribe = string.Empty;
        UpdateText();

        if (Active) 
            return;

        voice.VoiceEvents.OnFullTranscription.RemoveAllListeners();
        voice.VoiceEvents.OnError.RemoveAllListeners();
        voice.VoiceEvents.OnPartialTranscription.RemoveAllListeners();
        OnTranscribeFinished?.Invoke(fullTranscribe);
    }

    /// <summary>
    /// When a small sample has been processed we get an estimate of the sentence.
    /// This is used to show the user what has so far been said
    /// </summary>
    /// <param name="partialTranscript">The text of the partial transcribe</param>
    private void OnPartialTranscript(string partialTranscript)
    {
        partialTranscribe = partialTranscript;
        UpdateText();
    }

    /// <summary>
    /// Update the text using the OnTextUpdate event
    /// </summary>
    private void UpdateText()
    {
        string currentTranscribe = fullTranscribe + partialTranscribe;
        OnTranscribeUpdate?.Invoke(currentTranscribe);
    }


    /// <summary>
    /// When the app is not focused The mic is  released so when somebody exists the app we stop transcribing.
    /// </summary>
    /// <param name="focus">True if the app is focused and false if not focused anymore</param>
    private void OnApplicationFocus(bool focus)
    {
        if (Active && !focus)
        {
            Deactivate();
            OnFullTranscript(partialTranscribe);
        }
    }

    /// <summary>
    /// Check whether or not the string ends with punctuation
    /// </summary>
    /// <param name="message">The message you want to check</param>
    /// <returns>True if the last character is punctuation</returns>
    private static bool EndsWithPunctuation(string message)
    {
        char value = message[message.Length - 1];
        return value == '?' || value == '!' || value == '.';
    }
}
