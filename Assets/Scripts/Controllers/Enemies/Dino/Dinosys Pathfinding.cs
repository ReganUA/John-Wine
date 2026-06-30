using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class DinosysPathfidning : MonoBehaviour, IUpdatable
{
    [HideInInspector] public Unit unit;
    [HideInInspector] public Transform playerTarget;

    [SerializeField] private PathfindingStats _stats;

    private NavMeshAgent _agent;
    private Vector3 _flank;
    private Vector3 _modifier;
    private float _threshold;
    private Vector3 _groundedPlayerPos;
    private float _distanceToPlayer;

    [SerializeField] private bool isNotMoving;
    private float maxSpeed;

    [SerializeField] private bool ignoreFlank;
    void Start()
    {
        StartCoroutine(LaterLoad());
        unit = GetComponent<Unit>();
        Registerer.RegisterUpdatable(this);
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        unit.OnHealthIsZero += Death;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        _modifier = new Vector3(randomCircle.x, 0, randomCircle.y) * Random.Range(_stats.minDistanceFlank, _stats.maxDistanceFlank);
        _threshold = _modifier.magnitude;

        if (isNotMoving)
        {
            float maxSpeed = unit.Stats.GetStats(unit.UnitSO.SimComponents.Movers.Mover).MaxSpeed;
            unit.Stats.GetStatsModifiable(unit.UnitSO.SimComponents.Movers.Mover).BuffAdd(new MovementStats() { MaxSpeed = -maxSpeed });
        }
    }

    public void OnUpdate(float deltaTime)
    {
        if (playerTarget == null) return;
        if (_agent == null) return;
        if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh) return;

        _agent.nextPosition = transform.position;

        if (NavMesh.SamplePosition(playerTarget.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            _groundedPlayerPos = hit.position;
        }
        else
        {
            _groundedPlayerPos = playerTarget.position;
        }

        _flank = new Vector3(playerTarget.position.x + _modifier.x, _groundedPlayerPos.y, playerTarget.position.z + _modifier.z);
        _distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        Mover(deltaTime);

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit groundHit, 5f, NavMesh.AllAreas))
        {
            transform.position = new Vector3(transform.position.x, groundHit.position.y, transform.position.z);
        }
    }
    private void Mover(float dt)
    {
        unit.UnitSO.SimComponents.Movers.RotationalMover.Move(unit, playerTarget.position, dt);

        if (_agent.pathPending == false && _agent.remainingDistance < 0.2f)
        {
            unit.UnitSO.SimComponents.Movers.Mover.Move(unit, Vector3.zero, dt);
        }
        else
        {
            if (_distanceToPlayer <= _stats.stoppingDistance)
            {
                unit.UnitSO.SimComponents.Movers.Mover.Move(unit, Vector3.zero, dt);
            }
            else
            {
                unit.UnitSO.SimComponents.Movers.Mover.Move(unit, new Vector3(_agent.desiredVelocity.x, 0, _agent.desiredVelocity.z).normalized, dt);
            }
        }
    }
    public void Death()
    {
        Registerer.UnregisterUpdatable(this);

        unit.OnHealthIsZero -= Death;
    }
    public void ReturnSpeedToNormal()
    {
        unit.Stats.GetStatsModifiable(unit.UnitSO.SimComponents.Movers.Mover).BuffAdd(new MovementStats() { MaxSpeed = maxSpeed });
    }
    private IEnumerator LaterLoad()
    {
        yield return new WaitForEndOfFrame();
        playerTarget = GameManager.instance.player.transform;
    }
}
