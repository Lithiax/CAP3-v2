using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

[System.Serializable]
public class ProgressionData
{
    public int CurrentWeek { get; private set; }
    public int CurrentMonth { get; private set; }

    public ProgressionData(int month, int week)
    {
        CurrentMonth = Mathf.Clamp(month, 1, 3);
        CurrentWeek = Mathf.Clamp(week % 5, 1, 4);
    }

    public void ProgressDate()
    {
        CurrentWeek++;

        if (CurrentWeek > 4)
        {
            CurrentWeek = 1;
            CurrentMonth++;
            CurrentMonth = Mathf.Clamp(CurrentMonth, 1, 3);
        }
    }
}

public class CalendarUI : MonoBehaviour, IDataPersistence
{
    [Header("UI Elements")]
    [SerializeField] GameObject panelParent;
    [SerializeField] List<GameObject> mainPanels;
    [SerializeField] GameObject xMarkParent;
    [SerializeField] List<Image> xMarks;
    [SerializeField] GameObject arrow;

    [Header("Tween Values")]


    ProgressionData progressionData;
    Vector2 bgPanelPos;

    Vector3 datePanelOldLocalPos;
    Vector3 xMarksParentOldLocalPos;

    readonly int BGDifference = 1920;

    public Action OnAnimationDone;
    private void Awake()
    {
        if (StaticUserData.ProgressionData == null)
        {
            progressionData = new ProgressionData(1, 1);
        }
        else
        {
            progressionData = StaticUserData.ProgressionData;
        }
        datePanelOldLocalPos = panelParent.transform.localPosition;
        xMarksParentOldLocalPos = xMarkParent.transform.localPosition;
    }
    public void Init()
    {
        Debug.Log("M" + progressionData.CurrentMonth + "W" + progressionData.CurrentWeek);
        SetDatePanel(progressionData.CurrentMonth);
        SetXMarks(progressionData.CurrentWeek);
        SetArrow(progressionData.CurrentWeek);
    }

    public void StartAnimation()
    {
        StartCoroutine(MainLoop());
    }

    IEnumerator MainLoop()
    {
        yield return new WaitForSeconds(1f);

        XMarkFadeIn(progressionData.CurrentWeek, () =>
        {
            MoveArrow(progressionData.CurrentWeek, () =>
            {
                if (progressionData.CurrentMonth >= 2 && progressionData.CurrentWeek == 1)
                {
                    MoveDatePanel(progressionData.CurrentMonth, () => OnAnimationDone?.Invoke());
                }
                else
                {
                    OnAnimationDone?.Invoke();
                }
            });
        });

        yield return new WaitForSeconds(3f);
    }

    IEnumerator Debugger()
    {
        while (progressionData.CurrentWeek <= 5 || progressionData.CurrentMonth != 3)
        {
            Debug.Log("M" + progressionData.CurrentMonth + "W" + progressionData.CurrentWeek);
            yield return new WaitForSeconds(1f);

            XMarkFadeIn(progressionData.CurrentWeek, () =>
            {
                MoveArrow(progressionData.CurrentWeek, () =>
                {
                    if (progressionData.CurrentMonth >= 2 && progressionData.CurrentWeek == 1)
                    {
                        MoveDatePanel(progressionData.CurrentMonth);
                    }
                });
            });

            yield return new WaitForSeconds(4f);
            progressionData.ProgressDate();
        }
    }

    void SetXMarks(int week)
    {
        if (week == 1 && progressionData.CurrentMonth == 1)
        {
            return;
        }
        else if (week == 1)
        {
            week = 4;
        }

        for (int i = 0; i < week - 1; i++)
        {
            xMarks[i].color = new Color(255, 255, 255, 1);
        }
    }

    void SetDatePanel(int month)
    {
        if (progressionData.CurrentWeek == 1) return;
        float posX = datePanelOldLocalPos.x;
        posX -= (month - 1) * BGDifference;

        panelParent.transform.localPosition = new Vector3(posX, panelParent.transform.localPosition.y, panelParent.transform.localPosition.z);
    }

    void SetArrow(int week)
    {
        if (week == 1 && progressionData.CurrentMonth != 1)
        {
            week = 4;
        }

        float xPos = xMarks[week - 1].gameObject.transform.position.x;
        arrow.transform.position = new Vector3(xPos, arrow.transform.position.y, arrow.transform.position.z);

        arrow.GetComponent<CalendarArrowUI>().StartHovering();
    }

    void MoveDatePanel(int month, TweenCallback onEnd = null)
    {
        float posX = datePanelOldLocalPos.x;
        float posX2 = xMarksParentOldLocalPos.x;
        posX -= (month - 1) * BGDifference;
        posX2 -= (month - 1) * BGDifference;

        Tween tween = panelParent.transform.DOLocalMoveX(posX, 0.5f);
        xMarkParent.transform.DOLocalMoveX(posX2, 0.5f).OnComplete(() => 
        {
            for (int i = 0; i < xMarks.Count; i++)
            {
                xMarks[i].color = new Color(255, 255, 255, 0);
            }

            xMarkParent.transform.localPosition = new Vector3(xMarksParentOldLocalPos.x, xMarkParent.transform.localPosition.y,
                xMarkParent.transform.localPosition.z);
        });

        tween.onComplete += onEnd;
    }

    void XMarkFadeIn(int week, TweenCallback onEnd = null)
    {
        if (week == 1 && progressionData.CurrentMonth == 1)
        {
            onEnd?.Invoke();
            return;
        }
        else if (week == 1)
        {
            week = 5;
        }

        Image mark = xMarks[week - 2];
        Tween tween = mark.DOFade(1, 0.5f);
        tween.onComplete += onEnd;
    }

    void MoveArrow(int week, TweenCallback onEnd = null)
    {
        CalendarArrowUI arrowComp = arrow.GetComponent<CalendarArrowUI>();

        float targetX = xMarks[week-1].gameObject.transform.localPosition.x;
        Tween tween = arrow.transform.DOLocalMoveX(targetX, 1).OnComplete(() =>
        {
            arrowComp.startPos = new Vector3(arrow.transform.position.x, arrowComp.startPos.y, arrowComp.startPos.z);
        });

        tween.onComplete += onEnd;
    }

    private void OnDisable()
    {
        StaticUserData.ProgressionData = progressionData;
    }

    void IDataPersistence.LoadData(GameData data)
    {
        if (data.ProgressionData != null)
        {
            progressionData = data.ProgressionData;
        }
    }


    void IDataPersistence.SaveData(ref GameData data)
    {
        data.ProgressionData = progressionData;
    }
}
