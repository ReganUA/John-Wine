using UnityEngine;

public sealed class WineBarrelController : Controller, IUpdatable, ISpawned
{
    public AbilitySO abilitySO { get; set; }
    private bool _hasHit;
    public override void OnStart()
    {
        _unit.Stats.SetComponentsStats(abilitySO.ImpactComponents);
        _unit.OnHealthIsZero += _unit.Die;
        Registerer.RegisterUpdatable(this);
    }
    public void OnUpdate(float dt) 
    {
        if (this == null) return;

        Vector3 moveDir = transform.forward;

        _unit.UnitSO.SimComponents.Movers.Mover.Move(_unit, moveDir ,dt);
    }
    public void Hit(Unit hitUnit)
    {
        abilitySO.OnHit(_unit.Stats, new PositionArgs(transform.position, transform.rotation, transform.forward), _unit, hitUnit);
        _hasHit = true;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            Hit(null);
            _unit.Die();
            return;
        }

        if (collision.gameObject.TryGetComponent(out Unit u))
        {
            if (_unit.UnitSO.SimComponents.Sensor.IsDetectionViable(_unit.Stats, u, _unit) == false) return;

            Hit(u);
            _unit.Die();
        }
    }

    public override void OnDeath()
    {
        _unit.OnHealthIsZero -= _unit.Die;
        if (_hasHit == false)
        {
            Hit(null);
        }
        Registerer.UnregisterUpdatable(this);
    }
}
