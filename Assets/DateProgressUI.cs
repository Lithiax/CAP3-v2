using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DateProgressUI : MonoBehaviour
{

    [Header("Hearts")]
    [SerializeField] List<RectTransform> hearts;

    public void SetHearts(int dateProgress)
    {
        if (dateProgress > 2) return;

        for (int i = 0; i < dateProgress; i++)
        {
            hearts[i].DOScale(1.1f, 0);
        }
    }

    public void AddHearts(ref int dateProgress, string data)
    {
        int num = int.Parse(data);
        if (dateProgress == 2) return;

        dateProgress = num;
        //hearts[dateProgress-1].DOScale(1.1f, 0.5f);
        StartCoroutine(HeartAnim(dateProgress));
    }

    public void IncHearts(ref int dateProgress)
    {
        if (dateProgress == 2) return;

        dateProgress++;

        //hearts[dateProgress - 1].DOScale(1.1f, 0.5f);
        StartCoroutine(HeartAnim(dateProgress));
    }

    IEnumerator HeartAnim(int dateProgress)
    {
        yield return new WaitForSeconds(1f);
        hearts[dateProgress - 1].DOScale(1.1f, 0.5f);
    }
}
