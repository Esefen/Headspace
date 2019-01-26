using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundscape : MonoBehaviour
{
    const float threshold = 0.65f;
    float xMin, yMin;
    Vector2 center, topLeft, topCenter, topRight, bottomLeft, bottomRight;
    bool connectedToCorner = false;

    Question currentQuestion;
    public AudioClip testHautGauche;
    public AudioClip testHautDroite;
    public AudioClip testBasGauche;
    public AudioClip testBasDroite;

    // Start is called before the first frame update
    void Start()
    {
        xMin = 1 - threshold;
        yMin = 1 - threshold;

        center = new Vector2(Screen.width / 2, Screen.height / 2);
        topLeft = new Vector2(0, Screen.height);
        topCenter = new Vector2(Screen.width / 2, Screen.height);
        topRight = new Vector2(Screen.width, Screen.height);
        bottomLeft = new Vector2(0, 0);
        bottomRight = new Vector2(Screen.width, 0);

        // temp
        GameManager.gameState = AppState.Answer;

        currentQuestion = new Question();
        currentQuestion.possibleAnswers = AnswerNumber.Three;
        currentQuestion.answers = new AudioClip[4];
        currentQuestion.answers[0] = testHautGauche;
        currentQuestion.answers[1] = testHautDroite;
        currentQuestion.answers[2] = testBasGauche;
        currentQuestion.answers[3] = testBasDroite;
    }

    // Update is called once per frame
    void Update()
    {
        // Only active in answer mode
        if (GameManager.gameState == AppState.Answer)
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
                UpdateInteractiveSoundscape(pos);
                if (connectedToCorner)
                    UpdateSoundVolume(pos);
            }
            // Return soundscape to default
            else
            {
                connectedToCorner = false;
                GameManager.Instance.FadeOutSpeaker();
                //StartCoroutine(GameManager.Instance.FadeOut());
                //GameManager.speaker.Stop();
            }
        }
    }

    // Modify soundscape
    void UpdateInteractiveSoundscape(Vector2 pos)
    {
        switch(currentQuestion.possibleAnswers)
        {
            case AnswerNumber.Two: QuestionTwoAnswers(pos); break;
            case AnswerNumber.Three: QuestionThreeAnswers(pos); break;
            case AnswerNumber.Four: QuestionFourAnswers(pos); break;
        }
    }

    void QuestionTwoAnswers(Vector2 pos)
    {
        if (pos.x < xMin)
        {
            ConnectAnswer(currentQuestion.answers[0]);
        }
        // right
        else if (pos.x > threshold)
        {
            ConnectAnswer(currentQuestion.answers[1]);
        }
        else
        {
            connectedToCorner = false;
            GameManager.Instance.FadeOutSpeaker();
        }
    }

    void QuestionThreeAnswers(Vector2 pos)
    {
        if (CheckDistance(topCenter)) ConnectAnswer(currentQuestion.answers[0]);
        // left
        else if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomLeft)) ConnectAnswer(currentQuestion.answers[1]);
            }
            else connectedToCorner = false;
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(currentQuestion.answers[2]);
            }
            else connectedToCorner = false;
        }
        else
        {
            connectedToCorner = false;
            GameManager.Instance.FadeOutSpeaker();
        }
    }

    void QuestionFourAnswers(Vector2 pos)
    {
        //Debug.Log("Mouse Position: " + Input.mousePosition.x + ";" + Input.mousePosition.y);
        //Debug.Log("Mouse Position: " + pos.x + ";" + pos.y);
        // left
        if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomLeft)) ConnectAnswer(currentQuestion.answers[2]);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topLeft)) ConnectAnswer(currentQuestion.answers[0]);
            }
            else connectedToCorner = false;
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(currentQuestion.answers[3]);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topRight)) ConnectAnswer(currentQuestion.answers[1]);
            }
            else connectedToCorner = false;
        }
        else
        {
            connectedToCorner = false;
            GameManager.Instance.FadeOutSpeaker();
        }
    }

    bool CheckDistance(Vector2 corner)
    {
        float d = Vector3.Distance(corner, Input.mousePosition) / Screen.width;
        //Debug.Log("Mouse distance from corner: " + d);
        return (d < 0.3f);
    }

    void UpdateSoundVolume(Vector2 pos)
    {
        float d = Vector3.Distance(center, Input.mousePosition) / ((float)Screen.width /2f);
        //Debug.Log("Mouse distance from center: " + d);
        GameManager.Instance.UpdateAnswerSound(Mathf.Min(d, 1));
    }

    void ConnectAnswer(AudioClip clip)
    {
        connectedToCorner = true;
        GameManager.Instance.PlayAnswer(clip);
    }

}
