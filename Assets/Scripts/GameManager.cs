using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Questions Array
    Question[] _questions = null;
    public Question[] Preguntas { get { return _questions; } }

    //GameEvents
    //[SerializeField] GameEvents events = null;

    //Timer
    [SerializeField] Animator timerAnimator = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [SerializeField] Color colorHalfTimer = Color.yellow;
    [SerializeField] Color colorTimerRunningOut = Color.red;
    private Color colorTimer = Color.white;
    private int parametroTimerStateHash = 0;

    //Answers & questions Control
    /*private List<AnswersData> ChoosenAnswers = new List<AnswersData>();
    private List<int> FinishedQuestion = new List<int>();*/
    private int actualQuestion = 0;

}
