using UnityEngine;

[CreateAssetMenu(fileName = "Default Sensor", menuName = "Components/Simulation/Sensor/Default Sensor", order = 1)]
public sealed class DefaultSensor : SensorSO
{
    internal override bool IsDetectionViable(ComponentRuntimeStats statsCarrier, Unit hitUnit, Unit sourceUnit)
    {
        if (hitUnit == null) return false;
        if (sourceUnit == hitUnit) return false;

        SensorStats s = statsCarrier.GetStats(this);

        if (s.DetectOwner == false && sourceUnit != null)
        {
            if (sourceUnit.Owner == hitUnit) return false;
        }
        if ((hitUnit.UnitSO.Tags & s.TagFilter) == 0) return false;

        if (sourceUnit != null)
        {
            TeamsFilter relation = CompositionManager.Instance.GetRelation(sourceUnit.UnitSO.Team, hitUnit.UnitSO.Team);
            if ((s.TeamsFilter & relation) == 0) return false;
        }

        return true;
    }
}