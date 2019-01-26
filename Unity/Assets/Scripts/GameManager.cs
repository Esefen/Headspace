using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AppState {Menu, Question, Answer, Transition};
public enum AnswerNumber {Two, Three, Four};

public struct Question
{
    public AnswerNumber possibleAnswers;
    public AudioClip intro;
    public AudioClip[] answers;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static AudioSource speaker;
    public static AppState gameState = AppState.Menu;
    public Image fade;
    public GameObject menuPanel, gamePanel;

    public Question test;

    bool fadeOut = false;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        Instance = this;
        speaker = GetComponent<AudioSource>();

        // Hardcoded questions
    }

    public void LaunchGame()
    {
        // Fade out
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 1, "onupdate", "FadeTransition", "easetype", "easeInCubic", "oncomplete", "StartGame"));
    }

    void FadeTransition(float newAlpha)
    {
        fade.color = new Color(1, 1, 1, newAlpha);
    }

    void StartGame()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        // Fade in
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 1, "onupdate", "FadeTransition", "easetype", "easeInCubic"));

        gameState = AppState.Question;
        // Pick a random question
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnswer(AudioClip answer)
    {
        if (!speaker.isPlaying)
        {
            speaker.clip = answer;
            speaker.Play();
        }
        else if (speaker.clip != answer)
        {
            if (speaker.volume != 0) iTween.AudioTo(gameObject, 0, 1, 0.15f);
            else
            {
                speaker.clip = answer;
                speaker.Play();
            }
        }
    }

    public void UpdateAnswerSound(float newVolume)
    {
        fadeOut = false;
        speaker.volume = newVolume;
    }

    public void FadeOutSpeaker()
    {
        if (!fadeOut)
        {
            fadeOut = true;
            iTween.AudioTo(gameObject, 0, 1, 0.7f);
            //iTween.AudioTo(gameObject, iTween.Hash(""));
        }
    }

}
