using UnityEngine;

public class HealingOrb : Controller, IUpdatable
{
    GameObject playerTarget; 
    void Start()
    {
        _unit = GetComponent<Unit>();

        Registerer.RegisterUpdatable(this);
        _unit.OnSpawn(null);
    }
    public override void OnStart()
    {
        playerTarget = GameManager.instance.player;
    }
    public void OnUpdate(float deltaTime)
    {
       _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, playerTarget.transform.position, deltaTime);
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.TryGetComponent(out Unit unit);
            OnHit(unit);
        }
    }
    void OnHit(Unit hitUnit)
    {
        _unit.UnitSO.SimComponents.Effect.Affect(hitUnit, _unit.Stats);
        Registerer.UnregisterUpdatable(this);
        Destroy(gameObject);
    }
}
