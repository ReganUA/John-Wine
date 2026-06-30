using System.Collections;
using UnityEngine;

public sealed class WeaponAdder : Controller, IUpdatable
{
    public Unit playerTarget;
    public override void OnStart()
    {
        Registerer.RegisterUpdatable(this);
    }
    void Start()
    {
        StartCoroutine(LaterLoad());
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
            AddWeapon(playerTarget);
    }
    public void OnUpdate(float dt)
    {
        if (playerTarget != null)
        {
            _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, playerTarget.transform.position, Time.deltaTime);
        }
    }
    private IEnumerator LaterLoad()
    {
        yield return new WaitForEndOfFrame();
        playerTarget = GameManager.instance.player;
        Debug.Log(playerTarget);
    }
    public override void OnDeath()
    {
        Registerer.UnregisterUpdatable(this);
    }
}