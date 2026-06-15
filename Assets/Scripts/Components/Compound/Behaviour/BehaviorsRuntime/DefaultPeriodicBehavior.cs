using UnityEngine;

public class DefaultPeriodicBehavior : PeriodicBehavior
{
    public override void OnPeriod()
    {
        if (PeriodicComponents.Effect != null)
            PeriodicComponents.Effect.Affect(target, statsCarrier);

        if (PeriodicComponents.AreaSearcher != null)
            PeriodicComponents.AreaSearcher.Search(statsCarrier, new PositionArgs(target.transform.position, target.transform.rotation), target.Owner);
    }
    public DefaultPeriodicBehavior(PeriodicBehaviorSO config, Unit targetUnit, ComponentsPack periodicComponents) : base(config, targetUnit, periodicComponents) { }
}
