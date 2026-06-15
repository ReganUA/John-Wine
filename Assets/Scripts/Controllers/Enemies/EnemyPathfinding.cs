using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyPathfinding : MonoBehaviour, IUpdatable
{
    [HideInInspector] public Unit unit;
    [HideInInspector] public Transform playerTarget;
    [HideInInspector] public bool _token;

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
        playerTarget = GameManager.instance.player.transform;
        unit = GetComponent<Unit>();
        Registerer.RegisterUpdatable(this);
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        unit.OnTakeDamageEvent += OnDamage;
        unit.OnHealthIsZero += Death;

        TokenManager.instance._enemies.Add(gameObject, this);

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        _modifier = new Vector3(randomCircle.x, 0, randomCircle.y) * Random.Range(_stats.minDistanceFlank, _stats.maxDistanceFlank);
        _threshold = _modifier.magnitude;

        if (isNotMoving)
        {
            float maxSpeed = unit.Stats.GetStats(unit.UnitSO.SimComponents.Movers.Mover).MaxSpeed;
            unit.Stats.GetStatsModifiable(unit.UnitSO.SimComponents.Movers.Mover).BuffAdd(new MovementStats() {MaxSpeed = -maxSpeed });
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

        GetToken();
        Mover(deltaTime);

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit groundHit, 5f, NavMesh.AllAreas))
        {
            transform.position = new Vector3(transform.position.x, groundHit.position.y, transform.position.z);
        }
    }
    private void GetToken()
    {
        Vector3 target;

        if (_distanceToPlayer <= _threshold + 0.5f)
        {
            if (!_token)
            {
                _token = TokenManager.instance.RequestToken(gameObject, _distanceToPlayer + (_stats.tokenPriority * -1f));
            }

            target = _token ? playerTarget.position : _flank;
        }
        else
        {
            target = _flank;

            if (_token && _distanceToPlayer > _threshold + 1f)
            {
                ReleaseToken();
            }
        }
        if (Vector3.Distance(_agent.destination, target) > 0.5f)
        {
            _agent.SetDestination(target);
        }
    }
    private void Mover(float dt)
    {
        unit.UnitSO.SimComponents.Movers.RotationalMover.Move(unit, playerTarget.position, dt);

        if (_distanceToPlayer > _stats.maxDistanceSearch)
        {
            unit.UnitSO.SimComponents.Movers.Mover.Move(unit, Vector3.zero, dt);
            return;
        }

        if (_token != false && _distanceToPlayer <= _threshold)
        {
            ChangeFlank();
        }

        if (_agent.pathPending == false && _agent.remainingDistance < 0.2f)
        {
            unit.UnitSO.SimComponents.Movers.Mover.Move(unit, Vector3.zero, dt);

            if (_token == false)
            {
                ChangeFlank();
            }
        }
        else
        {
            if(_token == true && _distanceToPlayer <= _stats.stoppingDistance)
            {
                unit.UnitSO.SimComponents.Movers.Mover.Move(unit, Vector3.zero, dt);
            }
            else
            {
                unit.UnitSO.SimComponents.Movers.Mover.Move(unit, new Vector3(_agent.desiredVelocity.x, 0, _agent.desiredVelocity.z).normalized, dt);
            }
        }
    }
    private void ChangeFlank()
    {
        if (ignoreFlank == true) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        _modifier = new Vector3(randomCircle.x, 0, randomCircle.y) * Random.Range(_stats.minDistanceFlank, _stats.maxDistanceFlank);
        _threshold = _modifier.magnitude;
    }
    public void ReleaseToken()
    {
        if (_token)
        {
            if (TokenManager.instance._activeTokens.ContainsKey(gameObject))
            {
                TokenManager.instance._activeTokens.Remove(gameObject);
                _token = false;
            }
        }
    }
    public void Death()
    {
        ReleaseToken();
        TokenManager.instance._enemies.Remove(gameObject);
        Registerer.UnregisterUpdatable(this);

        unit.OnHealthIsZero -= Death;
        unit.OnTakeDamageEvent -= OnDamage;
    }
    public void OnDamage()
    {
        if (unit.State.HealthState.HealthDelta <= 0.5f)
        {
            ReleaseToken();
            ChangeFlank();
        }
    }
    public void ReturnSpeedToNormal()
    {
        unit.Stats.GetStatsModifiable(unit.UnitSO.SimComponents.Movers.Mover).BuffAdd(new MovementStats() { MaxSpeed = maxSpeed });
    }
}
