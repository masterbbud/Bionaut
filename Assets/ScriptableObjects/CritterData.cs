using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CritterData", menuName = "ScriptableObjects/CritterData", order = 1)]
public class CritterData : ScriptableObject
{
    public string critterName;
    public string description;
    public List<Sprite> sprites;
    public Color planetColor;
    public GameObject prefab;
}
