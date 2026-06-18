
using UnityEngine;

public class DInoController : Controller, IUpdatable
{
    public Transform FirePoint;

    private EnemyPathfinding _pf;
    private Animator _anim;
    void Start() => _unit.OnSpawn();
    public override void OnStart()
    {
        Registerer.RegisterUpdatable(this);
        _unit.ChangeAbility(0);
    }

    public void OnUpdate(float dt)
    {
        _unit.OnUpdate(dt);
        _unit.State.CurrentAbility.ReloadProgress(dt);

        HandleWeapon();
    }
    private void HandleWeapon()
    {
        if (_pf._token)
        {
            if (_unit.State.CurrentAbility.CanShoot == false) return;

            _anim.SetInteger("Attack1", Random.Range(1, 3));
            _unit.ChangeAbility(Random.Range(1, 3));

            _unit.State.CurrentAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
            _unit.State.CurrentAbility.ResetReloadProgress();
        }
    }
}
