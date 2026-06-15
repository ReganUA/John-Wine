using System;
using UnityEngine;

public abstract class BehaviorSO : ScriptableObject
{
    protected virtual TemporaryBehavior CreateTempBehavior(Unit targetUnit) { return null; }
    protected virtual PeriodicBehavior CreatePeriodBehavior(Unit targetUnit) { return null; }
    public abstract void ApplyBehavior(Unit targetUnit);
}
public abstract class TemporaryBehaviorSO : BehaviorSO
{
    [field: SerializeField] public TemporaryBehaviorStats Stats { get; private set; }
    [SerializeField] public ComponentsPack StartComponents;
    [SerializeField] public ComponentsPack EndComponents;
}
public abstract class PeriodicBehaviorSO : BehaviorSO
{
    [field: SerializeField] public PeriodicBehaviorStats Stats { get; private set; }
    [SerializeField] public ComponentsPack PeriodicComponents;
}
[Serializable]
public struct TemporaryBehaviorStats
{
    public float Duration;
}
[Serializable]
public struct PeriodicBehaviorStats
{
    public float Duration;
    public float Period;
}
