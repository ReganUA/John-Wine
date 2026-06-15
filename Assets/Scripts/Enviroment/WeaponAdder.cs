using UnityEngine;

public sealed class WeaponAdder : Controller
{
    public override void OnStart()
    {

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
        if (other.TryGetComponent(out Unit unit))
        {
            if (_unit.UnitSO.SimComponents.Sensor.IsDetectionViable(_unit.Stats, unit, _unit))
                AddWeapon(unit);
        }
    }
}