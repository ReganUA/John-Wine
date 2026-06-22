using System;
using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyController : Controller, IUpdatable
{
    public Transform FirePoint;
    [SerializeField] private bool _useWeapon;
    [SerializeField] private GameObject healingOrbPrefab;

    private EnemyPathfinding _pf;
    private Animator _anim;
    private void Start()
    {
        _unit.OnSpawn();
        _pf = GetComponent<EnemyPathfinding>();
        _anim = GetComponent<Animator>();
    }
    public override void OnStart()
    {
        _unit.OnHealthIsZero += OnDeath;
        Registerer.RegisterUpdatable(this);
        _unit.ChangeAbility(0);
    }
    public void OnUpdate(float dt)
    {
        _unit.OnUpdate(dt);
        _unit.State.CurrentAbility.ReloadProgress(dt);
        
        if (_useWeapon == true)
            HandleWeapon();

        UpdateAnimation();
    }
    private void HandleWeapon()
    {
        if (_pf._token == true)
        {   
            if (_unit.State.CurrentAbility.CanShoot == false) return;

            _anim.SetTrigger("Attack");
            _unit.State.CurrentAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
            _unit.State.CurrentAbility.ResetReloadProgress();
        }
    }
    public override void OnDeath()
    {
        _unit.OnHealthIsZero -= OnDeath;
        Registerer.UnregisterUpdatable(this);
        WaveSpawner.instance.EnemyDied(gameObject);

        if (healingOrbPrefab != null)
            Instantiate(healingOrbPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
    private void UpdateAnimation()
    {
        _anim.SetFloat("Speed", _unit.State.MoveState.CurrentSpeed);
    }
}
