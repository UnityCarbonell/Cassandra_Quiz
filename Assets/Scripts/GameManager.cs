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
    public Question[] Questions { get { return _questions; } }

    //GameEvents
    [SerializeField] GameEvents events = null;

    //Timer
    [SerializeField] Animator timerAnimator = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [SerializeField] Color colorHalfTimer = Color.yellow;
    [SerializeField] Color colorTimerRunningOut = Color.red;
    private Color colorTimer = Color.white;
    private int parametroTimerStateHash = 0;

    //Track responses to each question, each question asked, and the current question.
    private List<AnswersData> ChoosenAnswers = new List<AnswersData>();
    private List<int> FinishedQuestion = new List<int>();
    private int actualQuestion = 0;

    //Animations delay
    private IEnumerator IE_WaitUntillNextRound = null;
    private IEnumerator IE_StartTimer = null;

    //Check if the game ended
    private bool AllFinished
    {
        get
        {
            return (FinishedQuestion.Count < Questions.Length) ? false : true;
        }
    }

    //Call the GameEvents
    void OnEnable()
    {

    }
    void OnDisable()
    {
        
    }

    //On Start
    void Awake()
    {
        events.ActualFinalScore = 0;
    }

    void Start()
    {
        //events.StartupHighScore = PlayerPrefs.GetInt();
        colorTimer = timerText.color;

    }
}
