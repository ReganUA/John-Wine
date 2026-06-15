using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaSearchSO : ScriptableObject
{
    [field: SerializeField] public AreaSearchStats Stats { get; private set; } = AreaSearchStats.Default;
    /// <summary>
    /// Method searches for units in area and calls components for each one of them.
    /// </summary>
    /// <returns>List of found objects</returns>
    public abstract List<Unit> Search(ComponentRuntimeStats statsCarrier, PositionArgs posArgs, Unit owner);
}
[Serializable]
public struct AreaSearchStats
{
    public Vector3 Size;
    [Range(0f, 360f)]
    public float Angle;
    public int MaximumTargetCount;
    public readonly float CosCutOff => Mathf.Cos((Angle * 0.5f) * Mathf.Deg2Rad);
    public ComponentsPack Components;

    public static AreaSearchStats Default => new AreaSearchStats
    {
        Size = Vector3.one,
        Angle = 360f,
        MaximumTargetCount = 50
    };
}