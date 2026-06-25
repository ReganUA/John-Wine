using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<GameObject> heldItems = new();
    public List<GameObject> itemsDisplayed = new();

    public List<List<Ability>> SavedWeapons = new();
    private int _currentlySelected = -1;
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
    public void SaveWeapon(List<Ability> weapon)
    {
        SavedWeapons.Add(weapon);
    }
    public void SetWeapon(Unit target, ComponentRuntimeStats statsCarrier, int idx)
    {
        if (idx >= SavedWeapons.Count || SavedWeapons[idx] == null || idx == _currentlySelected) return;

        target.State.CurrentAbility?.Release();
        target.Abilities.Clear();
        target.State.CurrentAbility = null;
        List<Ability> abilities = new List<Ability>();
        for (int i = 0; i < SavedWeapons[idx].Count; i++)
        {
            Ability ability = SavedWeapons[idx][i];

            ability.ResetReloadProgress();
            ability.Release();

            abilities.Add(ability);
        }
        _currentlySelected = idx;
        target.Abilities = abilities;
    }
}
