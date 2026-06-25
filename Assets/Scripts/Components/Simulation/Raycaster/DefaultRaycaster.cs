using UnityEngine;

[CreateAssetMenu(fileName = "Default Raycaster", menuName = "Components/Simulation/Raycast/Default Raycaster")]
public class DefaultRaycaster : RaycasterSO
{
    public override RaycastHit Raycast(ComponentRuntimeStats statsCarrier, Vector3 origin, Vector3 dir)
    {
        RaycastStats stats = statsCarrier.GetStats(this);
        RaycastHit hit;

        Physics.Raycast(origin, dir, out hit, stats.Range, stats.Layer);

        float actualLength = hit.collider != null ? hit.distance : stats.Range;

        if (stats.Emitter != null)
        {
            var p = stats.Emitter.Emit(new PositionArgs(origin, Quaternion.LookRotation(dir)));
            var shape = p.shape;
            shape.scale = new Vector3(shape.scale.x,shape.scale.y, actualLength);
            shape.position = new Vector3(0f, 0f, actualLength * 0.5f);
            p.Play();
        }
        return hit;
    }
}
