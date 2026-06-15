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

        _unit.State.CurrentAbility.ReloadProgress(dt);
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

        if (_jumpBufferTime > 0 && _unit.Refs.CC.isGrounded == true)
        {
            _unit.State.MoveState.ExternalForcesVelocity.y = Mathf.Sqrt(_moveStats.Value.JumpForce * -2f * _moveStats.Value.Gravity);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && _coyoteTime > 0)
        {
            _unit.State.MoveState.ExternalForcesVelocity.y = Mathf.Sqrt(_moveStats.Value.JumpForce * -2f * _moveStats.Value.Gravity);
        }
    }
    private void HandleWeapon(float dt)
    {
        if (Input.GetMouseButton(0))
        {
            _unit.State.CurrentAbility.Hold(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), dt);

            if (_unit.State.CurrentAbility.CanShoot == false || _unit.State.CurrentAbility.IsBlocked) return;

            _unit.State.CurrentAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
            _unit.State.CurrentAbility.ResetReloadProgress();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _unit.State.CurrentAbility.Release();
        }
    }
    private void HandleWeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _unit.ChangeAbility(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _unit.ChangeAbility(1);
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
