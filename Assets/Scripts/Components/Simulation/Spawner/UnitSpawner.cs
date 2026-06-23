using System;
using UnityEngine;

[CreateAssetMenu(fileName ="Unit Spawner", menuName ="Components/Simulation/Spawner/Unit Spawner")]
public sealed class UnitSpawner : Spawner<Unit>
{
    public override Unit Spawn(PositionArgs args, Unit owner)
    {
        Vector3 spawnPos = args.position + (args.rotation * PositionOffset);
        Unit spawned = Instantiate(_prefab, spawnPos, args.rotation);
        spawned.Owner = owner;
        return spawned;
    }
}
