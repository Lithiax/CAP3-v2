using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainFindRUI : MonoBehaviour
{
    [SerializeField] List<GameObject> Panels;
    [SerializeField] List<Button> Buttons;
    [SerializeField] Color buttonActivatedColor;
    [SerializeField] Color oldButtonColor;

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

    public void Start()
    {
        foreach (Button b in Buttons)
        {
            b.onClick.AddListener(() => ActivateButton(b));
        }
    }

    void ActivateButton(Button but)
    {
        foreach (Button b in Buttons)
        {
            if (b == but)
            {
                b.image.color = buttonActivatedColor;
                continue;
            }

            b.image.color = oldButtonColor;
        }
    }
}
