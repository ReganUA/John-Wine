using UnityEngine;

public sealed class RocketController : Controller, IUpdatable, IAbilityConfigCarrier
{
    public AbilitySO abilitySO { get; set; }
    public override void OnStart()
    {
        _unit.Stats.SetComponentsStats(abilitySO.ImpactComponents);
        _unit.OnHealthIsZero += _unit.Die;
        Registerer.RegisterUpdatable(this);
    }
    public void OnUpdate(float dt)
    {
        _unit.OnUpdate(dt);

        if (this == null) return;
        _unit.UnitSO.SimComponents.Movers.Mover.Move(_unit, transform.forward, dt);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            OnHit(null);
            _unit.Die();
            return;
        }
        if (other.TryGetComponent(out Unit u))
        {
            if (_unit.UnitSO.SimComponents.Sensor.IsDetectionViable(_unit.Stats, u, _unit) == false) return;

            OnHit(u);
            _unit.Die();
        }
    }
    public void OnHit(Unit hitUnit)
    {
        abilitySO.OnHit(_unit.Stats, new PositionArgs(transform.position, transform.rotation, transform.forward), _unit, hitUnit);
    }
    public override void OnDeath()
    {
        OnHit(null);
        Registerer.UnregisterUpdatable(this);
    }
}
