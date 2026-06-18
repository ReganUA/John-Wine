using UnityEngine;

public class InventoryContainer : MonoBehaviour
{
    public static InventoryContainer instance;
    void Awake()
    {
        instance = this;
    }
}
