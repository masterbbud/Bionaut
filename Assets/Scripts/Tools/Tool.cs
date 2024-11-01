using UnityEngine;

/*
 * General class for a Tool which can be equipped and used.
 */
public abstract class Tool : MonoBehaviour
{
    public ItemData itemData;
    void Start() 
    {

    }

    // Called when the player left clicks with this tool equipped
    public abstract void Use();
}
