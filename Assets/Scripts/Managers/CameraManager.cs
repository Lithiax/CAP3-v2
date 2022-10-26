using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
public class CameraMovedEvent : UnityEvent<Vector2> { }
public class ShakeCameraEvent : UnityEvent { }
public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CameraManager>();
            }

            return _instance;
        }
    }
    [SerializeField] public Camera worldCamera;
    [SerializeField] public Camera uiCamera;
    [SerializeField] public List<RectTransform> rectTransforms;
    List<Vector2> savedPositions = new List<Vector2>();
    public static CameraMovedEvent onCameraMovedEvent = new CameraMovedEvent();
    public static ShakeCameraEvent onShakeCameraEvent = new ShakeCameraEvent();

    [Header("Camera Shake Position Settings")]
    [SerializeField] float shakePositionDuration = 0.2f;
    [SerializeField] Vector3 shakePositionPower = new Vector3(0.05f, 0.05f);
    [SerializeField] int shakePositionVibrato = 1;
    [SerializeField] float shakePositionRandomRange = 1f;
    [SerializeField] bool shakePositionCanFade = true;

    [Header("Camera Shake Rotation Settings")]
    [SerializeField] float shakeRotationDuration = 0.2f;
    [SerializeField] Vector3 shakeRotationPower = new Vector3(0.05f, 0.05f);
    [SerializeField] int shakeRotationVibrato = 1;
    [SerializeField] float shakeRotationRandomRange = 1f;
    [SerializeField] bool shakeRotationCanFade = true;

    [Header("Camera Zoom In Settings")]
    [SerializeField] float zoomInSize = 13f;
    [SerializeField] float zoomInDuration = 1f;

    [SerializeField] float delayTime = 1f;

    [Header("Camera Zoom Out Settings")]
    [SerializeField] float zoomOutSize = 15f;
    [SerializeField] float zoomOutDuration = 1f;

    public bool tutorialOn = true;
    private void Awake()
    {
       
        _instance = this;
        //If condition is true then do expression 1, else do expression 2
        worldCamera = worldCamera ? worldCamera : GameObject.Find("World Camera").GetComponent<Camera>();
        uiCamera = worldCamera ? worldCamera : GameObject.Find("UI Camera").GetComponent<Camera>();
        onShakeCameraEvent.AddListener(ShakeCamera);
        for (int i =0; i < rectTransforms.Count; i++)
        {
            savedPositions.Add(rectTransforms[i].anchoredPosition);
        }
    }

    private void OnDestroy()
    {
        onShakeCameraEvent.RemoveListener(ShakeCamera);

    }

    private void OnEnable()
    {
        ResetCamera();
    }

    Vector2 Vector2Abs(Vector2 p_vector2)
    {
        Vector2 answer = new Vector2(Mathf.Abs(p_vector2.x), Mathf.Abs(p_vector2.y));
        return answer;
    }
    public void ResetCamera()
    {
        if (!tutorialOn)
        {
            //Debug.Log("Invoked: " + defaultRoom.transform.position + ", " + panLimit);
            onCameraMovedEvent.Invoke(new Vector2(0,0));
        }

    }

    public void ShakeCamera()
    {
        Debug.Log("RAA");
        StartCoroutine(Co_ShakeCamera());
    }
    public IEnumerator Co_ShakeCamera()
    {


        var sequence = DOTween.Sequence()
        .Append(rectTransforms[0].DOShakePosition(shakePositionDuration, shakePositionPower, shakePositionVibrato, shakePositionRandomRange, shakePositionCanFade));
        for (int i= 1; i < rectTransforms.Count; i++)
        {
            sequence.Join(rectTransforms[i].DOShakePosition(shakeRotationDuration, shakeRotationPower, shakeRotationVibrato, shakeRotationRandomRange, shakeRotationCanFade));
        }
        //sequence.Join(worldCamera.DOShakeRotation(shakeRotationDuration, shakeRotationPower, shakeRotationVibrato, shakeRotationRandomRange, shakeRotationCanFade));
        sequence.Play();
        yield return sequence.WaitForCompletion();
        for (int i = 0; i < rectTransforms.Count; i++)
        {
            rectTransforms[i].anchoredPosition = savedPositions[i];
        }
        Debug.Log("FINISHED");
    }

    public void ZoomCamera()
    {
        StartCoroutine(Co_ZoomCamera());
    }
    public IEnumerator Co_ZoomCamera()
    {
        var sequence = DOTween.Sequence()
        .Append(worldCamera.DOOrthoSize(zoomInSize, zoomInDuration));

        sequence.Play();
        yield return sequence.WaitForCompletion();
        var sequenceTwo = DOTween.Sequence()
       .Append(worldCamera.DOOrthoSize(zoomOutSize, zoomOutDuration));
        yield return new WaitForSeconds(delayTime);
        sequenceTwo.Play();
        yield return sequenceTwo.WaitForCompletion();
        StartCoroutine(Co_ZoomCamera());
    }
}
