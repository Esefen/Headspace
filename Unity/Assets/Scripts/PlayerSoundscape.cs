using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundscape : MonoBehaviour
{
    const float threshold = 0.65f;
    float xMin, yMin;
    Vector2 left, right, center, topLeft, topCenter, topRight, bottomLeft, bottomRight;
    Vector2 cornerConnected = Vector2.negativeInfinity;

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
    }

    // Update is called once per frame
    void Update()
    {
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
                if (cornerConnected != Vector2.negativeInfinity) UpdateSoundVolume(pos);
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
        }
    }

    void QuestionTwoAnswers(Vector2 pos)
    {
        // left
        if (pos.x < xMin)
        {
            ConnectAnswer(left, 0);
        }
        // right
        else if (pos.x > threshold)
        {
            ConnectAnswer(right, 1);
        }
        else DisconnectAnswer();
    }

    void QuestionThreeAnswers(Vector2 pos)
    {
        // center top
        if (CheckDistance(topCenter)) ConnectAnswer(topCenter, 0);
        // left
        else if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomLeft)) ConnectAnswer(bottomLeft, 1);
            }
            else ResetCorner();
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(bottomRight, 2);
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
                if (CheckDistance(bottomLeft)) ConnectAnswer(bottomLeft, 2);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topLeft)) ConnectAnswer(topLeft, 0);
            }
            else ResetCorner();
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(bottomRight, 3);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topRight)) ConnectAnswer(topRight, 1);
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

    void ConnectAnswer(Vector2 corner, int index)
    {
        cornerConnected = corner;
        GameManager.Instance.PreviewAnswer(index);
    }

    void ResetCorner()
    {
        //Debug.Log("Corner reset");
        cornerConnected = Vector2.negativeInfinity;
    }

    void DisconnectAnswer()
    {
        ResetCorner();
        GameManager.Instance.FadeOutSpeaker();
    }

    bool AnswerSelected()
    {
        return Vector2.Distance(cornerConnected, Vector2.negativeInfinity) > 10;
    }

    void ChooseAnswer()
    {
        GameManager.Instance.ChooseAnswer();
    }

}
