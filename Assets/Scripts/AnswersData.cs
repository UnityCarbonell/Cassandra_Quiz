using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswersData : MonoBehaviour
{
    //This script contains all the information of the Prefab where the responses are generated
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI infoTextObject;
    [SerializeField] Image toggle;

    [Header("Sprites")]
    [SerializeField] Sprite unchekedToggle;
    [SerializeField] Sprite chekedToggle;

    [Header("References")]
    [SerializeField] GameEvents events;

    //Make sure the RectTransform component exists in the prefab
    private RectTransform _rect;
    public RectTransform Rect
    {
        get
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
            }
            return _rect;
        }
    }

    private int _indexAnswer = -1;
    public int IndexAnswer { get { return _indexAnswer; } }

    public bool Checked = false;

    //This part of the code makes sure that the prefab changes a sprite when you select that answer, in addition to notifying that this answer is selected.
    public void UpdateData(string info, int index)
    {
        infoTextObject.text = info;
        _indexAnswer = index;
    }

    public void Reset()
    {
        Checked = false;
        UpdateUI();
    }

    public void ChangeState()
    {
        Checked = !Checked;
        UpdateUI();

        if (events.UpdateAnswerToQuestions != null)
        {
            events.UpdateAnswerToQuestions(this);
        }
    }

    void UpdateUI()
    {
        toggle.sprite = (Checked) ? chekedToggle : unchekedToggle;
    }

}
