using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    
    public List<CritterData> collectedCritters = new();
    public Dictionary<ItemData, int> collectedItems = new();
    // Start is called before the first frame update
    void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
        Player.inventory = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GiveObject(ItemData obj, int count)
    {
        if (collectedItems.ContainsKey(obj)) {
            collectedItems[obj] += count;
        }
        else {
            collectedItems.Add(obj, count);
        }
    }

    public void AddCritter(CritterData critter)
    {
        collectedCritters.Add(critter);
    }

    public bool HasTool(ItemData toolData)
    {
        if (toolData == null) { // nothing equipped
            return true;
        }
        return collectedItems.ContainsKey(toolData);
    }
}
