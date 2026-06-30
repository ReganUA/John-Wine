using System.Collections;
using UnityEngine;

public class HealthUpgrade : Controller, IUpdatable
{
    [HideInInspector] Unit playerTarget;
    private Vector3 startPos;
    void Start()
    {
        _unit.OnSpawn(null);
    }
    public override void OnStart()
    {
        startPos = transform.position;
        StartCoroutine(LaterLoad());
        Registerer.RegisterUpdatable(this);
    }


    public void OnUpdate(float dt)
    {
        if (playerTarget != null)
            _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, playerTarget.transform.position, Time.deltaTime);

        Float();
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
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            UpdateMaxHealth();
            _unit.Die();
        }
    }
    private void UpdateMaxHealth()
    {
        playerTarget.Health.BuffAdd((new HealthStats() { MaxHealth = playerTarget.Health.Value.MaxHealth * 0.1f }));
    }
    public override void OnDeath()
    {
        Registerer.UnregisterUpdatable(this);
    }
}
