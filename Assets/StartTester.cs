using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTester : MonoBehaviour
{
    private void Awake()
    {
        StorylineManager.currentSO_Dialogues = Resources.Load<SO_Dialogues>("Scriptable Objects/Dialogues/Visual Novel/" + "Maeve1" + "/" + "Week1");
        
    }

  
}
