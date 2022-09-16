using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SoundCategoryData 
{

    public string name;
    [NonReorderable] public List<SoundData> sound;
}
