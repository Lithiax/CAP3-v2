using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class HealthOverheadUI : MonoBehaviour
{
    private bool isRevealed;
    [SerializeField] private float unrevealTimeOut;
    public Health health;
    private Camera cam;
    [SerializeField] private Vector2 positionCorrection = new Vector2(0, 40);
    private RectTransform targetCanvas;
    private RectTransform healthBarTransform;
    [SerializeField] private Image healthFrame;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image delayedBar;
    private Transform objectToFollow;
    IEnumerator currentTimeOut;
    //private IEnumerator runningCoroutine;
    private IEnumerator runningUpdateCoroutine;
    float fill;

    public void OnHealthDied()
    {
        if (currentTimeOut != null)
        {
            StopCoroutine(currentTimeOut);
            currentTimeOut = null;
        }
        if (runningUpdateCoroutine != null)
        {
            StopCoroutine(runningUpdateCoroutine);
            runningUpdateCoroutine = null;
        }
        isRevealed = false;
        healthFrame.gameObject.SetActive(false);
        //HealthOverheadUIPool.pool.Release(this); 
        
       
    }

    public IEnumerator Co_RevealTimeOut()
    {
        yield return new WaitForSeconds(unrevealTimeOut);
        healthFrame.gameObject.SetActive(false);
        isRevealed = false;
        if (currentTimeOut != null)
        {
            StopCoroutine(currentTimeOut);
            currentTimeOut = null;
        }

        if (runningUpdateCoroutine != null)
        {
            StopCoroutine(runningUpdateCoroutine);
            runningUpdateCoroutine = null;
        }

        //Debug.Log(gameObject.name + " - ");

        Deinit();


    }
    public void Deinit()
    {
        health.onHealthModifiedEvent.RemoveListener(OnHealthChanged);
        health.OnDeathEvent.RemoveListener(OnHealthDied);
        health.healthOverheadUI = null;
        Destroy(gameObject); // TEMPORARY object pool this
       // HealthOverheadUIPool.pool.Release(this);
    }
    public void SetHealthBarData(Transform p_targetTransform, RectTransform p_healthBarPanel)
    {
        cam = cam ? cam : CameraManager.instance.worldCamera;
      
        isRevealed = false;
        this.targetCanvas = p_healthBarPanel;
        healthBarTransform = GetComponent<RectTransform>();
        objectToFollow = p_targetTransform;
        //Debug.Log("TES");
        healthFrame.gameObject.SetActive(true);
        transform.SetParent(p_healthBarPanel, false);

    }

    public IEnumerator Co_UpdatePosition()
    {
      
        RepositionHealthBar();
        //Debug.Log("REPOSITIONING");
        yield return new WaitForSeconds(0.05f);
        //Debug.Log("REPOSITIONING");
        if (runningUpdateCoroutine != null)
        {
            StopCoroutine(runningUpdateCoroutine);
            runningUpdateCoroutine = null;
        }
        runningUpdateCoroutine = Co_UpdatePosition();
        StartCoroutine(runningUpdateCoroutine);

       // Debug.Log("REPOSITIONING 2");
    }
    public void OnHealthChanged(bool p_isAlive, float p_currentHealth, float p_maxHealth)
    {
        if (p_isAlive)
        {
            //Debug.Log("RES");
            if (!isRevealed)
            {
                if (runningUpdateCoroutine != null)
                {
                    StopCoroutine(runningUpdateCoroutine);
                    runningUpdateCoroutine = null;
                }
                runningUpdateCoroutine = Co_UpdatePosition();
                StartCoroutine(runningUpdateCoroutine);



                healthFrame.gameObject.SetActive(true);
                isRevealed = true;

            }

            fill = p_currentHealth / p_maxHealth;

      

            StartCoroutine(Co_Transition());


        

            if (currentTimeOut != null)
            {
                StopCoroutine(currentTimeOut);
                currentTimeOut = null;
            }
            currentTimeOut = Co_RevealTimeOut();
            StartCoroutine(currentTimeOut);
        }
       

    }
  

    IEnumerator Co_Transition()
    {
        //white
        healthBar.color = new Color(255, 255, 255);

        //black
        Tween WhiteToBlack = healthBar.DOColor(new Color(0, 0, 0), 0.05f);
        yield return WhiteToBlack.WaitForCompletion();

        Tween BlackToRed = healthBar.DOColor(new Color(255, 0, 0), 0.05f);
        yield return BlackToRed.WaitForCompletion();

        healthBar.color = new Color(0, 248, 0);//reset
        healthBar.fillAmount = fill;
        yield return new WaitForSeconds(1f);

        Sequence s = DOTween.Sequence();
        s.Join(delayedBar.DOFade(0f, 0.35f));
        s.Join(delayedBar.DOFillAmount(fill, 0.35f));
        s.Play();
        yield return s.WaitForCompletion();
        delayedBar.color = new Color32(250, 255, 255, 255);//reset
        //delayedBar.DOFade(1f, 0.01f); //reset
    }
   
    private void RepositionHealthBar()
    {

        Vector2 ViewportPosition = cam.WorldToViewportPoint(objectToFollow.position);

        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * targetCanvas.sizeDelta.x)- (targetCanvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * targetCanvas.sizeDelta.y) - (targetCanvas.sizeDelta.y * 0.5f)));

        WorldObject_ScreenPosition += new Vector2(positionCorrection.x, positionCorrection.y);

        healthBarTransform.anchoredPosition = WorldObject_ScreenPosition;

    }

}
