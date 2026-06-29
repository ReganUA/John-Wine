
using UnityEngine;

public class DionysusController : Controller, IUpdatable
{
    public Transform FirePoint;

    private EnemyPathfinding _pf;
    private Animator _anim;
    [SerializeField] private GameObject titles;
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
        //_unit.ChangeAbility(Random.Range(1, 3));

        Ability curAbility = _unit.State.CurrentAbility;

        // 1. Отримуємо базовий вектор руху гравця (X та Z)
        Vector3 playerBaseVelocity = GameManager.instance.player.State.MoveState.CurrentMoveDirection * GameManager.instance.player.State.MoveState.CurrentSpeed;
        Vector3 playerTotalVelocity = playerBaseVelocity + GameManager.instance.player.State.MoveState.ExternalForcesVelocity;

        // 2. Швидкість снаряда ворога
        float projectileSpeed = curAbility.config.LaunchComponents.UnitSpawner._prefab.UnitSO.SimComponents.Movers.Mover.Stats.MaxSpeed;
        if (projectileSpeed <= 0) projectileSpeed = 10f;

        // 3. Рахуємо час польоту снаряда до гравця
        float distance = Vector3.Distance(_unit.transform.position, GameManager.instance.player.transform.position);
        float timeToTarget = distance / projectileSpeed;

        // ... (Верхня частина HandleWeapon з projectileSpeed та timeToTarget залишається такою ж)

        // 4. Отримуємо силу гравітації гравця
        float grav = GameManager.instance.player.Stats.GetStats(GameManager.instance.player.UnitSO.SimComponents.Movers.Mover).Gravity;

        // 5. Розрахунок вертикального упередження (Y)
        float finalYOffset = 0f;

        // Перевіряємо стан заземлення гравця
        bool isGrounded = GameManager.instance.player.Refs.CC.isGrounded;

        if (isGrounded || Mathf.Abs(playerTotalVelocity.y - (-2f)) < 0.05f)
        {
            // Гравець на землі — стріляємо без вертикальних зміщень
            finalYOffset = 0f;
        }
        else
        {
            // Гравець у повітрі.
            // Оскільки твій HandleGravity вже застосовує гравітацію до playerTotalVelocity.y кожен кадр,
            // для передбачення позиції на короткий час польоту (timeToTarget) 
            // правильна лінійна екстраполяція поточної швидкості дає набагато точніший результат!

            float realVerticalVelocity = playerTotalVelocity.y;

            // Якщо гравець щойно відірвався від землі, але в швидкості завис старий притиск
            if (realVerticalVelocity < -1.9f && realVerticalVelocity > -2.1f)
            {
                realVerticalVelocity = 0f;
            }

            // Враховуємо чисту середню швидкість підйому/падіння за час польоту снаряда
            // Додаємо + (0.5f * grav * timeToTarget), щоб компенсувати те, що швидкість гравця ВЖЕ падає в HandleGravity
            finalYOffset = (realVerticalVelocity + (0.5f * grav * timeToTarget)) * timeToTarget;
        }

        // 6. Збираємо фінальну точку пострілу (чітко в центр тіла)
        Vector3 targetCenterPos = GameManager.instance.player.transform.position + Vector3.up * 1f;

        Vector3 targetPointPos = new Vector3(
            targetCenterPos.x + (playerTotalVelocity.x * timeToTarget),
            targetCenterPos.y + finalYOffset,
            targetCenterPos.z + (playerTotalVelocity.z * timeToTarget)
        );

        // 7. Напрямок від дула до цілі
        Vector3 directionToTargetPoint = targetPointPos - FirePoint.position;
        Quaternion fireRotation = Aim(directionToTargetPoint);

        // ... (Далі твій виклик Fire)

        // Передаємо правильні аргументи у Fire
        _unit.State.CurrentAbility.Fire(
            new PositionArgs(_unit.Turret.position, _unit.Turret.rotation, _unit.Turret.forward),
            new PositionArgs(FirePoint.position, fireRotation, directionToTargetPoint.normalized), // forward міняємо на чистий напрямок
              _unit);
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

        titles.SetActive(true);
        gameObject.SetActive(false);
    }
    private void UpdateAnimation()
    {
        _anim.SetFloat("Speed", _unit.State.MoveState.CurrentSpeed);
    }
}
