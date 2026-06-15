using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class ComponentRuntimeStats
{
    Dictionary<AbilitySO, ModifiableStats<AbilityStats>> AbilityStats = new();
    Dictionary<MoverSO, ModifiableStats<MovementStats>> MoverStats = new();
    Dictionary<EffectSO, ModifiableStats<EffectStats>> EffectStats = new();
    Dictionary<AreaSearchSO, ModifiableStats<AreaSearchStats>> AreaSearchStats = new();
    Dictionary<SensorSO, ModifiableStats<SensorStats>> SensorStats = new();
    Dictionary<RaycasterSO, ModifiableStats<RaycastStats>> RaycasterStats = new();
    Dictionary<TemporaryBehaviorSO, ModifiableStats<TemporaryBehaviorStats>> TemporaryBehaviorStats = new();
    Dictionary<PeriodicBehaviorSO, ModifiableStats<PeriodicBehaviorStats>> PeriodicBehaviorStats = new();

    public void SetComponentsStats(ComponentsPack pack)
    {
        for (int i = pack.Abilities.Count - 1; i >= 0; i--)
        {
            if (pack.Abilities[i] != null) AddStats(pack.Abilities[i]);
        }
        if (pack.Movers.Mover != null) AddStats(pack.Movers.Mover);
        if (pack.Movers.RotationalMover != null) AddStats(pack.Movers.RotationalMover);

        if (pack.Effect != null) AddStats(pack.Effect);
        if (pack.Sensor != null) AddStats(pack.Sensor);
        if (pack.Raycaster != null) AddStats(pack.Raycaster);

        if (pack.TemporaryBehaviour != null)
        {
            AddStats(pack.TemporaryBehaviour);
        }
        if (pack.PeriodicBehaviour != null)
        {
            AddStats(pack.PeriodicBehaviour);
        }

        if (pack.AreaSearcher != null)
        {
            AddStats(pack.AreaSearcher);
            SetComponentsStats(pack.AreaSearcher.Stats.Components);
        }
    }
    public ref readonly AbilityStats GetStats(AbilitySO config) => ref AbilityStats[config].Value;
    public ref readonly MovementStats GetStats(MoverSO config) => ref MoverStats[config].Value;
    public ref readonly EffectStats GetStats(EffectSO config) => ref EffectStats[config].Value;
    public ref readonly AreaSearchStats GetStats(AreaSearchSO config) => ref AreaSearchStats[config].Value;
    public ref readonly SensorStats GetStats(SensorSO config) => ref SensorStats[config].Value;
    public ref readonly RaycastStats GetStats(RaycasterSO config) => ref RaycasterStats[config].Value;
    public ref readonly TemporaryBehaviorStats GetStats(TemporaryBehaviorSO config) => ref TemporaryBehaviorStats[config].Value;
    public ref readonly PeriodicBehaviorStats GetStats(PeriodicBehaviorSO config) => ref PeriodicBehaviorStats[config].Value;

    public ModifiableStats<AbilityStats> GetStatsModifiable(AbilitySO config) => AbilityStats[config];
    public ModifiableStats<MovementStats> GetStatsModifiable(MoverSO config) => MoverStats[config];
    public ModifiableStats<RaycastStats> GetStatsModifiable(RaycasterSO config) => RaycasterStats[config];

    public void AddStats(AbilitySO config)
    {
        if (AbilityStats.ContainsKey(config)) return;

        AbilityStats.Add(config, new ModifiableStats<AbilityStats>(config.Stats));

        SetComponentsStats(config.LaunchComponents);
        SetComponentsStats(config.ImpactComponents);
    }
    public void AddStats(MoverSO config)
    {
        if (MoverStats.ContainsKey(config)) return;

        MoverStats.Add(config, new ModifiableStats<MovementStats>(config.Stats));
    }
    public void AddStats(EffectSO config)
    {
        if (EffectStats.ContainsKey(config)) return;

        EffectStats.Add(config, new ModifiableStats<EffectStats>(config.Stats));
    }
    public void AddStats(AreaSearchSO config)
    {
        if (AreaSearchStats.ContainsKey(config)) return;

        AreaSearchStats.Add(config, new ModifiableStats<AreaSearchStats>(config.Stats));
    }
    public void AddStats(SensorSO config)
    {
        if (SensorStats.ContainsKey(config)) return;

        SensorStats.Add(config, new ModifiableStats<SensorStats>(config.Stats));
    }
    public void AddStats(RaycasterSO config)
    {
        if (RaycasterStats.ContainsKey(config)) return;

        RaycasterStats.Add(config, new ModifiableStats<RaycastStats>(config.Stats));
    }
    public void AddStats(TemporaryBehaviorSO config)
    {
        if (TemporaryBehaviorStats.ContainsKey(config)) return;

        TemporaryBehaviorStats.Add(config, new ModifiableStats<TemporaryBehaviorStats>(config.Stats));
    }
    public void AddStats(PeriodicBehaviorSO config)
    {
        if (PeriodicBehaviorStats.ContainsKey(config)) return;

        PeriodicBehaviorStats.Add(config, new ModifiableStats<PeriodicBehaviorStats>(config.Stats));
    }
}

