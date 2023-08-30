using UnityEngine;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Quiz/ new GameEvents")]
public class GameEvents : ScriptableObject
{
    //Update questions in the UI so they can be displayed during games.
    public delegate void UpdateQuestionsUICallback(Question question);
    public UpdateQuestionsUICallback UpdateQuestionsUI;

    //Updates the response prefab sprites based on whether they are selected or not.
    public delegate void UpdateAnswerToQuestionsCallback(AnswersData choosenAnswer);
    public UpdateAnswerToQuestionsCallback UpdateAnswerToQuestions;

    /*It shows the screen that appears to indicate if our answer was correct or incorrect, 
     * the points we won or lost, 
    and the game over screen that has some buttons and the high score.*/
    public delegate void ShowResultsScreenCallback();
    public ShowResultsScreenCallback ShowResultsScreen;

    //Update the text with the dots in the UI.
    public delegate void UpdateScoreCallback();
    public UpdateScoreCallback UpdateScore;

    //Store Score.
    [HideInInspector]
    public int ActualFinalScore;

    //Store High Score.
    [HideInInspector]
    public int StartupHighScore;
}
