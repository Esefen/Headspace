using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnswerNumber { Two, Three, Four };

public class Question : MonoBehaviour
{
    public AnswerNumber possibleAnswers;
    public AudioClip intro;
    public AudioClip[] previewAnswers;
    public AudioClip[] answers;
}
