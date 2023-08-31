using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script contains all the information of the object that works as Question.

//Structure of responses within the Question object
public struct Answer
{
    [SerializeField]
    private string _info;
    public string Info { get { return _info; } }

    [SerializeField]
    private bool _right;
    public bool Right { get { return _right; } }
}

//Object Structure Question
[CreateAssetMenu(fileName = "New Question", menuName = "Quiz/new Question")]
public class Question : ScriptableObject
{
    //Answers
    public enum AnswerType { Multi, One }

    [SerializeField] private string _info = string.Empty;
    public string Info { get { return _info; } }

    [SerializeField] Answer[] _answers = null;
    public Answer[] Answers { get { return _answers; } }

    //Parameters

    [SerializeField] private bool _useTimer = false;
    public bool UseTimer { get { return _useTimer; } }

    [SerializeField] private int _timer = 0;
    public int Timer { get { return _timer; } }

    [SerializeField] private AnswerType _answerType = AnswerType.Multi;
    public AnswerType AnswerTypeGet { get { return _answerType; } }

    [SerializeField] private int _plusPoints = 10;
    public int PlusPoints { get { return _plusPoints; } }

    //Show the right answer
    public List<int> GetRightAnswer()
    {
        List<int> RightAnswer = new List<int>();
        for (int i = 0; i < Answers.Length; i++)
        {
            if (Answers[i].Right)
            {
                RightAnswer.Add(i);
            }
        }
        return RightAnswer;
    }

}
