using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSys : Controller, IUpdatable
{
    public GameObject imagePrefab;
    public Transform itemDisplay;
    public override void OnStart()
    {
    
    }
    public void OnUpdate(float dt)
    {

    }
    public bool GetKey(InteractiveBaseEnviroment target)
    {
        if (target == null) return false;
        if (InventoryManager.instance.heldItems == null || InventoryManager.instance.heldItems.Count == 0) return false;

        for (int i = 0; i < InventoryManager.instance.heldItems.Count; i++)
        {
            if (InventoryManager.instance.heldItems[i] != null && InventoryManager.instance.heldItems[i].TryGetComponent(out Unit unit) && target.CheckKey(unit.UnitSO))
            {
                InventoryManager.instance.heldItems.RemoveAt(i);
                Destroy(itemDisplay.GetChild(0).gameObject);
                return true;
            }
        }
        return false;
    }
    public void AddItem(GameObject item)
    {
        InventoryManager.instance.heldItems.Add(item);
        item.SetActive(false);

        ItemDisplay();
    }
    private void ItemDisplay()
    {
        Instantiate(imagePrefab, itemDisplay);
    }
}