public class ModifiableStats<T> where T : struct
{
    private readonly T _baseValue;
    private T _buffValue;

    public ref readonly T Value => ref _value;
    private T _value;

    private static readonly Func<T, T, T> AddOperation;
    private static readonly Func<T, T, T> MultiplyOperation;

    static ModifiableStats()
    {
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

        var paramAddA = Expression.Parameter(typeof(T), "a");
        var paramAddB = Expression.Parameter(typeof(T), "b");
        MemberBinding[] addBindings = new MemberBinding[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            var fieldA = Expression.Field(paramAddA, field);
            var fieldB = Expression.Field(paramAddB, field);

            if (field.FieldType == typeof(float) || field.FieldType == typeof(double) || field.FieldType == typeof(int))
            {
                var addField = Expression.Add(fieldA, fieldB);
                addBindings[i] = Expression.Bind(field, addField);
            }
            else
            {
                addBindings[i] = Expression.Bind(field, fieldA);
            }
        }

        var addMemberInit = Expression.MemberInit(Expression.New(typeof(T)), addBindings);
        AddOperation = Expression.Lambda<Func<T, T, T>>(addMemberInit, paramAddA, paramAddB).Compile();



        var paramStructBase = Expression.Parameter(typeof(T), "sBase");
        var paramStructMult = Expression.Parameter(typeof(T), "sMult");
        MemberBinding[] multiplyBindings = new MemberBinding[fields.Length];

        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo field = fields[i];
            var fieldBase = Expression.Field(paramStructBase, field);
            var fieldMult = Expression.Field(paramStructMult, field);

            if (field.FieldType == typeof(float) || field.FieldType == typeof(double))
            {
                var zero = Expression.Convert(Expression.Constant(0.0), field.FieldType);
                var one = Expression.Convert(Expression.Constant(1.0), field.FieldType);

                var actualMultiplier = Expression.Condition(
                    Expression.Equal(fieldMult, zero),
                    one,
                    fieldMult
                );

                var subOne = Expression.Subtract(actualMultiplier, one);
                var multiplyField = Expression.Multiply(fieldBase, subOne);
                multiplyBindings[i] = Expression.Bind(field, multiplyField);
            }
            else if (field.FieldType == typeof(int))
            {
                var castBase = Expression.Convert(fieldBase, typeof(float));
                var castMult = Expression.Convert(fieldMult, typeof(float));

                var actualMultiplier = Expression.Condition(
                    Expression.Equal(castMult, Expression.Constant(0f)),
                    Expression.Constant(1f),
                    castMult
                );

                var subOne = Expression.Subtract(actualMultiplier, Expression.Constant(1f));
                var multiplyField = Expression.Multiply(castBase, subOne);
                var castToInt = Expression.Convert(multiplyField, typeof(int));
                multiplyBindings[i] = Expression.Bind(field, castToInt);
            }
            else
            {
                multiplyBindings[i] = Expression.Bind(field, Expression.Default(field.FieldType));
            }
        }

        var multiplyMemberInit = Expression.MemberInit(Expression.New(typeof(T)), multiplyBindings);
        MultiplyOperation = Expression.Lambda<Func<T, T, T>>(multiplyMemberInit, paramStructBase, paramStructMult).Compile();
    }

    public ModifiableStats(T baseValue)
    {
        _baseValue = baseValue;
        _buffValue = default;
        UpdateValue();
    }

    public void BuffAdd(T buff)
    {
        _buffValue = AddOperation(_buffValue, buff);
        UpdateValue();
    }

    public void BuffMultiply(T multipliers)
    {
        T buffDelta = MultiplyOperation(_baseValue, multipliers);
        _buffValue = AddOperation(_buffValue, buffDelta);
        UpdateValue();
    }

    private void UpdateValue()
    {
        _value = AddOperation(_baseValue, _buffValue);
    }
}