using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<GameObject> panels;
    void Awake()
    {
        StartCoroutine(Co_AudioFadeIn());
    }
    public IEnumerator Co_AudioFadeOut()
    {
        DialogueSpreadSheetPatternConstants.penelopeHealth = 50;
        DialogueSpreadSheetPatternConstants.bradHealth = 50;
        DialogueSpreadSheetPatternConstants.liamHealth = 50;
        DialogueSpreadSheetPatternConstants.maeveHealth = 50;

        DialogueSpreadSheetPatternConstants.effects.Clear();
    Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(audioSource.DOFade(0, 1.25f));
        fadeOutSequence.Play();
        StorylineManager.firstTime = true;
        StorylineManager.LoadVisualNovel("Maeve1", "Week1");
        //SO_Character mainCharacter = Resources.Load<SO_Character>("Scriptable Objects/Characters/You");
        //mainCharacter.stageName = "You";
        //StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + "Maeve1" + "/" + "Week1");
        //StorylineManager.currentInteractibleChoices = Resources.Load<SO_InteractibleChoices>("Scriptable Objects/Dialogues/Visual Novel/" + "Maeve1" + "/" + "Interactible Choices");
        DialogueSpreadSheetPatternConstants.effects.Add("<progress>");
        LoadingUI.instance.InitializeLoadingScreen("VisualNovel");
        yield return fadeOutSequence.WaitForCompletion();
        audioSource.Stop();
 
    }

    public IEnumerator Co_AudioFadeIn()
    {
        audioSource.volume = 0;
        audioSource.Play();
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Append(audioSource.DOFade(1, 1.25f));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();

    }
    void SetAspect()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = Camera.main;//GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject p in panels)
            {
                p.SetActive(false);
            }
        }
    }
    public void NewGameButton()
    {
        StaticUserData.Reset();
        StartCoroutine(Co_AudioFadeOut());
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void ActivatePanel(GameObject panel)
    {
        foreach (GameObject p in panels)
        {
            p.SetActive(false);

            if (p == panel)
                p.SetActive(true);
        }
    }

    public void MainMenuButton()
    {
        LoadingUI.instance.InitializeLoadingScreen("MainMenu");
    }
}
