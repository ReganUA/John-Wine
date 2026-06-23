using System;
using UnityEngine;

public abstract class Spawner<T> : ScriptableObject
{
    [field: SerializeField] public T _prefab { get; private set; }
    [SerializeField] protected Vector3 PositionOffset;
    public abstract T Spawn(PositionArgs args, T owner = default);
}
public struct PositionArgs
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 direction;

    public PositionArgs(Vector3 position, Quaternion rotation, Vector3 direction = default)
    {
        this.position = position;
        this.rotation = rotation;
        this.direction = direction;
    }
}