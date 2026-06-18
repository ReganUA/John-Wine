using UnityEngine;

[CreateAssetMenu(fileName = "Default Mover", menuName = "Components/Simulation/Mover/Default Mover", order = 1)]
public class DefaultMover : MoverSO
{
    public override void Move(Unit unit, Vector3 dir, float dt)
    {
        MovementStats m = unit.Stats.GetStats(this);
        bool moving = dir.sqrMagnitude > 0.001f;
        UpdateSpeed(m, unit.State.MoveState, moving, dt);

        Vector3 finalVelocity = dir + unit.State.MoveState.MovementVelocity + unit.State.MoveState.ExternalForcesVelocity;

        unit.transform.position += dt * unit.State.MoveState.CurrentSpeed * finalVelocity;
    }
}