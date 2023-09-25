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
    private int parameterTimerStateHash = 0;

    //Track responses to each question, each question asked, and the current question.
    private List<AnswersData> ChoosenAnswers = new List<AnswersData>();
    private List<int> FinishedQuestion = new List<int>();
    public int actualQuestion = 0;

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
        events.UpdateAnswerToQuestions += UpdateAnswers;
    }
    void OnDisable()
    {
        events.UpdateAnswerToQuestions -= UpdateAnswers;
    }

    //On Start
    void Awake()
    {
        events.ActualFinalScore = 0;
    }

    void Start()
    {
        events.StartupHighScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        colorTimer = timerText.color;
        LoadQuestions();

        parameterTimerStateHash = Animator.StringToHash("TimerState");

        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);

        Show();
    }


    //Answers
    public void UpdateAnswers(AnswersData newAnswer)
    {
        if (Questions[actualQuestion].AnswerTypeGet == Question.AnswerType.One)
        {
            foreach (var answer in ChoosenAnswers)
            {
                if (answer != newAnswer)
                {
                    answer.Reset();
                }
            }
            ChoosenAnswers.Clear();
            ChoosenAnswers.Add(newAnswer);
        }
        else
        {
            bool alreadyChoosen = ChoosenAnswers.Exists(x => x == newAnswer);
            if (alreadyChoosen)
            {
                ChoosenAnswers.Remove(newAnswer);
            }
            else
            {
                ChoosenAnswers.Add(newAnswer);
            }
        }
    }

    public void DeleteAnswers()
    {
        ChoosenAnswers = new List<AnswersData>();
    }

    bool CompareAnswers()
    {
        if (ChoosenAnswers.Count > 0)
        {
            List<int> c = Questions[actualQuestion].GetRightAnswer();
            List<int> p = ChoosenAnswers.Select(x => x.IndexAnswer).ToList();

            var f = c.Except(p).ToList();
            var s = p.Except(c).ToList();

            return !f.Any() && !s.Any();
        }
        return false;
    }

    bool CheckAnswers()
    {
        if (!CompareAnswers())
        {
            return false;
        }
        return true;
    }

     //Show questions in the UI
    void Show()
    {
        DeleteAnswers();
        var question = RandomQuestion();

        if (events.UpdateQuestionsUI != null)
        {
            events.UpdateQuestionsUI(question);
        }
        else
        {
            Debug.LogWarning("An error occurred while trying to display the information for a new question in the UI. GameEvents.UpdateQuestionsUI is equal to null. We have a problem in the GameManager.Show() method");
        }

        if (question.UseTimer)
        {
            UpdateTimer(question.UseTimer);
        }
    }

    //Control what happens when you click the Next button
    public void Accept()
    {
        UpdateTimer(false);
        bool isRight = CheckAnswers();
        FinishedQuestion.Add(actualQuestion);

        UpdateScore((isRight) ? Questions[actualQuestion].PlusPoints : -Questions[actualQuestion].PlusPoints);

        if (AllFinished)
        {
            SetHighScore();
        }

        var type = (AllFinished) ? UIManager.FinalScreenType.Final : (isRight) ? UIManager.FinalScreenType.Right : UIManager.FinalScreenType.Wrong;

        if (events.ShowResultsScreen != null)
        {
            events.ShowResultsScreen(type, Questions[actualQuestion].PlusPoints);
        }

        AudioManager.Instance.SoundStop("TimerSFX");
        AudioManager.Instance.SoundPlay((isRight) ? "RightSFX" : "WrongSFX");

        if (type != UIManager.FinalScreenType.Final)
        {
            if (IE_WaitUntillNextRound != null)
            {
                StopCoroutine(IE_WaitUntillNextRound);
            }
        }

        IE_WaitUntillNextRound = WaitUntillNextRound();
        StartCoroutine(IE_WaitUntillNextRound);
    }

    //Timer
    void UpdateTimer(bool state)
    {
        switch (state)
        {
            case true:
                IE_StartTimer = StartTimer();
                StartCoroutine(IE_StartTimer);
                timerAnimator.SetInteger(parameterTimerStateHash, 2);
                break;
            case false:

                if (IE_StartTimer != null)
                {
                    StopCoroutine(IE_StartTimer);
                }

                timerAnimator.SetInteger(parameterTimerStateHash, 1);
                break;
        }
    }

    IEnumerator StartTimer()
    {
        var totalTime = Questions[actualQuestion].Timer;
        var timeLeft = totalTime;

        timerText.color = colorTimer;
        while (timeLeft > 0)
        {
            timeLeft--;

            AudioManager.Instance.SoundPlay("TimerSFX");

            if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4)
            {
                timerText.color = colorHalfTimer;
            }
            else if (timeLeft < totalTime / 4)
            {
                timerText.color = colorTimerRunningOut;
            }

            timerText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        Accept();
    }
     
    //Delay between rounds
    IEnumerator WaitUntillNextRound()
    {   
        yield return new WaitForSeconds(GameUtility.FinalWaitTime);
        Show();
    }

    //Questions
    Question RandomQuestion()
    {
        var randomIndex = RandomQuestionIndex();
        actualQuestion = randomIndex;

        return Questions[actualQuestion];
    }

    int RandomQuestionIndex()
    {
        var random = 0;
        if (FinishedQuestion.Count < Questions.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, Questions.Length);
            } while (FinishedQuestion.Contains(random) || random == actualQuestion);
        }

        return random;
    }

    void LoadQuestions()
    {
        Object[] objs = Resources.LoadAll("Questions", typeof(Question));
        _questions = new Question[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            _questions[i] = (Question)objs[i];
        }
    }

    //Game Over UI
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AudioManager.Instance.SoundPlay("UISFX");
    }

    public void Menu()
    {
        AudioManager.Instance.SoundPlay("UISFX");
        SceneManager.LoadScene("MENU");
    }

    public void ExitGame()
    {
        AudioManager.Instance.SoundPlay("UISFX");
        Application.Quit();
    }

    //High Scores
    public void DeleteHighScore()
    {
        AudioManager.Instance.SoundPlay("UISFX");
        PlayerPrefs.SetInt(GameUtility.SavePrefKey, 0);
    }

    private void SetHighScore()
    {
        var highScore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        if (highScore < events.ActualFinalScore)
        {
            PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.ActualFinalScore);
        }
    }

    private void UpdateScore(int add)
    {
        events.ActualFinalScore += add;
        
        if (events.UpdateScore != null)
        {
            events.UpdateScore();
        }
    }

}
