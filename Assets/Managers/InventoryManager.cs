using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<GameObject> heldItems = new();
    public List<GameObject> itemsDisplayed = new();

    public List<List<AbilitySO>> SavedWeapons = new();
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SaveWeapon(List<AbilitySO> weapon)
    {
        SavedWeapons.Add(weapon);
    }
    public void GetWeapon(Unit target, ComponentRuntimeStats statsCarrier, int idx)
    {
        target.Abilities.Clear();
        List<Ability> createdAbilities = new List<Ability>();
        for (int i = 0; i < createdAbilities.Count; i++)
        {
            createdAbilities[i] = SavedWeapons[idx][i].CreateAbility(statsCarrier);
        }
        target.Abilities = createdAbilities;
    }
}
