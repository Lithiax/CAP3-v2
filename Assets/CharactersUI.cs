using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
public class CharactersUI : MonoBehaviour
{
    [SerializeField] GameObject frame;
    public static Action<List<SO_Character>> onAddCharactersEvent;// = new onLoadAvatarsEvent
    public static Action<List<SO_Character>> onRemoveCharactersEvent;// = new onLoadAvatarsEvent
    public static Action<List<CharacterData>> onUpdateCharacterDatasEvent;// = new onLoadAvatarsEvent
    [SerializeField] private Transform characterUIContainerTransform;
    [SerializeField] private Transform live2DCollisionUIContainerTransform;
    [SerializeField] private Transform characterObjectContainerTransform;


    [SerializeField]
    private List<CharacterPresetData> characterPresetDatas = new List<CharacterPresetData>();

    [SerializeField] private CharacterUI staticCharacterPrefab;

    [HeaderAttribute("ADJUSTABLE VALUES")]

    [SerializeField] private Color32 nonSpeakerTintColor;


    [SerializeField]
    private float avatarFadeTime;
    [SerializeField]
    private float avatarDelayTime;

    List<CharacterData> characterDatas;
    private void Awake()
    {
        onRemoveCharactersEvent += RemoveAvatar;
        onAddCharactersEvent += AddAvatar;
        onUpdateCharacterDatasEvent += UpdateCharacterDatas;
        CharacterDialogueUI.OnIsSkipping += Skip;
    }
    //    CharacterDialogueUI.OnInspectingEvent += Toggle;
    //    CharacterDialogueUI.OnDeinspectingEvent += Toggle;
    //}
    //void Toggle()
    //{
    //    frame.SetActive(!frame.activeSelf);
    //}
    void Skip()
    {
        for (int i = 0; i < characterDatas.Count; i++)
        {
            ////Remove
            //CharacterDialogueUI.savedCharacters.Remove(foundPreset);
            //Destroy(foundPreset.gameObject);

            ////add avatar
            //for (int x = 0; x < CharacterDialogueUI.savedCharacters.Count; x++)
            //{
            //    if (CharacterDialogueUI.savedCharacters[x] is CharacterUI)
            //    {
            //        if (CharacterDialogueUI.savedCharacters[x].so_Character.avatar != null)
            //        {
            //            CharacterUI currentCharacterUI = CharacterDialogueUI.savedCharacters[x] as CharacterUI;
            //            currentCharacterUI.avatarImage.color = new Color(1, 1, 1, 1);
            //        }
            //    }

            //}
            Character foundCharacter = FindPreset(characterDatas[i].character);

            //Tint
            if (foundCharacter is CharacterUI)
            {
                CharacterUI foundPreset = foundCharacter as CharacterUI;
               
                if (characterDatas[i].isSpeaking)
                {


                    //Add reference coroutine so when player skips it can be referenced and stopped
                    foundPreset.avatarImage.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    foundPreset.avatarImage.color = nonSpeakerTintColor;

                }
            }
          
        }

    }
    Character FindPreset(SO_Character p_so_Character)
    {

        for (int x = 0; x < CharacterDialogueUI.savedCharacters.Count; x++)
        {
            if (CharacterDialogueUI.savedCharacters[x].so_Character == p_so_Character)
            {
                return CharacterDialogueUI.savedCharacters[x];

            }

        }
        return null;
    }

