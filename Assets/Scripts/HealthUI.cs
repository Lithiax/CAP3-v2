using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public delegate bool IsWithinHealthCondition(int p_healthCeilingCondition, int p_healthFloorCondition);

[System.Serializable]
public class InstantColorData : UnityTransitionData
{

    [SerializeField] public Color32 color;
    public override void PerformTransition()
    {
        bar.color = color;

    }

}

[System.Serializable]
public class ColorTransitionData : DoTweenTransitionData
{
    [SerializeField] public Color32 amount;
    [SerializeField] public float transitionTime;
    public override Tween GetAndPerformTween()
    {
        Tween colorTransition = bar.DOColor(amount, transitionTime);
        return colorTransition;
    }

}
[System.Serializable]
public class FadeTransitionData : DoTweenTransitionData
{
    [SerializeField] public float amount;
    [SerializeField] public float transitionTime;
    public override Tween GetAndPerformTween()
    {
        Tween colorTransition = bar.DOFade(amount, transitionTime);
        return colorTransition;
    }
}

[System.Serializable]
public class FillTransitionData : DoTweenTransitionData
{
    [SerializeField] public float amount;
    [SerializeField] public float transitionTime;
    public override Tween GetAndPerformTween()
    {
        Tween colorTransition = bar.DOFillAmount(amount, transitionTime);
        return colorTransition;
    }
}

[System.Serializable]
public class DoTweenTransitionData : TransitionData
{
    [SerializeField] public bool joinToNextTransition;
    [SerializeField] public bool waitToFinish;
    [SerializeField] public float delayTimeToNextTransition;
    public virtual Tween GetAndPerformTween()
    {
        return null;
    }
}

[System.Serializable]
public class UnityTransitionData : TransitionData
{

    public override void PerformTransition()
    {

    }
}
[System.Serializable]
public enum TransitionType
{
    None,
    InstantColorData,
    FadeTransitionData,
    FillTransitionData,
    DoTweenTransitionData,
}
[System.Serializable]
public class TransitionData
{

    [SerializeField]
    public TransitionType transitionType;
    [SerializeField] public Image bar;


    public virtual void PerformTransition()
    {

    }
}

public class HealthUI : MonoBehaviour
{
    public string characterOwnerName = "";
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public Action<string> OnInitializeEvent;
    public Action<int> OnModifyHealthEvent;
    public Action OnHealthDeathEvent;
    public Action OnSaveHealthEvent;
    public IsWithinHealthCondition OnIsWithinHealthConditionEvent;

    [SerializeField] private GameObject frame;
    [SerializeField] private Image realBarUI; //colored
    [SerializeField] private Image ghostBarUI; //white

    private bool isResetting = false;
    private bool isCurrentlyResettingCoroutine = false;
    [SerializeField] private float resetTransitionTime;

    [SerializeField] private Color32 defaultRealBarColor;
    [SerializeField] private InstantColorData realBarColorFlash;
    [SerializeField] private List<ColorTransitionData> realBarColorTransitions = new List<ColorTransitionData>();

    [SerializeField] private Color32 fakeRealBarColor;
    [SerializeField] private Color32 defaultGhostBarColor;
    [SerializeField] private Color32 addGhostBarColor;
    [SerializeField] private Color32 subtractGhostBarColor;

    [SerializeField] private FadeTransitionData ghostBarFadeTransition;
    [SerializeField] private FillTransitionData ghostBarFillTransition;

    [SerializeField] private float delayTime = 0;

    private float current = 0;
    private float currentMax = 1;
    private float savedFill = 0;
    IEnumerator runningCoroutine;
    private void Awake()
    {
        OnInitializeEvent += OnInitialize;
        OnModifyHealthEvent += ModifyHealth;
        OnIsWithinHealthConditionEvent += IsWithinHealthCondition;
       // OnSaveHealthEvent += SaveHealth;
        CharacterDialogueUI.OnInspectingEvent += Open;
        CharacterDialogueUI.OnDeinspectingEvent += Close;
        realBarUI.fillAmount = 0f;
        ghostBarUI.fillAmount = 0f;

        
        InstantUpdateBar(currentHealth, maxHealth, maxHealth);
    }

