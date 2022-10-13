using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public class AnxButton : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI text;

    Vector3 dir;
    float speed = 3;
    // Start is called before the first frame update
    private void Awake()
    {
        image.color = SetColorTransparency(image.color, 0);
        text.color = SetColorTransparency(text.color, 0);

        float x = Random.Range(0f, 1f);
        float y = Random.Range(0f, 1f);

        dir = new Vector3(x, y, 0);
    }

    Color SetColorTransparency(Color c, float transparency)
    {
        Color color = c;
        color.a = 0;

        return color;
    }

    void OnEnable()
    {
        Fade(1);

    }

    private void Update()
    {
        //transform.position += dir * speed * Time.deltaTime;
    }

    void Fade(float endValue)
    {
        image.DOFade(endValue, 1f);
        text.DOFade(endValue, 1f)
            .OnComplete(() =>
        {
            AfterFade(endValue);
        });
    }

    void AfterFade(float value)
    {
        if (value != 0) return;

        GameObject.Destroy(this.gameObject);
    }

    public void SetLifeSpan(float life)
    {
        StartCoroutine(DestroyOnTimer(life));
    }

    IEnumerator DestroyOnTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Fade(0);
    }
}
