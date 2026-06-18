using System.Collections;
using UnityEngine;

public sealed class PlayerController : Controller, IUpdatable
{
    public Unit Camera;
    public Transform FirePoint;
    public float PushPower = 2;

    public float CoyoteTime;
    public float JumpBufferTime;
    private float _coyoteTime;
    private float _jumpBufferTime;
    private ModifiableStats<MovementStats> _moveStats;
    private void Start() => _unit.OnSpawn();
    public override void OnStart()
    {
        _unit.OnHealthIsZero += _unit.Die;
        Registerer.RegisterUpdatable(this);
        _moveStats = _unit.Stats.GetStatsModifiable(_unit.UnitSO.SimComponents.Movers.Mover);
        _unit.ChangeAbility(0);
        GameManager.instance.player = gameObject;

        //StartCoroutine(BuffTest());
    }
    private IEnumerator BuffTest()
    {
        yield return new WaitForSeconds(2);
        _unit.Stats.GetStatsModifiable(_unit.UnitSO.SimComponents.Movers.Mover).BuffMultiply(new MovementStats() { Deceleration = 0.1f });
        Debug.Log("Buff applied.");
    }
    public void OnUpdate(float dt)
    {
        _unit.OnUpdate(dt);

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 moveDir = ConvertToCameraSpace(input);

        HandleGravity(dt);
        HandleJump(dt);
        HandleWeaponChange();
        HandleWeapon(dt);

        _unit.UnitSO.SimComponents.Movers.Mover.Move(_unit, moveDir, dt);
    }
    private void HandleGravity(float dt)
    {
        if (_unit.Refs.CC.isGrounded && _unit.State.MoveState.ExternalForcesVelocity.y < 0)
        {
            _unit.State.MoveState.ExternalForcesVelocity.y = -2f;
        }
        _unit.State.MoveState.ExternalForcesVelocity.y += _unit.Stats.GetStats(_unit.UnitSO.SimComponents.Movers.Mover).Gravity * dt;
    }
    private void HandleJump(float dt)
    {
        if (_unit.Refs.CC.isGrounded == true)
            _coyoteTime = CoyoteTime;
        else
            _coyoteTime -= dt;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferTime = JumpBufferTime;
        }

        if (_jumpBufferTime > 0)
            _jumpBufferTime -= dt;

        if (Input.GetKeyDown(KeyCode.Space) && _coyoteTime > 0 ||_jumpBufferTime > 0 && _unit.Refs.CC.isGrounded == true)
        {
            _unit.State.MoveState.ExternalForcesVelocity.y = Mathf.Sqrt(_moveStats.Value.JumpForce * -2f * _moveStats.Value.Gravity);
        }
    }
    private void HandleWeapon(float dt)
    {
        for (int i = 0; i < _unit.Abilities.Count; i++)
            _unit.Abilities[i].ReloadProgress(dt);

        int activeIdx = _unit.Abilities.FindIndex(a => a.IsShooting);

        if (activeIdx == -1) 
            activeIdx = Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) ? 0 :
                        Input.GetMouseButton(1) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1) ? 1 : -1;

        if (activeIdx == -1 || activeIdx >= _unit.Abilities.Count) return;

        Ability activeAbility = _unit.Abilities[activeIdx];
        _unit.State.CurrentAbility = activeAbility;

        PositionArgs turretArgs = new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward);
        PositionArgs firePointArgs = new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward);

        if (Input.GetMouseButtonUp(activeIdx))
        {
            activeAbility.Release();
        }
        else if (Input.GetMouseButton(activeIdx)) 
        {
            activeAbility.Hold(turretArgs, firePointArgs, dt);

            if (activeAbility.CanShoot && !activeAbility.IsBlocked)
            {
                activeAbility.Fire(turretArgs, firePointArgs, _unit);
                activeAbility.ResetReloadProgress();
            }
        }
    }
    private void HandleWeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            InventoryManager.instance.SetWeapon(_unit, _unit.Stats, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            InventoryManager.instance.SetWeapon(_unit, _unit.Stats, 1);
        }
    }
    private Vector3 ConvertToCameraSpace(Vector3 input)
    {
        Vector3 forward = Camera.transform.forward;
        Vector3 right = Camera.transform.right;
        forward.y = 0; right.y = 0;

        forward.Normalize();
        right.Normalize();

        return Vector3.ClampMagnitude(forward * input.z + right * input.x, 1f);
    }

    public override void OnDeath()
    {
        _unit.OnHealthIsZero -= _unit.Die;
        Camera.Die();

        Registerer.UnregisterUpdatable(this);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.linearVelocity = pushDir * PushPower;
    }
}
