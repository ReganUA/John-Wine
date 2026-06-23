using UnityEngine;

public class HealingOrb : Controller, IUpdatable
{
    Unit playerTarget; 
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
            OnHit(playerTarget);
        }
    }
    void OnHit(Unit hitUnit)
    {
        _unit.UnitSO.SimComponents.Effect.Affect(hitUnit, _unit.Stats);
        Registerer.UnregisterUpdatable(this);
        Destroy(gameObject);
    }
}
