using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSoundscape : MonoBehaviour
{
    const float threshold = 0.65f;
    float xMin, yMin;
    Vector2 left, right, center, topLeft, topCenter, topRight, bottomLeft, bottomRight;
    public Animator leftIcon, rightIcon, topLeftIcon, topCenterIcon, topRightIcon, bottomLeftIcon, bottomRightIcon;
    Animator cornerConnected = null;
    public Image nopeIcon;

    // Start is called before the first frame update
    void Start()
    {
        xMin = 1 - threshold;
        yMin = 1 - threshold;

        left = new Vector2(0, Screen.height / 2);
        right = new Vector2(Screen.width, Screen.height / 2);
        center = new Vector2(Screen.width / 2, Screen.height / 2);
        topLeft = new Vector2(0, Screen.height);
        topCenter = new Vector2(Screen.width / 2, Screen.height);
        topRight = new Vector2(Screen.width, Screen.height);
        bottomLeft = new Vector2(0, 0);
        bottomRight = new Vector2(Screen.width, 0);

        nopeIcon.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Set question-relevant parameters
        if (GameManager.gameState == AppState.Question)
        {
            SetCurrentQuestionIcons(true);
        }
        // Only active in answer mode
        if (GameManager.gameState == AppState.Answer)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (AnswerSelected()) ChooseAnswer();
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
                //Debug.Log("Mouse Position: " + Input.mousePosition.x + ";" + Input.mousePosition.y);
                //Debug.Log("Mouse Position: " + pos.x + ";" + pos.y);
                UpdateInteractiveSoundscape(pos);
                if (AnswerSelected()) UpdateSoundVolume(pos);
            }
            // Return soundscape to default
            else DisconnectAnswer();
        }
    }

    // Modify soundscape
    void UpdateInteractiveSoundscape(Vector2 pos)
    {
        switch(GameManager.currentQuestion.possibleAnswers)
        {
            case AnswerNumber.Two: QuestionTwoAnswers(pos); break;
            case AnswerNumber.Three: QuestionThreeAnswers(pos); break;
            case AnswerNumber.Four: QuestionFourAnswers(pos); break;
            default: throw new UnityException();
        }
    }

    void QuestionTwoAnswers(Vector2 pos)
    {
        // left
        if (pos.x < xMin)
        {
            ConnectAnswer(leftIcon, 0);
        }
        // right
        else if (pos.x > threshold)
        {
            ConnectAnswer(rightIcon, 1);
        }
        else DisconnectAnswer();
    }

    void QuestionThreeAnswers(Vector2 pos)
    {
        // center top
        if (CheckDistance(topCenter)) ConnectAnswer(topCenterIcon, 0);
        // left
        else if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomLeft)) ConnectAnswer(bottomLeftIcon, 1);
            }
            else ResetCorner();
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(bottomRightIcon, 2);
            }
            else ResetCorner();
        }
        else DisconnectAnswer();
    }

    void QuestionFourAnswers(Vector2 pos)
    {
        // left
        if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomLeft)) ConnectAnswer(bottomLeftIcon, 2);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topLeft)) ConnectAnswer(topLeftIcon, 0);
            }
            else ResetCorner();
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(bottomRightIcon, 3);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topRight)) ConnectAnswer(topRightIcon, 1);
            }
            else ResetCorner();
        }
        else DisconnectAnswer();
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

    void ConnectAnswer(Animator corner, int index)
    {
        IconShiftFade(0.02f);
        cornerConnected = corner;
        cornerConnected.SetBool("SoundIsTested", true);
        GameManager.Instance.PreviewAnswer(index);
    }

    void ResetCorner()
    {
        IconShiftFade(-0.03f);
        //Debug.Log("Corner reset");
        if (cornerConnected != null) cornerConnected.SetBool("SoundIsTested", false);
        cornerConnected = null;
    }

    void DisconnectAnswer()
    {
        ResetCorner();
        GameManager.Instance.FadeOutSpeaker();
    }

    bool AnswerSelected()
    {
        return cornerConnected != null;
    }

    void ChooseAnswer()
    {
        IconShiftFade(-0.09f);
        cornerConnected.SetBool("SoundIsChosen", true);
        GameManager.Instance.ChooseAnswer();
        Invoke("ResetSpriteAnimations", 2.5f);
    }

    void IconShiftFade(float alphaMod)
    {
        nopeIcon.color = new Color(1, 1, 1, Mathf.Clamp(nopeIcon.color.a + alphaMod, 0, 1));
    }

    void ResetSpriteAnimations()
    {
        cornerConnected.SetBool("SoundIsTested", false);
        cornerConnected.SetBool("SoundIsChosen", false);
        cornerConnected = null;
        IconShiftFade(-1);
        SetCurrentQuestionIcons(false);
    }

    void SetCurrentQuestionIcons(bool enable)
    {
        switch (GameManager.currentQuestion.possibleAnswers)
        {
            case AnswerNumber.Two:
                leftIcon.SetBool("QuestionAsked", enable);
                rightIcon.SetBool("QuestionAsked", enable);
                break;
            case AnswerNumber.Three:
                topCenterIcon.SetBool("QuestionAsked", enable);
                bottomLeftIcon.SetBool("QuestionAsked", enable);
                bottomRightIcon.SetBool("QuestionAsked", enable);
                break;
            case AnswerNumber.Four:
                topLeftIcon.SetBool("QuestionAsked", enable);
                topRightIcon.SetBool("QuestionAsked", enable);
                bottomLeftIcon.SetBool("QuestionAsked", enable);
                bottomRightIcon.SetBool("QuestionAsked", enable);
                break;
        }
    }
}
