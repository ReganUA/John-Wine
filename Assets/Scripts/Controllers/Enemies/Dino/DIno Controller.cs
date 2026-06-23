
using UnityEngine;

public class DionysusController : Controller, IUpdatable
{
    public Transform FirePoint;

    private EnemyPathfinding _pf;
    private Animator _anim;
    void Start() => _unit.OnSpawn();
    public override void OnStart()
    {
        _anim = GetComponent<Animator>();
        _pf = GetComponent<EnemyPathfinding>();
        Registerer.RegisterUpdatable(this);
        _unit.ChangeAbility(0);

        _unit.OnHealthIsZero += OnDeath;
    }

    public void OnUpdate(float dt)
    {
        _unit.OnUpdate(dt);
        _unit.State.CurrentAbility.ReloadProgress(dt);

        HandleWeapon();
        UpdateAnimation();
    }
    private void HandleWeapon()
    {
        if (_unit.State.CurrentAbility.CanShoot == false) return;

        //_anim.SetInteger("Attack1", Random.Range(1, 3));
        //_unit.ChangeAbility(Random.Range(1, 3));

        Ability curAbility = _unit.State.CurrentAbility;

        Vector3 playerVelocity = (GameManager.instance.player.State.MoveState.CurrentMoveDirection * GameManager.instance.player.State.MoveState.CurrentSpeed)
                         * (curAbility.config.LaunchComponents.UnitSpawner._prefab.UnitSO.SimComponents.Movers.Mover.Stats.MaxSpeed * Vector3.Distance(_unit.transform.position, GameManager.instance.player.transform.position)) * 0.05f   
                         + (GameManager.instance.player.State.MoveState.ExternalForcesVelocity + new Vector3(0, 2, 0));
        Vector3 targetPointPos = GameManager.instance.player.transform.position + playerVelocity;
        Vector3 dirToTarget = targetPointPos - FirePoint.position;
        Quaternion angle = Aim(dirToTarget);
        PositionArgs firePoint = new PositionArgs(FirePoint.position, angle, FirePoint.forward);

        _unit.State.CurrentAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), firePoint, _unit);
        _unit.State.CurrentAbility.ResetReloadProgress();
    }
    private Quaternion Aim(Vector3 dirToTarget)
    {
        if (dirToTarget.sqrMagnitude < 0.001f) return FirePoint.rotation;

        Quaternion targetRotation = Quaternion.LookRotation(dirToTarget, Vector3.up);

        // Quaternion finalRotation = targetRotation * Quaternion.Euler(0, -45, 0);

        return targetRotation;
    }
    public override void OnDeath()
    {
        _unit.OnHealthIsZero -= OnDeath;
        Registerer.UnregisterUpdatable(this);

        //trigger smth
    }
    private void UpdateAnimation()
    {
        _anim.SetFloat("Speed", _unit.State.MoveState.CurrentSpeed);
    }
}
