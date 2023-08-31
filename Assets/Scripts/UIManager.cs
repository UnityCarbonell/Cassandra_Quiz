using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//UI Parameters
[Serializable()]
public struct UIManagerParameters
{
    [Header("AnswerOptions")]
    [SerializeField] float margins;
    public float Margins { get { return margins; } }

    [Header("OptionsForFinalScreen")]
    [SerializeField] Color rightColorBG;
    public Color RightColorBG { get { return rightColorBG; } }
    [SerializeField] Color wrongColorBG;
    public Color WrongColorBG { get { return wrongColorBG; } }
    [SerializeField] Color finalColorBG;
    public Color FinalColorBG { get { return finalColorBG; } }
}

//Reference to the most important elements of the UI
[Serializable()]
public struct UIElements
{
    [SerializeField] RectTransform answersContent;
    public RectTransform AnswersContent { get { return answersContent; } }

    [SerializeField] TextMeshProUGUI questionInfo;
    public TextMeshProUGUI QuestionInfo { get { return questionInfo; } }

    [SerializeField] TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI ScoreTxt { get { return scoreTxt; } }

    [Space]

    [SerializeField] Animator finalScreenAnim;
    public Animator FinalScreenAnim { get { return finalScreenAnim; } }

    [SerializeField] Image resultsBG;
    public Image ResultsBG { get { return resultsBG; } }

    [SerializeField] TextMeshProUGUI resInfo;
    public TextMeshProUGUI ResInfo { get { return resInfo; } }

    [SerializeField] TextMeshProUGUI finalScore;
    public TextMeshProUGUI FinalScore { get { return finalScore; } }

    [Space]

    [SerializeField] TextMeshProUGUI highScoreTxt;
    public TextMeshProUGUI HighScoreTxt { get { return highScoreTxt; } }

    [SerializeField] CanvasGroup mainCanvasGroup;
    public CanvasGroup MainCanvasGroup { get { return mainCanvasGroup; } }

    [SerializeField] RectTransform finalElementsUI;
    public RectTransform FinalElementsUI { get { return finalElementsUI; } }
}


public class UIManager : MonoBehaviour
{
    public enum FinalScreenType { Right, Wrong, Final}

    //gameEvents reference
    [Header("References")]
    [SerializeField] GameEvents events;

    //Reference to the most important UI elements that are prefabs
    [Header("UIElementsPrefabs")]
    [SerializeField] AnswersData answerPrefab;
    [SerializeField] UIElements uIElements;

    [Space]

    //Parameters
    [SerializeField] UIManagerParameters parameters;
    List<AnswersData> actualAnswer = new List<AnswersData>();
    private int finalStateParameterHash = 0;
    private IEnumerator IE_ShowFinalTime = null;

    //Call the GameEvents
    void OnEnable()
    {
        events.UpdateQuestionsUI += UpdateQuestionsUI;
        events.ShowResultsScreen += ShowFinal;
        events.UpdateScore += UpdateScoreUI;
    }
    void OnDisable()
    {
        events.UpdateQuestionsUI -= UpdateQuestionsUI;
        events.ShowResultsScreen -= ShowFinal;
        events.UpdateScore -= UpdateScoreUI;
    }

    //Executed on Start
    void Start()
    {
        finalStateParameterHash = Animator.StringToHash("ScreenState");
    }

    //UI updates to display different data
    void UpdateQuestionsUI(Question question)
    {
        uIElements.QuestionInfo.text = question.Info;
        CreateAnswers(question);
    }

    //Answers
    void CreateAnswers(Question question)
    {
        DeleteAnswers();

        float offset = 0 - parameters.Margins;

        for (int i = 0; i < question.Answers.Length; i++)
        {
            AnswersData newAnswer = (AnswersData)Instantiate(answerPrefab, uIElements.AnswersContent);
            newAnswer.UpdateData(question.Answers[i].Info, i);
            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);
            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);
            uIElements.AnswersContent.sizeDelta = new Vector2(uIElements.AnswersContent.sizeDelta.x, offset * -1);
            actualAnswer.Add(newAnswer);
        }
    }

    void DeleteAnswers()
    {
        foreach (var answer in actualAnswer)
        {
            Destroy(answer.gameObject);
        }
        actualAnswer.Clear();
    }

    //Show the final screen
    void ShowFinal(FinalScreenType type, int score)
    {
        UpdateFinalUI(type, score);
        uIElements.FinalScreenAnim.SetInteger(finalStateParameterHash, 2);
        uIElements.MainCanvasGroup.blocksRaycasts = false;

        if (type != FinalScreenType.Final)
        {
            if (IE_ShowFinalTime != null)
            {
                StopCoroutine(IE_ShowFinalTime);
            }
            IE_ShowFinalTime = ShowFinalTime();
            StartCoroutine(IE_ShowFinalTime);
        }
    }

    void UpdateFinalUI(FinalScreenType type, int score)
    {
        var hs = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        switch (type)
        {
            case FinalScreenType.Right:
                uIElements.ResultsBG.color = parameters.RightColorBG;
                uIElements.ResInfo.text = "¡GOOD WORK!";
                uIElements.FinalScore.text = "+" + score;
                break;
            case FinalScreenType.Wrong:
                uIElements.ResultsBG.color = parameters.WrongColorBG;
                uIElements.ResInfo.text = "¡OH NO!";
                uIElements.FinalScore.text = "-" + score;
                break;
            case FinalScreenType.Final:
                uIElements.ResultsBG.color = parameters.FinalColorBG;
                uIElements.ResInfo.text = "GAME OVER";
                StartCoroutine(CalculateScore());
                uIElements.FinalElementsUI.gameObject.SetActive(true);
                uIElements.HighScoreTxt.gameObject.SetActive(true);
                uIElements.HighScoreTxt.text = ((hs > events.StartupHighScore) ? "<color=yellow>Nuevo </color>" : string.Empty) + "HIGH SCORE: " + hs;
                break;
            default:
                break;
        }
    }

    IEnumerator ShowFinalTime()
    {
        yield return new WaitForSeconds(GameUtility.FinalWaitTime);
        uIElements.FinalScreenAnim.SetInteger(finalStateParameterHash, 1);
        uIElements.MainCanvasGroup.blocksRaycasts = true;
    }

    //Score
    IEnumerator CalculateScore()
    {
        var scoreValue = 0;
        while (scoreValue < events.ActualFinalScore)
        {
            scoreValue++;
            uIElements.FinalScore.text = scoreValue.ToString();
            yield return null;
        }
    }

    void UpdateScoreUI()
    {
        uIElements.ScoreTxt.text = "SCORE: " + events.ActualFinalScore;
    }
}
