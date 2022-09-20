using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFindRUI : MonoBehaviour
{
    [SerializeField] List<GameObject> Panels;

    public void SwitchPanels(GameObject selectedPanel)
    {
        foreach(GameObject panel in Panels)
        {
            if (selectedPanel == panel)
                panel.SetActive(true);
            else
                panel.SetActive(false);
        }
    }
}
