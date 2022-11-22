using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CalendarArrowUI : MonoBehaviour
{
    public Vector3 startPos;

    Image arrowImage;

    [SerializeField] float frequency = 5f;
    [SerializeField] float magnitude = 5f;
    [SerializeField] float offset = 0f;

    [SerializeField] List<Color> arrowColors;
    int currentColorIndex = 0;

    bool startHover = false;
    void Awake()
    {
        startPos = transform.position;
        arrowImage = GetComponent<Image>();
    }

    public void StartHovering()
    {
        startPos = transform.position;

        startHover = true;
    }

    public void SetHovering(bool b)
    {
        startPos = new Vector3(transform.position.x, startPos.y, startPos.z);
        transform.position = startPos;

        startHover = b;
    }  

    public void ChangeColor()
    {
        currentColorIndex++;

        arrowImage.DOColor(arrowColors[currentColorIndex], 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startHover) return;
        transform.position = startPos + transform.up * Mathf.Sin(Time.time * frequency + offset) * magnitude;
    }
}
