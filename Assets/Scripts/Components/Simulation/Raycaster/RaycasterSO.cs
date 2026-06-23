using System;
using UnityEngine;

public abstract class RaycasterSO : ScriptableObject
{
    [field: SerializeField] public RaycastStats Stats { get; private set; }
    public abstract RaycastHit Raycast(ComponentRuntimeStats statsCarrier, Vector3 origin, Vector3 dir);
}
[Serializable]
public struct RaycastStats
{
    public float Range;
    public LayerMask Layer;
    public EmitterSO Emitter;
}