using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script containing the player's persistent inventory, such as
 * critters or items that you've collected
 */
public class PlayerInventory : MonoBehaviour
{
    
    public List<CritterData> collectedCritters = new();
    public Dictionary<ItemData, int> collectedItems = new();
    // Start is called before the first frame update
    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Gives a number `count` of the given object
    public void GiveObject(ItemData obj, int count)
    {
        if (collectedItems.ContainsKey(obj)) {
            collectedItems[obj] += count;
        }
        else {
            collectedItems.Add(obj, count);
        }
    }

    // Adds the given CritterData to the collected list
    public void AddCritter(CritterData critter)
    {
        if (! collectedCritters.Contains(critter)) {
            collectedCritters.Add(critter);
        }
    }

    // Checks if they player has any number (including 0) of the given tool
    public bool HasTool(ItemData toolData)
    {
        if (toolData == null) { // nothing equipped
            return true;
        }
        return collectedItems.ContainsKey(toolData);
    }
}
