using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    private Animator _anim;
    private Ability _currentLeftAbility;
    private Ability _currentRightAbility;

    [SerializeField] private Image damageEffect;
    [SerializeField] private float damageEffectTime;

    private void Start() => _unit.OnSpawn();

    public override void OnStart()
    {
        _unit.OnHealthIsZero += _unit.Die;
        Registerer.RegisterUpdatable(this);
        _moveStats = _unit.Stats.GetStatsModifiable(_unit.UnitSO.SimComponents.Movers.Mover);

        GameManager.instance.player = _unit;
        _anim = GetComponentInChildren<Animator>();

        RefreshCurrentWeapons();

        if (DamageEffecto.instance != null)
        {
            damageEffect = DamageEffecto.instance.GetComponent<Image>();
            damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, 1f);
        }
        else
        {
            StartCoroutine(LaterLoad());
        }
    }

    public void RefreshCurrentWeapons()
    {
        _currentLeftAbility = (_unit.Abilities.Count > 0) ? _unit.Abilities[0] : null;
        _currentRightAbility = (_unit.Abilities.Count > 1) ? _unit.Abilities[1] : null;

        _unit.State.CurrentAbility = _currentLeftAbility;

        // Вимога 3: При перемиканні на інший сет (1, 2, 3) — скидаємо прогрес перезарядки обох абілок
        _currentLeftAbility?.ResetReloadProgress();
        _currentRightAbility?.ResetReloadProgress();
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
        DamageEffect();

        _unit.UnitSO.SimComponents.Movers.Mover.Move(_unit, moveDir, dt);

        // Обидві абілки завжди оновлюють свій таймер у фоні
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
        // === ЛОГІКА ЛІВОЇ КНОПКИ МИШІ (ЛКМ) ===
        if (_currentLeftAbility != null && Input.GetMouseButton(0))
        {
            // Вимога 2: Якщо щойно натиснули ЛКМ, змушуємо ПКМ скинути свою перезарядку
            if (Input.GetMouseButtonDown(0))
            {
                _currentRightAbility?.ResetReloadProgress();
            }

            _unit.State.CurrentAbility = _currentLeftAbility;
            _currentLeftAbility.Hold(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), dt);

            if (_currentLeftAbility.CanShoot && !_currentLeftAbility.IsBlocked)
            {
                UpdateAnimation();
                _currentLeftAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
                _currentLeftAbility.ResetReloadProgress();
            }
        }
        // === ЛОГІКА ПРАВОЇ КНОПКИ МИШІ (ПКМ) ===
        // Вимога 1: Використовуємо else if. Якщо затиснуто ЛКМ, цей блок фізично не виконається
        else if (_currentRightAbility != null && Input.GetMouseButton(1))
        {
            // Вимога 2: Якщо щойно натиснули ПКМ, змушуємо ЛКМ скинути свою перезарядку
            if (Input.GetMouseButtonDown(1))
            {
                _currentLeftAbility?.ResetReloadProgress();
            }

            _unit.State.CurrentAbility = _currentRightAbility;
            _currentRightAbility.Hold(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), dt);

            if (_currentRightAbility.CanShoot && !_currentRightAbility.IsBlocked)
            {
                UpdateAnimation();
                _currentRightAbility.Fire(new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward), new PositionArgs(FirePoint.position, FirePoint.rotation, FirePoint.forward), _unit);
                _currentRightAbility.ResetReloadProgress();
            }
        }

        // Звичайний виклик Release при відпусканні кнопок
        if (Input.GetMouseButtonUp(0) && _currentLeftAbility != null)
        {
            _currentLeftAbility.Release();
        }
        if (Input.GetMouseButtonUp(1) && _currentRightAbility != null)
        {
            _currentRightAbility.Release();
        }
    }

    private void HandleWeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) InventoryManager.instance.SetWeapon(_unit, _unit.Stats, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) InventoryManager.instance.SetWeapon(_unit, _unit.Stats, 1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) InventoryManager.instance.SetWeapon(_unit, _unit.Stats, 2);
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

    public void DamageEffect()
    {
        if (damageEffect == null) return;

        float healthPercent = _unit.State.HealthState.HealthDelta;
        damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, 1f - healthPercent);
    }

    public override void OnDeath()
    {
        _unit.OnHealthIsZero -= _unit.Die;
        Camera.Die();

        Registerer.UnregisterUpdatable(this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.linearVelocity = pushDir * PushPower;
    }

    private IEnumerator LaterLoad()
    {
        yield return new WaitForEndOfFrame();
        damageEffect = DamageEffecto.instance.GetComponent<Image>();
        damageEffect.color = new Color(damageEffect.color.r, damageEffect.color.g, damageEffect.color.b, 1f);
    }

    private void UpdateAnimation()
    {
        _anim.SetTrigger("Attack");
    }
}