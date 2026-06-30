using System.Collections;
using UnityEngine;

public sealed class WeaponAdder : Controller, IUpdatable
{
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
        for (int i = 0; i < _unit.UnitSO.SimComponents.Abilities.Count; i++)
        {
            target.AddAbility(_unit.UnitSO.SimComponents.Abilities[i].CreateAbility(target.Stats));
            Debug.Log("AddedWeapon");
            _unit.Die();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            AddWeapon(GameManager.instance.player);
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