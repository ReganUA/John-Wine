using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit SO", menuName = "Components/Compound/Unit/Unit SO")]
public class UnitSO : ScriptableObject
{
    [Header("General")]
    public StatsTemplate StatsTemplate;
    [SerializeField] public ComponentsPack SimComponents;
    public Tags Tags;
    public Team Team;
}
[Serializable]
public struct ComponentsPack
{
    public SensorSO Sensor;
    public RaycasterSO Raycaster;
    public AreaSearchSO AreaSearcher;
    public Movers Movers;
    public List<AbilitySO> Abilities;
    public Spawner<Unit> UnitSpawner;
    public EffectSO Effect;
    public TemporaryBehaviorSO TemporaryBehaviour;
    public PeriodicBehaviorSO PeriodicBehaviour;
    public EmitterSO Emitter;
}
[Serializable]
public struct Movers
{
    public MoverSO Mover;
    public MoverSO RotationalMover;
}