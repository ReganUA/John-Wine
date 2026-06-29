using System.Collections;
using UnityEngine;

public class HealthUpgrade : Controller
{
    [HideInInspector] Unit playerTarget;
    private Vector3 startPos;
    public override void OnStart()
    {
        _unit.OnSpawn(null);
        StartCoroutine(LaterLoad());
    }

    void Start()
    {
        _unit = GetComponent<Unit>();
        startPos = transform.position;
    }

    void Update()
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
        }
    }
    private void UpdateMaxHealth()
    {
        //playerTarget.Stats.GetStatsModifiable(playerTarget.UnitSO)
    }
}
