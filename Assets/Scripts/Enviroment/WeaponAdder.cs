using System.Collections;
using UnityEngine;

public sealed class WeaponAdder : Controller
{
    public Unit playerTarget;
    public override void OnStart()
    {
       
    }
    void Start()
    {
        StartCoroutine(LaterLoad());
        _unit = GetComponent<Unit>();
    }
    private void AddWeapon(Unit target)
    {
        for (int i = 0; i < _unit.UnitSO.SimComponents.Abilities.Count; i++)
        {
            target.AddAbility(_unit.UnitSO.SimComponents.Abilities[i].CreateAbility(target.Stats));
            Debug.Log("AddedWeapon");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            AddWeapon(playerTarget);
    }
    void Update()
    {
        if (playerTarget != null)
        {
            _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, playerTarget.transform.position, Time.deltaTime);
        }
    }
    private IEnumerator LaterLoad()
    {
        yield return new WaitForEndOfFrame();
        playerTarget = GameManager.instance.player;
        Debug.Log(playerTarget);
    }
}