    private void OnDestroy()
    {
        OnInitializeEvent -= OnInitialize;
     ///   OnSaveHealthEvent -= SaveHealth;
        OnModifyHealthEvent -= ModifyHealth;
        OnIsWithinHealthConditionEvent -= IsWithinHealthCondition;
        CharacterDialogueUI.OnInspectingEvent -= Open;
        CharacterDialogueUI.OnDeinspectingEvent -= Close;
    }
    void OnInitialize(string p_ownerName)
    {
        characterOwnerName = p_ownerName;
        Debug.Log("HEALTH INITIALIZED " + DialogueSpreadSheetPatternConstants.maeveHealth);
        if (!string.IsNullOrEmpty(characterOwnerName))
        {
            if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Maeve")
            {
                currentHealth = DialogueSpreadSheetPatternConstants.maeveHealth;
            }
            else if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Penelope")
            {
                currentHealth = DialogueSpreadSheetPatternConstants.penelopeHealth;
            }
            else if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Brad")
            {
                currentHealth = DialogueSpreadSheetPatternConstants.bradHealth;
            }
            else if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Liam")
            {
                currentHealth = DialogueSpreadSheetPatternConstants.liamHealth;
            }
            InstantUpdateBar(currentHealth, maxHealth, maxHealth);
        }
    }
    void SaveHealth()
    {
        if (DialogueSpreadSheetPatternConstants.cueCharacter != null)
        {
            if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Maeve")
            {
                DialogueSpreadSheetPatternConstants.maeveHealth = currentHealth;
            }
            else if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Penelope")
            {
                DialogueSpreadSheetPatternConstants.penelopeHealth = currentHealth;
            }
            else if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Brad")
            {
                DialogueSpreadSheetPatternConstants.bradHealth = currentHealth;
            }
            else if (DialogueSpreadSheetPatternConstants.cueCharacter.idName == "Liam")
            {
                DialogueSpreadSheetPatternConstants.liamHealth = currentHealth;
            }
        }
    }
    void Open()
    {
        frame.SetActive(false);
    }

