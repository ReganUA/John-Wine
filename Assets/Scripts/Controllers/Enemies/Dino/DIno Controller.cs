
using UnityEngine;

public class DInoController : Controller, IUpdatable
{
    public Transform FirePoint;

    private EnemyPathfinding _pf;
    private Animator _anim;
    [SerializeField] private GameObject titles;
    void Start() => _unit.OnSpawn();
    public override void OnStart()
    {
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
        if (_pf._token)
        {
            if (_unit.State.CurrentAbility.CanShoot == false) return;

            _anim.SetInteger("Attack1", Random.Range(1, 3));
            _unit.ChangeAbility(Random.Range(1, 3));

            _unit.State.CurrentAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
            _unit.State.CurrentAbility.ResetReloadProgress();
        }
    }
    public override void OnDeath()
    {
        _unit.OnHealthIsZero -= OnDeath;
        Registerer.UnregisterUpdatable(this);
        WaveSpawner.instance.EnemyDied(gameObject);

        titles.SetActive(true);
        gameObject.SetActive(false);
    }
    private void UpdateAnimation()
    {
        _anim.SetFloat("Speed", _unit.State.MoveState.CurrentSpeed);
    }
}
