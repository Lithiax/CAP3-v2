using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarArrowUI : MonoBehaviour
{
    public Vector3 startPos;

    [SerializeField] float frequency = 5f;
    [SerializeField] float magnitude = 5f;
    [SerializeField] float offset = 0f;

    bool startHover = false;
    void Awake()
    {
        startPos = transform.position;
    }

    public void StartHovering()
    {
        startPos = transform.position;
        startHover = true;
    }

    public void SetHovering(bool b)
    {
        startHover = b;
    }  

    // Update is called once per frame
    void Update()
    {
        if (!startHover) return;
        transform.position = startPos + transform.right * Mathf.Sin(Time.time * frequency + offset) * magnitude;
    }
}
