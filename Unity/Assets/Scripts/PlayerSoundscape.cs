using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundscape : MonoBehaviour
{
    const float threshold = 0.65f;

    float xMin, yMin;
    Vector2 center;

    Question currentQuestion;
    public AudioClip testHautGauche;
    public AudioClip testHautDroite;
    public AudioClip testBasGauche;
    public AudioClip testBasDroite;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameState = AppState.Answer;

        xMin = 1 - threshold;
        yMin = 1 - threshold;
        center = new Vector2(Screen.width / 2, Screen.height / 2);
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
                UpdateSoundVolume(pos);
            }
            // Return soundscape to default
            else
            {
                GameManager.Instance.FadeOutSpeaker();
                //StartCoroutine(GameManager.Instance.FadeOut());
                //GameManager.speaker.Stop();
            }
        }
    }

    // Modify soundscape
    void UpdateInteractiveSoundscape(Vector2 pos)
    {
        //Debug.Log("Mouse Position: " + Input.mousePosition.x + ";" + Input.mousePosition.y);
        //Debug.Log("Mouse Position: " + pos.x + ";" + pos.y);
        // left
        if (pos.x < xMin)
        {
            // bottom
            if (pos.y < yMin)
            {
                GameManager.Instance.PlayAnswer(testBasGauche);
            }
            // top
            else if (pos.y > threshold)
            {
                GameManager.Instance.PlayAnswer(testHautGauche);
            }
        }
        // right
        else if (pos.x > threshold)
        {
            // bottom
            if (pos.y < yMin)
            {
                GameManager.Instance.PlayAnswer(testBasDroite);
            }
            // top
            else if (pos.y > threshold)
            {
                GameManager.Instance.PlayAnswer(testHautDroite);
            }
        }
    }

    void UpdateSoundVolume(Vector2 pos)
    {
        float d = Vector3.Distance(center, Input.mousePosition) / ((float)Screen.width /2f);
        Debug.Log("Mouse distance from center: " + d);
        GameManager.Instance.UpdateAnswerSound(Mathf.Min(d, 1));
    }

}
