using System.Collections;
using UnityEngine;

public sealed class WeaponAdder : Controller
{
    Unit playerTarget;
    public override void OnStart()
    {
        LaterLoad();
    }
    private void AddWeapon(Unit target)
    {
        for (int i = 0; i < _unit.UnitSO.SimComponents.Abilities.Count; i++)
        {
            target.AddAbility(_unit.UnitSO.SimComponents.Abilities[i].CreateAbility(target.Stats));
            Debug.Log("AddedWeapon");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_unit.UnitSO.SimComponents.Sensor.IsDetectionViable(_unit.Stats, playerTarget, _unit))
            AddWeapon(playerTarget);
    }
    void Update()
    {
        if (playerTarget != null)
            _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, playerTarget.transform.position, Time.deltaTime);
    }
    private IEnumerator LaterLoad()
    {
        yield return new WaitForEndOfFrame();
        playerTarget = GameManager.instance.player;
    }
}