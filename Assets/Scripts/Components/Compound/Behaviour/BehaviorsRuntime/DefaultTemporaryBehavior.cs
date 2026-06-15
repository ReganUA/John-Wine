using UnityEngine;

public sealed class DefaultTemporaryBehavior : TemporaryBehavior
{
    public sealed override void OnStart()
    {
        if (StartComponents.Effect != null)
            StartComponents.Effect.Affect(target, statsCarrier);
    }
    public sealed override void OnEnd()
    {
        if (EndComponents.Effect != null)
            EndComponents.Effect.Affect(target, statsCarrier);
    }
    public DefaultTemporaryBehavior(TemporaryBehaviorSO config, Unit targetUnit, ComponentsPack startComponents, ComponentsPack endComponents) : base(config, targetUnit, startComponents, endComponents) { }
}
