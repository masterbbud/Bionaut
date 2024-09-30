using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public ItemData itemData;
    void Start() 
    {

    }

    public abstract void Use();
}
