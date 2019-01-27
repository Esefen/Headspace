using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AppState {Menu, Question, Answer, Transition, Credits};

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static AudioSource speaker;
    public static AppState gameState = AppState.Menu;
    public Image fade;
    public Button skipButton, repeatButton;
    public GameObject menuPanel, gamePanel, creditsPanel;

    public List<Question> questionLibrary = new List<Question>();
    List<Question> questionPool = new List<Question>();
    List<AudioClip> mix = new List<AudioClip>();
    public static Question currentQuestion;
    public int maxQuestions = 5;
    int questionsAnswered = 0;

    bool fadeOut = false;
    public float transitionFadeOut = 3f;
    public float transitionFadeIn = 1.5f;
    int chosenAnswerIndex;

    GameObject mixer;

    void Awake()
    {
        Instance = this;
        speaker = GetComponent<AudioSource>();
    }

    public void LaunchGame()
    {
        mix.Clear();
        questionsAnswered = 0;
        questionPool.AddRange(questionLibrary);
        // Fade out
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", transitionFadeOut, "onupdate", "FadeTransition", "easetype", "easeInCubic", "oncomplete", "StartGame"));
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
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", transitionFadeIn, "onupdate", "FadeTransition", "easetype", "easeInCubic"));

        // Pick a random question
        currentQuestion = questionPool[0];
        questionPool.RemoveAt(0);
        gameState = AppState.Question;
    }

    void SetNewQuestion()
    {
        int index = GetRandomQuestionIndex();
        currentQuestion = questionPool[index];
        questionPool.RemoveAt(index);
    }

    int GetRandomQuestionIndex()
    {
        return Mathf.FloorToInt(Random.value * questionPool.Count);
    }

    public void PreviewAnswer(int index)
    {
        chosenAnswerIndex = index;
        PreviewAnswer(currentQuestion.previewAnswers[index]);
    }

    void PreviewAnswer(AudioClip answer)
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

    void Update()
    {
        if (gameState == AppState.Question)
        {
            if (!speaker.isPlaying)
            {
                //Debug.Log("Play question !");
                speaker.volume = 1;
                speaker.PlayOneShot(currentQuestion.intro);
                Invoke("BeginExploration", currentQuestion.intro.length);
            }
        }
    }

    void BeginExploration()
    {
        gameState = AppState.Answer;
        repeatButton.interactable = true;
    }

    public void ChooseAnswer()
    {
        //Debug.Log("ChooseAnswer");
        mix.Add(currentQuestion.answers[chosenAnswerIndex]);
        gameState = AppState.Transition;
        speaker.volume = 1;
        speaker.clip = currentQuestion.answers[chosenAnswerIndex];
        speaker.Play();
        Invoke("DisplaySkipIcon", Mathf.Min (5, speaker.clip.length));
        StartCoroutine(SampleHasEnded());
        //Invoke("SampleHasEnded", speaker.clip.length);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", transitionFadeOut, "onupdate", "FadeTransition", "easetype", "easeInCubic"));
        questionsAnswered++;
    }

    void DisplaySkipIcon()
    {
        skipButton.interactable = true;
        skipButton.GetComponent<Animator>().SetBool("OnOff", true);
    }

    IEnumerator SampleHasEnded()
    {
        yield return new WaitForSeconds(speaker.clip.length);
        if (skipButton.interactable) SkipAnswer();
    }

    public void SkipAnswer()
    {
        skipButton.GetComponent<Animator>().SetBool("OnOff", false);
        EndTransition();
    }

    void EndTransition()
    {
        StopAllCoroutines();
        //Debug.Log("EndTransition");
        iTween.AudioTo(gameObject, 0, 1, transitionFadeIn);
        Invoke("StopMutedClip", transitionFadeIn);

        if (questionsAnswered < maxQuestions) // next question
        {
            Invoke("GoToNextQuestion", transitionFadeIn + 1);
        }
        else // display credits
        {
            gamePanel.SetActive(false);
            creditsPanel.SetActive(true);
            iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", transitionFadeIn, "onupdate", "FadeTransition", "easetype", "easeInCubic", "oncomplete", "PlayCumulatedMix"));
            gameState = AppState.Credits;
        }
    }

    void GoToNextQuestion()
    {
        //Debug.Log("GoToNextQuestion");
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", transitionFadeIn, "onupdate", "FadeTransition", "easetype", "easeInCubic"));
        SetNewQuestion();
        gameState = AppState.Question;
    }

    void StopMutedClip()
    {
        speaker.Stop();
    }

    void PlayCumulatedMix()
    {
        speaker.volume = 1;
        mixer = new GameObject("Mixer");
        foreach (AudioClip c in mix)
        {
            GameObject tmp = new GameObject("Mix");
            tmp.transform.SetParent(mixer.transform);
            AudioSource a = tmp.AddComponent<AudioSource>();
            a.clip = c;
            a.loop = true;
            a.Play();
        }
    }

    public void ReturnToMainMenu()
    {
        Destroy(mixer);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", transitionFadeOut, "onupdate", "FadeTransition", "easetype", "easeInCubic", "oncomplete", "ReactivateMainMenu"));
    }

    void ReactivateMainMenu()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
        gameState = AppState.Menu;
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", transitionFadeIn, "onupdate", "FadeTransition", "easetype", "easeInCubic"));
    }

    public void RepeatIntro()
    {
        gameState = AppState.Question;
    }
}
