using UnityEngine;
using System.Collections.Generic;
public sealed class WeaponAdder : Controller
{
    public List<AbilitySO> Weapon;
    public override void OnStart()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
        {
            if (unit.gameObject == GameManager.instance.player)
            {
                AddWeaponToInventory(unit);
                Destroy(gameObject);
            }
        }
    }
    private void AddWeaponToInventory(Unit target)
    {
        List<Ability> createdAbilities = new List<Ability>();
        for (int i = 0; i < Weapon.Count; i++)
        {
            Ability ability = Weapon[i].CreateAbility(target.Stats);
            createdAbilities.Add(ability);
        }
        InventoryManager.instance.SaveWeapon(createdAbilities);
    }
}