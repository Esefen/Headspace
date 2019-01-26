using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public static AppState gameState = AppState.Question;
    public static AudioSource speaker;
    public Question test;

    bool fadeOut = false;

    void Awake()
    {
        Instance = this;
        speaker = GetComponent<AudioSource>();
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
        }
        //iTween.AudioTo(gameObject, iTween.Hash(""));
    }
    /*
    public IEnumerator FadeOut()
    {
        while (speaker.volume >= 0)
        {
            yield return new WaitForFixedUpdate();
            speaker.volume -= 0.1f * Time.deltaTime;
        }
    }

    public void DiminishSound()
    {
        speaker.volume -= 0.5f * Time.deltaTime;
    }
    */
}
