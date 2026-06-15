using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Spawn Ability", menuName = "Components/Compound/Ability/Default Spawn Ability")]
public class SpawnAbility : AbilitySO
{
    public override Unit Fire(ComponentRuntimeStats statsCarrier, PositionArgs raycastPos, PositionArgs firePointPos, Unit sourceUnit)
    {
        Unit spawned = null;
        Debug.Log("Fire!");
        if (LaunchComponents.Effect != null)
            LaunchComponents.Effect.Affect(sourceUnit, statsCarrier);

        if (LaunchComponents.PeriodicBehaviour != null)
            LaunchComponents.PeriodicBehaviour.ApplyBehavior(sourceUnit);

        if (LaunchComponents.TemporaryBehaviour != null)
            LaunchComponents.TemporaryBehaviour.ApplyBehavior(sourceUnit);

        if (LaunchComponents.AreaSearcher != null)
            LaunchComponents.AreaSearcher.Search(statsCarrier, raycastPos, sourceUnit);

        if (LaunchComponents.Raycaster != null)
        {
            RaycastHit _hit = LaunchComponents.Raycaster.Raycast(statsCarrier, raycastPos.position, raycastPos.direction);

            if (_hit.point != default && _hit.distance > 1)
            {
                Vector3 _desiredFireDirection = (_hit.point - firePointPos.position).normalized;

                firePointPos = new PositionArgs(firePointPos.position, Quaternion.LookRotation(_desiredFireDirection), firePointPos.direction);
            }
        }

        if (LaunchComponents.Abilities != null)
        {
            for (int j = 0; j < LaunchComponents.Abilities.Count; j++)
            {
                LaunchComponents.Abilities[j].Fire(statsCarrier, raycastPos, firePointPos, sourceUnit);
            }
        }

        if (LaunchComponents.UnitSpawner != null)
        {
            spawned = LaunchComponents.UnitSpawner.Spawn(firePointPos, sourceUnit);
            if (spawned != null && spawned.ControllerScript is IAbilityConfigCarrier abilityCarrier)
                abilityCarrier.abilitySO = this;
            spawned.OnSpawn(sourceUnit);
        }

        if (LaunchComponents.Emitter != null)
        {
            LaunchComponents.Emitter.Emit(new PositionArgs(firePointPos.position, firePointPos.rotation));
        }

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

        if(ImpactComponents.Emitter != null)
        {
            ImpactComponents.Emitter.Emit(new PositionArgs(hitPos.position, Quaternion.identity));
        }
    }
    public override Ability CreateAbility(ComponentRuntimeStats statsCarrier)
    {
        return new Ability(this, statsCarrier);
    }
}