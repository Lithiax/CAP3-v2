using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
//using System.Threading.Tasks;
//using System.Threading;
public class Splashscreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer splashcreenVideo;
    [SerializeField]
    private GameObject titlescreen;
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private BlinkEffect tip;
  IEnumerator runningCoroutine;
    private void Start()
    {
        float waitTime = (float)splashcreenVideo.clip.length;

        InitializeSplashscreen(waitTime);

    }
    public void InitializeSplashscreen(float p_videoLength)
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        runningCoroutine = Co_InitializeSplashscreen(p_videoLength);
        StartCoroutine(runningCoroutine);

       
    }
  
    IEnumerator Co_InitializeSplashscreen(float p_videoLength)
    {

        TransitionUI.onFadeTransition.Invoke(0, false);
        yield return new WaitForSeconds(0.5f);
        
        splashcreenVideo.Play();
        yield return new WaitForSeconds(p_videoLength);
        if (runningCoroutine != null)
        {
            //StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
        runningCoroutine = Co_OpenTitlescreen();
        StartCoroutine(runningCoroutine);



    }

    public void OnSkipButtonUIClicked()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
   
        runningCoroutine = Co_OpenTitlescreen();
        StartCoroutine(runningCoroutine);

    }
    IEnumerator Co_OpenTitlescreen()
    {
        tip.Stop();
        TransitionUI.onFadeTransition.Invoke(1);
        yield return new WaitForSeconds(0.5f);
        titlescreen.SetActive(true);
        background.SetActive(true);
        TransitionUI.onFadeTransition.Invoke(0);



    }


  
}