    void UpdateCharacterDatas(List<CharacterData> p_characterDatas)
    {
        characterDatas = p_characterDatas;
        for (int i = 0; i < p_characterDatas.Count; i++)
        {
            Character foundCharacter = FindPreset(p_characterDatas[i].character);
            //Set Character UI Rect Transform
            //Position
            SetRectTransformToPreset(p_characterDatas[i].characterPosition, foundCharacter);
            SetAvatarFlipOrientation(p_characterDatas[i], foundCharacter);
            SetFacialEmotion(p_characterDatas[i], foundCharacter);
            SetBodyEmotion(p_characterDatas[i], foundCharacter);
            SetSpeakerTint(p_characterDatas[i].isSpeaking, foundCharacter);
        }
    }
    IEnumerator AvatarFadeIn(Image p_avatarImage, Sprite p_sprite)
    {
        //CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        p_avatarImage.sprite = p_sprite;
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
        //CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }

    IEnumerator AvatarFadeOut(Image p_avatarImage, Character p_newCharacter)
    {
        //CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
       .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        //CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
        CharacterDialogueUI.savedCharacters.Remove(p_newCharacter);

        if (p_newCharacter != null)
        {
            if (p_newCharacter.gameObject != null)
            {
                Destroy(p_newCharacter.gameObject);
            }
        }
     
           
        
    
    }

    IEnumerator SpeakerTintIn(Image p_avatarImage)
    {
        CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(Color.white, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();
        CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }

    IEnumerator SpeakerTintOut(Image p_avatarImage)
    {
        CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOColor(nonSpeakerTintColor, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
        CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }

    void SetRectTransformToPreset(CharacterPositionType p_characterPositionType, Character foundCharacter)
    {

        if (foundCharacter != null)
        {

            for (int i = 0; i < characterPresetDatas.Count; i++)
            {
                if (p_characterPositionType == characterPresetDatas[i].characterPositionType)
                {
                    if (foundCharacter is CharacterUI)
                    {
                        CharacterUI foundPreset = foundCharacter as CharacterUI;
                        foundPreset.avatarRectTransform.anchoredPosition = characterPresetDatas[i].avatarRectTransform.anchoredPosition;
                    }
                    else if (foundCharacter is CharacterObject)
                    {
                        CharacterObject foundPreset = foundCharacter as CharacterObject;
                        foundPreset.transform.position = characterPresetDatas[i].avatarTransform.position;
                    }
                }
            }
        }
    }

    void SetSpeakerTint(bool p_isSpeaking, Character foundCharacter)
    {

        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterUI)
            {
                CharacterUI foundPreset = foundCharacter as CharacterUI;
                if (CharacterDialogueUI.isSkipping)
                {
                    //Tint
                    if (p_isSpeaking)
                    {
                        //Add reference coroutine so when player skips it can be referenced and stopped
                        foundPreset.avatarImage.color = new Color(1, 1, 1, 1);
                    }
                    else
                    {
                        foundPreset.avatarImage.color = nonSpeakerTintColor;

                    }
                }
                else
                {
                    //Tint
                    if (p_isSpeaking)
                    {
                        //Add reference coroutine so when player skips it can be referenced and stopped
                        StartCoroutine(SpeakerTintIn(foundPreset.avatarImage));

                    }
                    else
                    {
                        StartCoroutine(SpeakerTintOut(foundPreset.avatarImage));

                    }
                }

            }
        }

    }

    void SetFacialEmotion(CharacterData p_characterData, Character foundCharacter)
    {
        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterObject)
            {
                if (p_characterData.faceEmotion != CharacterEmotionType.none)
                {
                    CharacterObject foundPreset = foundCharacter as CharacterObject;

                    for (int i = 0; i < foundPreset.so_Character.faceEmotionDatas.Count; i++)
                    {

                        if (foundPreset.so_Character.faceEmotionDatas[i].type == p_characterData.faceEmotion)
                        {
                            foundPreset.expressionController.CurrentExpressionIndex = foundPreset.so_Character.faceEmotionDatas[i].index;
                            break;
                        }
                    }
                }
            }
        }
    }

    void SetBodyEmotion(CharacterData p_characterData, Character foundCharacter)
    {
        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterObject)
            {

                if (p_characterData.bodyEmotion != CharacterEmotionType.none)
                {
                    CharacterObject foundPreset = foundCharacter as CharacterObject;
                    for (int i = 0; i < foundPreset.charAnim.parameters.Length;)
                    {
                        if (foundPreset.charAnim.parameters[i].name == p_characterData.bodyEmotion.ToString())
                        {
                            foundPreset.charAnim.SetTrigger(p_characterData.bodyEmotion.ToString());
                            break;
                        }
                        i++;
                        if (i >= foundPreset.charAnim.parameters.Length)
                        {
                            Debug.Log(StorylineManager.currentDialogueIndex + " Set Body Emotion is Not available");
                        }
                    }

                }

            }
        }
    }

    void RemoveAvatar(List<SO_Character> p_charactersToBeRemoved)
    {
        //Debug.Log("-----REMOVING " + isSkipping);
        //Do functions to characters to be Added
        for (int i = 0; i < p_charactersToBeRemoved.Count; i++)
        {
            Character foundCharacter = FindPreset(p_charactersToBeRemoved[i]);
            if (foundCharacter != null)
            {
                if (foundCharacter is CharacterUI)
                {
                    CharacterUI foundPreset = foundCharacter as CharacterUI;
                    if (p_charactersToBeRemoved[i].avatar != null)
                    {
                        //if (CharacterDialogueUI.isSkipping)
                        //{

                        //    // Debug.Log("REMOVINGRR " + foundPreset);
                        //    CharacterDialogueUI.savedCharacters.Remove(foundPreset);
                        //    Destroy(foundPreset.gameObject);
                        //}
                        //else
                        //{
                            StartCoroutine(AvatarFadeOut(foundPreset.avatarImage, foundPreset));
                        //}
                    }

                }
                else if (foundCharacter is CharacterObject)
                {

                    CharacterObject foundPreset = foundCharacter as CharacterObject;
                    CharacterDialogueUI.savedCharacters.Remove(foundPreset);
                    //LIVE 2D
                    //Check if its the character thats talking
                    if (StorylineManager.currentSO_Dialogues.cueBankData.isEnabled)
                    {
                        if (p_charactersToBeRemoved[i] == StorylineManager.cueCharacter)
                        {
                            for (int x = 0; x < live2DCollisionUIContainerTransform.childCount; x++)
                            {
                                Debug.Log("DELETING RAD");
                                Destroy(live2DCollisionUIContainerTransform.GetChild(i).gameObject);
                            }
                        }
                    }
                    Destroy(foundPreset.gameObject);
                }

            }
        }

    }



    void AddAvatar(List<SO_Character> p_charactersToBeAdded)
    {
        //Do functions to characters to be Added
        //Debug.Log("-----ADDING " + isSkipping);
        //if (CharacterDialogueUI.isSkipping)
        //{
        //    for (int i = 0; i < CharacterDialogueUI.savedCharacters.Count; i++)
        //    {
        //        if (CharacterDialogueUI.savedCharacters[i] is CharacterUI)
        //        {
        //            if (CharacterDialogueUI.savedCharacters[i].so_Character.avatar != null)
        //            {
        //                CharacterUI currentCharacterUI = CharacterDialogueUI.savedCharacters[i] as CharacterUI;
        //                currentCharacterUI.avatarImage.color = new Color(1, 1, 1, 1);
        //            }
        //        }

        //    }

        //}
        //else
        //{
        for (int i = 0; i < p_charactersToBeAdded.Count; i++)
        {
            Character newCharacter = null;
            // Debug.Log("PRINT NAME: " + p_charactersToBeAdded[i].name);

            if (p_charactersToBeAdded[i].prefab != null) //Live 2D
            {

                newCharacter = Instantiate(p_charactersToBeAdded[i].prefab, characterObjectContainerTransform);
                //Check if its the character thats talking
                if (StorylineManager.currentSO_Dialogues.cueBankData.isEnabled)
                {
                    if (p_charactersToBeAdded[i] == StorylineManager.cueCharacter)
                    {
                        if (StorylineManager.cueCharacter.collisionPrefab != null)
                        {
                            GameObject newCharacterCollision = Instantiate(p_charactersToBeAdded[i].collisionPrefab, live2DCollisionUIContainerTransform);
                            Debug.Log("COLLISION SHUD BE LOADING");
                        }
                    }
                }
                    
                           

               


            }
            else //UI
            {
                if (p_charactersToBeAdded[i].avatar != null)
                {
                    newCharacter = Instantiate(staticCharacterPrefab, characterUIContainerTransform);
                    CharacterUI newCharacterUI = newCharacter as CharacterUI;
                    newCharacter.so_Character = p_charactersToBeAdded[i];

                    StartCoroutine(AvatarFadeIn(newCharacterUI.avatarImage, p_charactersToBeAdded[i].avatar));
                }


            }
            if (newCharacter != null)
            {
                CharacterDialogueUI.savedCharacters.Add(newCharacter);
            }

        }
        //}
    }

    public IEnumerator AvatarFlipSequence(Image p_avatarImage, RectTransform p_avatarRectTransform, Quaternion p_quaternion)
    {
        CharacterDialogueUI.OnAddNewTransitionEvent.Invoke();
        var fadeOutSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(0, avatarFadeTime));
        fadeOutSequence.Play();
        yield return fadeOutSequence.WaitForCompletion();

        p_avatarRectTransform.rotation = p_quaternion;

        var fadeInSequence = DOTween.Sequence()
        .Append(p_avatarImage.DOFade(1, avatarFadeTime));
        fadeInSequence.Play();
        yield return fadeInSequence.WaitForCompletion();
        CharacterDialogueUI.OnFinishTransitionEvent.Invoke();
    }


    void SetAvatarFlipOrientation(CharacterData p_characterData, Character foundCharacter) // work on this
    {

        if (foundCharacter != null)
        {
            if (foundCharacter is CharacterUI)
            {
                CharacterUI foundPreset = foundCharacter as CharacterUI;
                Quaternion target;

                if (p_characterData.isFlipped)
                {
                    target = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    target = Quaternion.Euler(0f, 0f, 0f);
                }
                if (foundPreset.avatarRectTransform.rotation != target)
                {
                    StartCoroutine(AvatarFlipSequence(foundPreset.avatarImage, foundPreset.avatarRectTransform, target));
                }

            }


        }


    }


    void IdentifyCharactersToAdd(List<SO_Character> newList, List<SO_Character> oldList, List<SO_Character> charactersToBeAdded)
    {

        //Mark the Characters to Add and Characters that Exists
        for (int i = 0; i < newList.Count; i++)
        {
            if (oldList.Count > 0)
            {
                for (int x = 0; x < oldList.Count;)
                {
                    if (newList[i] == oldList[x])//matching
                    {
                        break;
                    }
                    x++;
                    if (x >= oldList.Count)//new
                    {
                        charactersToBeAdded.Add(newList[i]);
                    }
                }
            }
            else
            {
                charactersToBeAdded.Add(newList[i]);
            }

        }

    }
    void IdentifyCharactersToRemove(List<SO_Character> oldList, List<SO_Character> newList, List<SO_Character> charactersToBeRemoved)
    {
        //Mark the Characters to Remove
        for (int oldIndex = 0; oldIndex < oldList.Count; oldIndex++)
        {
            if (newList.Count > 0)
            {
                for (int currentIndex = 0; currentIndex < newList.Count;)
                {
                    if (oldList[oldIndex] == newList[currentIndex]) //If Old Element in Old List Matches New Element in New List, it means it stays
                    {
                        break;
                    }

                    currentIndex++;
                    if (currentIndex >= newList.Count)
                    {
                        if (charactersToBeRemoved.Count > 0)
                        {
                            for (int removedIndex = 0; removedIndex < charactersToBeRemoved.Count;)
                            {
                                if (charactersToBeRemoved[removedIndex] == oldList[oldIndex])
                                {
                                    //It already is marked to be removed
                                    break;
                                }
                                removedIndex++;
                                if (currentIndex >= charactersToBeRemoved.Count)
                                {
                                    //It has not been already marked to be removed
                                    charactersToBeRemoved.Add(oldList[oldIndex]);
                                }


                            }
                        }
                        else
                        {
                            charactersToBeRemoved.Add(oldList[oldIndex]);
                        }



                    }
                }

            }
            else
            {
                charactersToBeRemoved.Add(oldList[oldIndex]);
            }


        }

    }
}