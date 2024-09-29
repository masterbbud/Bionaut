using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "ScriptableObjects/PlanetData", order = 1)]
public class PlanetData : ScriptableObject
{
    public string planetName;
    public string description;
    public List<CritterData> critters;
    public string sceneName;
}
