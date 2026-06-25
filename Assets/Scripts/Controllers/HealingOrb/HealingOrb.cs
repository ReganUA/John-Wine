using System.Collections;
using UnityEngine;

public class HealingOrb : Controller, IUpdatable
{
    Unit playerTarget;
    private Vector3 startPos;
    void Start()
    {
        _unit = GetComponent<Unit>();

        Registerer.RegisterUpdatable(this);
        _unit.OnSpawn(null);
        startPos = transform.position;
    }
    public override void OnStart()
    {
        StartCoroutine(LaterLoad());
    }
    public void OnUpdate(float deltaTime)
    {
        if (playerTarget != null)
            _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, playerTarget.transform.position, deltaTime);

        Float();
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
    private IEnumerator LaterLoad()
    {
        yield return new WaitForEndOfFrame();
        playerTarget = GameManager.instance.player;
    }
    private void Float()
    {
        float newY = startPos.y + (Mathf.Sin(Time.time * 2f) * 0.15f);

        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
