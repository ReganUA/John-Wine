using UnityEngine;

public class InventoryCointainer : MonoBehaviour
{
    public static InventoryCointainer instance;
    void Start()
    {
        instance = this;
    }
}
