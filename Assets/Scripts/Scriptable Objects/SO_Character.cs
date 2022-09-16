using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Character Scriptable Object", menuName = "Scriptable Objects/Character")]
public class SO_Character : ScriptableObject
{
    public new string name;
    public List<Sprite> avatars = new List<Sprite>();
    public Sprite avatar;
}
