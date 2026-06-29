using System.Collections;
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
    void Start()
    {
        if (InventoryCointainer.instance != null)
        {
            itemDisplay = InventoryCointainer.instance.gameObject.transform;
        } else
        {
            StartCoroutine(LoadInterfaceLater());
        }
        StartCoroutine(LoadInterfaceLater2());
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
                InventoryManager.instance.itemsDisplayed.RemoveAt(i);
                Destroy(itemDisplay.GetChild(0).gameObject);
                return true;
            }
        }
        return false;
    }
    public void AddItem(GameObject item)
    {
        InventoryManager.instance.heldItems.Add(item);
        DontDestroyOnLoad(item);
        item.SetActive(false);

        ItemDisplay();
    }
    private void ItemDisplay()
    {
        GameObject ui = Instantiate(imagePrefab, itemDisplay);
        InventoryManager.instance.itemsDisplayed.Add(ui);
    }
    private IEnumerator LoadInterfaceLater()
    {
        yield return new WaitForEndOfFrame();
        itemDisplay = InventoryCointainer.instance.gameObject.transform;
    }
    private IEnumerator LoadInterfaceLater2()
    {
        yield return new WaitForEndOfFrame();

        if (InventoryManager.instance.itemsDisplayed.Count > 0)
        {
            if (itemDisplay.childCount == 0)
            {
                foreach (GameObject item in InventoryManager.instance.itemsDisplayed)
                {
                    Instantiate(imagePrefab, itemDisplay);
                }
            }
        }
    }
}
