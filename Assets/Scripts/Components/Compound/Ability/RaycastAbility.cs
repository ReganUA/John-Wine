using UnityEngine;

[CreateAssetMenu(fileName = "Raycast Ability", menuName = "Components/Compound/Ability/Default Raycast Ability")]
public class RaycastAbility : AbilitySO
{
    public override Unit Fire(ComponentRuntimeStats statsCarrier, PositionArgs raycastPos, PositionArgs firePointPos, Unit sourceUnit)
    {
        Unit spawned = null;
        if (LaunchComponents.Effect != null)
            LaunchComponents.Effect.Affect(sourceUnit, statsCarrier);

        if (LaunchComponents.PeriodicBehaviour != null)
            LaunchComponents.PeriodicBehaviour.ApplyBehavior(sourceUnit);

        if (LaunchComponents.TemporaryBehaviour != null)
            LaunchComponents.TemporaryBehaviour.ApplyBehavior(sourceUnit);

        if (LaunchComponents.AreaSearcher != null)
            LaunchComponents.AreaSearcher.Search(statsCarrier, raycastPos, sourceUnit);

        if (LaunchComponents.Abilities != null)
        {
            for (int j = 0; j < LaunchComponents.Abilities.Count; j++)
            {
                LaunchComponents.Abilities[j].Fire(statsCarrier, raycastPos, firePointPos, sourceUnit);
            }
        }

        if (LaunchComponents.UnitSpawner != null)
        {
            spawned = LaunchComponents.UnitSpawner.Spawn(raycastPos, sourceUnit);
            if (spawned != null && spawned.ControllerScript is IAbilityConfigCarrier abilityCarrier)
                abilityCarrier.abilitySO = this;
            spawned.OnSpawn(sourceUnit);
        }

        RaycastHit _hit = LaunchComponents.Raycaster.Raycast(statsCarrier, raycastPos.position, raycastPos.direction);
        if (_hit.collider != null)
        {
            _hit.collider.TryGetComponent(out Unit hitUnit);
            OnHit(statsCarrier, new PositionArgs(_hit.point, raycastPos.rotation, raycastPos.direction), sourceUnit, hitUnit);
            Debug.DrawLine(raycastPos.position, _hit.point, Color.red, 0.05f);
        }
        else
            Debug.DrawLine(raycastPos.position, raycastPos.position + raycastPos.direction * statsCarrier.GetStats(LaunchComponents.Raycaster).Range, Color.red, 0.05f);

        return spawned;
    }
    public override void OnHit(ComponentRuntimeStats statsCarrier, PositionArgs hitPos, Unit sourceUnit, Unit hitUnit)
    {
        if (hitUnit != null)
        {
            if (ImpactComponents.Effect != null)
                ImpactComponents.Effect.Affect(hitUnit, statsCarrier);

            if (ImpactComponents.PeriodicBehaviour != null)
                ImpactComponents.PeriodicBehaviour.ApplyBehavior(hitUnit);

            if (ImpactComponents.TemporaryBehaviour != null)
                ImpactComponents.TemporaryBehaviour.ApplyBehavior(hitUnit);
        }

        if (ImpactComponents.AreaSearcher != null)
            ImpactComponents.AreaSearcher.Search(statsCarrier, hitPos, sourceUnit);

        if (ImpactComponents.Abilities != null)
        {
            for (int j = 0; j < ImpactComponents.Abilities.Count; j++)
            {
                ImpactComponents.Abilities[j].Fire(statsCarrier, new PositionArgs(hitPos.position, hitPos.rotation, hitPos.direction), new PositionArgs(hitPos.position, hitPos.rotation, hitPos.direction), sourceUnit);
            }
        }

        if (ImpactComponents.UnitSpawner != null)
        {
            Unit spawned = ImpactComponents.UnitSpawner.Spawn(new PositionArgs(hitPos.position, Quaternion.identity), sourceUnit);
            spawned.OnSpawn(sourceUnit);
        }
    }
    public override Ability CreateAbility(ComponentRuntimeStats statsCarrier)
    {
        return new Ability(this, statsCarrier);
    }
}
