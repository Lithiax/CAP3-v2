using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public SO_Character so_Character;
    public RectTransform avatarRectTransform;

    public Image avatarImage;

    private void Awake()
    {
        avatarRectTransform = avatarRectTransform ? avatarRectTransform : GetComponent<RectTransform>();
        avatarImage = avatarImage ? avatarImage : GetComponent<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
