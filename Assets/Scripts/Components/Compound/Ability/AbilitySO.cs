using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    [field: SerializeField] public AbilityStats Stats { get; set; }
    [Header("Launch")]
    [SerializeField] internal ComponentsPack LaunchComponents;
    [Header("Impact")]
    [SerializeField] internal ComponentsPack ImpactComponents;
    public abstract Unit Fire(ComponentRuntimeStats statsCarrier, PositionArgs raycastPos, PositionArgs firePointPos, Unit sourceUnit);
    public abstract void OnHit(ComponentRuntimeStats statsCarrier, PositionArgs hitPos, Unit sourceUnit, Unit hitUnit);
    public abstract Ability CreateAbility(ComponentRuntimeStats statsCarrier);
}
[Serializable]
public class Ability
{
    public bool CanShoot;
    public bool IsBlocked;
    public List<Unit> Spawned = new();


    public AbilitySO config;
    protected ComponentRuntimeStats RuntimeStats;
    protected Unit owner;


    private float _reloadProgress = 0;
    private ModifiableStats<AbilityStats> _stats;
    public virtual void Fire(PositionArgs raycastPos, PositionArgs firePointPos, Unit whoFired)
    {
        Unit spawned = config.Fire(RuntimeStats, raycastPos, firePointPos, whoFired);

        Spawned.Add(spawned);
    }
    public virtual void Hold(PositionArgs raycastPos, PositionArgs firePointPos, float dt) { }
    public virtual void Release() { }
    public void ReloadProgress(float dt)
    {
        if (_reloadProgress < 1.0f)
        {
            _reloadProgress += dt / _stats.Value.ReloadSpeed;

            if (_reloadProgress > 1.0f)
            {
                _reloadProgress = 1.0f;
                CanShoot = true;
            }
        }
    }
    public void ResetReloadProgress()
    {
        CanShoot = false;
        _reloadProgress = 0;
    }
    public Ability(AbilitySO so, ComponentRuntimeStats statsCarrier)
    {
        config = so;
        RuntimeStats = statsCarrier;

        _stats = statsCarrier.GetStatsModifiable(config);
    }
}
[Serializable]
public struct AbilityStats
{
    public float ReloadSpeed;
}