using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeaponAdder : Controller, IUpdatable
{
    public List<AbilitySO> WeaponToAdd;

    void Start()
    {
        _unit.OnSpawn(null);
    }

    public override void OnStart()
    {
        Registerer.RegisterUpdatable(this);
    }

    private void AddWeapon(Unit target)
    {
        List<Ability> newWeaponSet = new List<Ability>();

        // Створюємо абілки з префабу на основі статів гравця
        for (int i = 0; i < _unit.UnitSO.SimComponents.Abilities.Count; i++)
        {
            Ability newAbility = _unit.UnitSO.SimComponents.Abilities[i].CreateAbility(target.Stats);
            newWeaponSet.Add(newAbility);
        }

        // Захист від непарності: якщо підібрана зброя одинарна, 
        // додаємо порожнє місце (null), щоб закрити ЛКМ/ПКМ пару
        if (newWeaponSet.Count == 1)
        {
            newWeaponSet.Add(null);
        }

        // Рятуємо новий ізольований сет в інвентар
        InventoryManager.instance.SaveWeapon(newWeaponSet);
        Debug.Log("Saved clean new isolated weapon set to InventoryManager!");

        _unit.Die();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.player != null)
        {
            AddWeapon(GameManager.instance.player);
        }
    }

    public void OnUpdate(float dt)
    {
        if (GameManager.instance.player != null)
        {
            _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, GameManager.instance.player.transform.position, Time.deltaTime);
        }
    }

    public override void OnDeath()
    {
        Registerer.UnregisterUpdatable(this);
    }
}