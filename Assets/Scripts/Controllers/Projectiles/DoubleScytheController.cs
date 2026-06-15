using UnityEngine;

public class DoubleScytheController : Controller, IUpdatable, IAbilityConfigCarrier
{
    public AbilitySO abilitySO { get; set; }

    public override void OnStart()
    {
        _unit.Stats.SetComponentsStats(abilitySO.ImpactComponents);

        Registerer.RegisterUpdatable(this);
    }

    public void OnUpdate(float dt)
    {
        _unit.OnUpdate(dt);
    }
    public override void OnDeath()
    {
        Registerer.UnregisterUpdatable(this);
    }
}
