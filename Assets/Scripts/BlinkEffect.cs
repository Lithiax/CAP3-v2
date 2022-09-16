using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI affectedText;
    [SerializeField] private float blinkRate;
    [SerializeField]
    private Button button;
    [SerializeField] private float delay;

    IEnumerator runningCoroutine;

    private void Awake()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        runningCoroutine = Co_Start();
        StartCoroutine(runningCoroutine);
    }

    private void OnDestroy()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

    }

    IEnumerator Co_Start()
    {
        yield return new WaitForSeconds(delay);
        button.enabled = true;
        if (runningCoroutine != null)
        {
      
            runningCoroutine = null;
        }
        runningCoroutine = Co_Blink();
        StartCoroutine(runningCoroutine);
    }
    IEnumerator Co_Blink()
    {
       
        
           
        affectedText.enabled = true;
        yield return new WaitForSeconds(blinkRate);
        affectedText.enabled = false;
        yield return new WaitForSeconds(blinkRate);
        if (runningCoroutine != null)
        {
           
            runningCoroutine = null;
        }
        runningCoroutine = Co_Blink();
        StartCoroutine(runningCoroutine);
        
    
   
    }

    public void Stop()
    {
        button.enabled = false;
        button.GetComponent<Image>().raycastTarget = false;
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        affectedText.enabled = false;
    }
}
