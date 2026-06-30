using UnityEngine;

[CreateAssetMenu(fileName = "Default Raycaster", menuName = "Components/Simulation/Raycast/Default Raycaster")]
public class DefaultRaycaster : RaycasterSO
{
    public override RaycastHit Raycast(ComponentRuntimeStats statsCarrier, Vector3 origin, Vector3 dir, PositionArgs sfxPos = default)
    {
        RaycastStats stats = statsCarrier.GetStats(this);
        RaycastHit hit;

        Physics.Raycast(origin, dir, out hit, stats.Range, stats.Layer);

        float actualLength = hit.collider != null ? hit.distance : stats.Range;
        if (stats.Emitter != null)
        {
            Vector3 hitPoint = hit.collider != null ? hit.point : origin + dir * stats.Range;
            var p = stats.Emitter.Emit(new PositionArgs(sfxPos.position, Quaternion.LookRotation(hitPoint - sfxPos.position)));

            var shape = p.shape;
            var emission = p.emission;

            shape.scale = new Vector3(shape.scale.x, shape.scale.y, actualLength);
            shape.position = new Vector3(0f, 0f, 0);

            p.Play();
        }
        return hit;
    }
}