    void Close()
    {
        frame.SetActive(true);
    }
    void ModifyHealth(int p_modifier)
    {
        //if (p_modifier >0)
        //{
        //    defaultGhostBarColor = addGhostBarColor;
        //}
        //else
        //{
        //    defaultGhostBarColor = subtractGhostBarColor;
        //}
        currentHealth += p_modifier;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            //Debug.Log("DIE " + currentHealth);
            if (StorylineManager.currentZeroSO_Dialogues != null)
            {
               // Debug.Log("DIE");
                currentHealth = 0;
                OnHealthDeathEvent.Invoke();
            }
            else
            {
               // Debug.Log("NO DIE");
                currentHealth = 1;
       
            }
               

        }
        SaveHealth();
        UpdateBar(currentHealth, maxHealth);
    }

    bool IsWithinHealthCondition(int p_healthCeilingCondition, int p_healthFloorCondition)
    {
        if (currentHealth <= p_healthCeilingCondition &&
            currentHealth >= p_healthFloorCondition)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void InstantUpdateBar(float p_current = 0, float p_currentMax = 1, float p_max = 1)
    {
        ghostBarUI.color = fakeRealBarColor;
        StopAllCoroutines();
        isResetting = false;

        current = p_current;
        currentMax = p_max;
        float fill = p_current / p_max;
        savedFill = fill;

        if (realBarUI)
        {
            realBarUI.DOFillAmount(savedFill, 0.01f);
            realBarUI.color = new Color(realBarUI.color.r, realBarUI.color.g, realBarUI.color.b, 1);
        }
        //else
        //{
        //    Debug.LogError(gameObject.name.ToString() + " IS MISSING primaryBarUI REFERENCE IN INSPECTOR");
        //}

        if (ghostBarUI)
        {
            ghostBarUI.DOFillAmount(savedFill, 0.01f);
            ghostBarUI.color = new Color(ghostBarUI.color.r, ghostBarUI.color.g, ghostBarUI.color.b, 1);
        }
        //else
        //{
        //    Debug.LogError(gameObject.name.ToString() + " IS MISSING ghostBarUI REFERENCE IN INSPECTOR");
        //}
 
    }


    public void UpdateBar(float p_current = 0, float p_currentMax = 1)
    {
    
        current = p_current;
        currentMax = p_currentMax;

        float fill = current / currentMax;

        if (enabled)
        {
            if (runningCoroutine != null)
            {
                StopCoroutine(runningCoroutine);
                runningCoroutine = null;
            }
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(Co_UpdateBar(fill));
        }
    }
    IEnumerator Co_UpdateBar(float p_fill = 0)
    {
        bool add = false;
        Image currentBar;
        Image currentGhostBar;
        if (savedFill > p_fill) //Decreased, go real bar go first
        {
            if (realBarColorFlash != null)
            {
     
                realBarUI.color = realBarColorFlash.color;
            }
            yield return new WaitForSeconds(0.1f);
            if (realBarColorFlash != null)
            {
                realBarUI.color = defaultRealBarColor;
            }
            yield return new WaitForSeconds(0.1f);
            if (realBarColorFlash != null)
            {
                realBarUI.color = realBarColorFlash.color;
            }
            yield return new WaitForSeconds(0.1f);
            //for (int i = 0; i < realBarColorTransitions.Count; i++)
            //{
            //    Tween colorTransition = realBarUI.DOColor(realBarColorTransitions[i].amount, 0.05f);
            //    yield return colorTransition.WaitForCompletion();
            //}
            yield return new WaitForSeconds(0.1f);
            if (realBarColorFlash != null)
            {
                realBarUI.color = defaultRealBarColor;
            }
            //realBarTransform.sortingOrder = 5;
            //ghostBarTransform.sortingOrder = 4;
            currentBar = realBarUI;
            currentGhostBar = ghostBarUI;


           
            defaultGhostBarColor = subtractGhostBarColor;
            ghostBarUI.color = defaultGhostBarColor;


        }
        else //Increase, fake bar go first
        {
            currentBar = ghostBarUI;
            currentGhostBar = realBarUI;
            defaultGhostBarColor = addGhostBarColor;
            add = true;

            //realBarTransform.sortingOrder = 4;
            //ghostBarTransform.sortingOrder = 5;

        }
        currentBar.fillAmount = p_fill;


        yield return new WaitForSeconds(delayTime);

        Sequence s = DOTween.Sequence();
        if (ghostBarFadeTransition != null)
        {
            float secondaryBarFadeAmount = 0;
            if (ghostBarFadeTransition.amount < 0)
            {
                secondaryBarFadeAmount = p_fill;
            }
            else if (ghostBarFadeTransition.amount > 0)
            {
                secondaryBarFadeAmount = ghostBarFillTransition.amount;
            }

            if (ghostBarFadeTransition.amount < 900)
            {
                if (!add)
                {
                    s.Join(currentGhostBar.DOFade(secondaryBarFadeAmount, ghostBarFadeTransition.transitionTime));
                }
           
            }

        }
        if (ghostBarFillTransition != null)
        {
            if (ghostBarFillTransition.amount <= 0)
            {
                savedFill = p_fill;


            }
            else if (ghostBarFillTransition.amount > 0)
            {
                if (ghostBarFillTransition.amount < -1)
                {
                    savedFill = 0;
                }
                else
                {
                    savedFill = ghostBarFillTransition.amount;
                }

            }
            if (p_fill < -1)
            {

                p_fill = 0;

            }
            s.Join(currentGhostBar.DOFillAmount(p_fill, ghostBarFillTransition.transitionTime));

        }

        s.Play();
        yield return s.WaitForCompletion();
        //realBarTransform.sortingOrder = 5;
        //ghostBarTransform.sortingOrder = 4;
       
      
    
        //if (!add)
        //{
            ghostBarUI.color = fakeRealBarColor;
            realBarUI.color = addGhostBarColor;
        //}
        //else
        //{
        //    realBarUI.color = fakeRealBarColor;
        //    ghostBarUI.color = addGhostBarColor;
        //}
    //    realBarTransform.sortingOrder = 5;
     //   ghostBarTransform.sortingOrder = 4;
    }
}
