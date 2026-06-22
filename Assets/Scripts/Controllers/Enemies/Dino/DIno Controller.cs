
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

        _anim.SetInteger("Attack1", Random.Range(1, 3));
        _unit.ChangeAbility(Random.Range(1, 3));

        _unit.State.CurrentAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
        _unit.State.CurrentAbility.ResetReloadProgress();
    }
    private void Aim()
    {
        Vector3 dirToPlayer = (GameManager.instance.player.transform.position - FirePoint.transform.position).normalized;
        float dot
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
