using UnityEngine;

public abstract class Behavior
{
    public bool Ended = false;
    protected Unit target;
    protected ComponentRuntimeStats statsCarrier = new();

    protected float durationProgress;
    public abstract void OnUpdate(float dt);
    public virtual void ResetDurtaionProgress()
    {
        durationProgress = 0;
    }
    public Behavior(Unit targetUnit)
    {
        target = targetUnit;
    }
}
public class TemporaryBehavior : Behavior
{
    public ModifiableStats<TemporaryBehaviorStats> Stats;

    protected TemporaryBehaviorSO config;
    protected ComponentsPack StartComponents;
    protected ComponentsPack EndComponents;

    public virtual void OnStart()
    {

    }
    public override void OnUpdate(float dt)
    {
        if (durationProgress < 1.0f)
        {
            durationProgress += dt / Stats.Value.Duration;

            if (durationProgress > 1.0f)
            {
                durationProgress = 1.0f;
                Ended = true;
            }
        }
    }
    public virtual void OnEnd()
    {

    }
    public TemporaryBehavior(TemporaryBehaviorSO config, Unit targetUnit, ComponentsPack startComponents, ComponentsPack endComponents) : base(targetUnit)
    {
        this.config = config;
        StartComponents = startComponents;
        EndComponents = endComponents;

        Stats = new ModifiableStats<TemporaryBehaviorStats>(config.Stats);
        statsCarrier.SetComponentsStats(StartComponents);
        statsCarrier.SetComponentsStats(EndComponents);
    }
}
public class PeriodicBehavior : Behavior
{
    public bool PeriodTicked = false;

    public ModifiableStats<PeriodicBehaviorStats> Stats;

    protected PeriodicBehaviorSO config;
    protected ComponentsPack PeriodicComponents;

    private float _periodProgress;
    public virtual void OnPeriod()
    {

    }
    public override void OnUpdate(float dt)
    {
        if (_periodProgress < 1.0f)
        {
            _periodProgress += dt / Stats.Value.Period;

            if (_periodProgress > 1.0f)
            {
                PeriodTicked = true;
                _periodProgress = 1.0f;
            }
        }

        if (durationProgress < 1.0f)
        {
            durationProgress += dt / Stats.Value.Duration;

            if (durationProgress > 1.0f)
            {
                durationProgress = 1.0f;
                Ended = true;
            }
        }
    }
    public virtual void ResetPeriodProgress()
    {
        PeriodTicked = false;
        _periodProgress = 0;
    }
    public PeriodicBehavior(PeriodicBehaviorSO config, Unit targetUnit, ComponentsPack periodicComponents) : base(targetUnit)
    {
        this.config = config;
        PeriodicComponents = periodicComponents;

        Stats = new ModifiableStats<PeriodicBehaviorStats>(config.Stats);
        statsCarrier.SetComponentsStats(PeriodicComponents);
    }
}