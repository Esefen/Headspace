using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundscape : MonoBehaviour
{
    const float threshold = 0.65f;
    float xMin, yMin;
    Vector2 center, topLeft, topCenter, topRight, bottomLeft, bottomRight;
    bool connectedToCorner = false;

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
                //Debug.Log("Mouse Position: " + Input.mousePosition.x + ";" + Input.mousePosition.y);
                //Debug.Log("Mouse Position: " + pos.x + ";" + pos.y);
                UpdateInteractiveSoundscape(pos);
                if (connectedToCorner) UpdateSoundVolume(pos);
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
            ConnectAnswer(0);
        }
        // right
        else if (pos.x > threshold)
        {
            ConnectAnswer(1);
        }
        else DisconnectAnswer();
    }

    void QuestionThreeAnswers(Vector2 pos)
    {
        // center top
        if (CheckDistance(topCenter)) ConnectAnswer(0);
        // left
        else if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomLeft)) ConnectAnswer(1);
            }
            else connectedToCorner = false;
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(2);
            }
            else connectedToCorner = false;
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
                if (CheckDistance(bottomLeft)) ConnectAnswer(2);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topLeft)) ConnectAnswer(0);
            }
            else connectedToCorner = false;
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                if (CheckDistance(bottomRight)) ConnectAnswer(3);
            }
            // top
            else if (pos.y > threshold)
            {
                if (CheckDistance(topRight)) ConnectAnswer(1);
            }
            else connectedToCorner = false;
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

    void ConnectAnswer(int index)
    {
        connectedToCorner = true;
        GameManager.Instance.PreviewAnswer(index);
    }

    void DisconnectAnswer()
    {
        connectedToCorner = false;
        GameManager.Instance.FadeOutSpeaker();
    }

}
