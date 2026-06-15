using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public UnitSO UnitSO;
    [field: SerializeField] public Controller ControllerScript { get; private set; }
    [field: SerializeField] public Transform Turret { get; private set; }
    [SerializeField] internal References Refs;
    [HideInInspector] public Unit Owner;
    [HideInInspector] public BehaviorMachine BehaviorMachine;
    public ComponentRuntimeStats Stats = new();
    public List<Ability> Abilities = new List<Ability>();
    public ModifiableStats<HealthStats> Health;
    public UnitState State;

    public event Action OnTakeDamageEvent;
    public event Action OnHealthIsZero;
    public event Action OnKillEvent;

    public void OnSpawn(Unit owner = null)
    {
        Owner = owner;
        OnStart();
    }
    private void OnStart()
    {
        if (UnitSO.StatsTemplate != null)
        {
            Health = new(UnitSO.StatsTemplate.Health);
            State = new(UnitSO.StatsTemplate);
        }
        BehaviorMachine = new BehaviorMachine(this);

        Stats.SetComponentsStats(UnitSO.SimComponents);

        for (int i = 0; i < UnitSO.SimComponents.Abilities.Count; i++)
        {
            Abilities.Add(UnitSO.SimComponents.Abilities[i].CreateAbility(Stats));
        }

        State.HealthState.CurrentHealth = Health.Value.HealthOnStart;

        if (UnitSO.SimComponents.TemporaryBehaviour != null)
            UnitSO.SimComponents.TemporaryBehaviour.ApplyBehavior(this);
        if (UnitSO.SimComponents.PeriodicBehaviour != null)
            UnitSO.SimComponents.PeriodicBehaviour.ApplyBehavior(this);

        if (ControllerScript != null)
            ControllerScript.OnStart();
        //OnStartEvent?.Invoke();
    }
    public void OnUpdate(float dt)
    {
        BehaviorMachine.OnUpdate(dt);
    }
    public void TakeDamage(float amount)
    {
        float finalAmount = amount - Health.Value.Armor;
        if (finalAmount < 0)
        {
            finalAmount = 0;
            return;
        }
        State.HealthState.CurrentHealth -= finalAmount;

        OnTakeDamageEvent?.Invoke();
        if (State.HealthState.CurrentHealth <= 0)
        {
            OnHealthIsZero?.Invoke();
        }
    }
    public void Die()
    {
        ControllerScript.OnDeath();
        Destroy(gameObject);
    }
    public void KillCredit()
    {
        OnKillEvent?.Invoke();
    }

    public void ChangeAbility(int abilityIndex)
    {
        if (Abilities[abilityIndex] == null) return;

        State.CurrentAbility = Abilities[abilityIndex];
        Debug.Log("Current ability: " + State.CurrentAbility.GetType());
    }
    public void AddAbility(Ability ability)
    {
        Abilities.Add(ability);
    }
}
public class UnitState
{
    public Ability CurrentAbility;
    public HealthState HealthState;
    public MovementState MoveState = new();

    public UnitState(StatsTemplate stats)
    {
        HealthState = new(stats);
    }
}
[Serializable]
public struct References
{
    [SerializeField] public Rigidbody RB;
    [SerializeField] public CharacterController CC;
}
[Serializable]
[Flags]
public enum Tags
{
    None = 0,
    Projectile = 1 << 0,
    Entity = 1 << 1,
    Invulnerable = 1 << 2,
    Obsticle = 1 << 3,
    Hidden = 1 << 4,
    Key = 1 << 5
}
public abstract class Controller : MonoBehaviour
{
    [SerializeField] protected Unit _unit;
    public abstract void OnStart();
    public virtual void OnDeath() { }
}
[Serializable]
public class MovementState
{
    public float CurrentSpeed;
    public float CurrentAcceleration;
    public float CurrentDeceleration;
    public Vector3 ExternalForcesVelocity;
    public Vector3 CurrentMoveDirection;
    public float Pitch;
    public float Yaw;
    public float Roll;
    public Vector3 MovementVelocity;
    public Vector2 RotationalVelocity;
}
[Serializable]
public class HealthState
{
    public float CurrentHealth;
    public float HealthDelta => CurrentHealth / _statsRef.Health.MaxHealth;

    private StatsTemplate _statsRef;
    public HealthState(StatsTemplate stats)
    {
        _statsRef = stats;
    }
}
public interface IAbilityConfigCarrier
{
    public AbilitySO abilitySO { get; set; }
}