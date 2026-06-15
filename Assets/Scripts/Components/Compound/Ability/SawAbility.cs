using System;
using UnityEngine;

public sealed class SawAbility : Ability
{
    public override void Fire(PositionArgs raycastPos, PositionArgs firePointPos, Unit whoFired)
    {
        if (IsBlocked == true) return;

        IsBlocked = true;
        base.Fire(raycastPos, firePointPos, whoFired);
    }
    public override void Hold(PositionArgs raycastPos, PositionArgs firePointPos, float dt)
    {
        if (Spawned.Count == 0) return;

        RaycastHit hit = config.LaunchComponents.Raycaster.Raycast(RuntimeStats, raycastPos.position, raycastPos.direction);
        Vector3 dir = Vector3.zero;
        RaycastStats raycastStats = RuntimeStats.GetStats(config.LaunchComponents.Raycaster);
        for (int i = 0; i < Spawned.Count; i++)
        {
            if (hit.collider != null)
            {
                dir = hit.point - Spawned[i].transform.position;
            }
            else
            {
                Vector3 targetPoint = raycastPos.position + (raycastPos.direction * raycastStats.Range);
                dir = targetPoint - Spawned[i].transform.position;
            }

            //Spawned[i].State.MoveState.CurrentMoveDirection = dir * Spawned[i].Stats.GetStats(Spawned[i].UnitSO.SimComponents.Movers.Mover).Acceleration;

            Spawned[i].UnitSO.SimComponents.Movers.Mover.Move(Spawned[i], dir, dt);
        }
    }
    public override void Release()
    {
        for (int i = 0;i < Spawned.Count;i++)
        {
            Unit spawned = Spawned[i];
            Spawned.Remove(spawned);
            spawned.Die();
        }
        IsBlocked = false;
    }
    public SawAbility(AbilitySO so, ComponentRuntimeStats statsCarrier) : base(so, statsCarrier) { }
}
