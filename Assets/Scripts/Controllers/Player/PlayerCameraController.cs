using UnityEngine;

public sealed class PlayerCameraController : Controller, IUpdatable
{
    [SerializeField] private Transform playerCamera;
    private ItemSys itemSys;
    private void Start() => _unit.OnSpawn();
    public override void OnStart()
    {
        itemSys = GetComponent<ItemSys>();
        Registerer.RegisterUpdatable(this);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void OnUpdate(float deltaTime)
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector3 cameraInput = new Vector3(mouseX, mouseY, 0f);

        _unit.UnitSO.SimComponents.Movers.RotationalMover.Move(_unit, cameraInput, deltaTime);

        HandleInteraction();
    }
    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider target = _unit.UnitSO.SimComponents.Raycaster.Raycast(_unit.Stats, transform.position, transform.forward).collider;
            if (target != null)
            {
                if (target.TryGetComponent(out Unit u))
                {
                    if (u.UnitSO.Tags.HasFlag(Tags.Key))
                    {
                        itemSys.AddItem(target.gameObject);
                        target.gameObject.SetActive(false);
                    }                    
                } else
                {
                    InteractiveBaseEnviroment enviromentTarget = target.GetComponent<InteractiveBaseEnviroment>();
                    if (target != null)
                    {
                        if (enviromentTarget.isInteractable == false) return;

                        if (enviromentTarget.key == null)
                        {
                            enviromentTarget.Interact();
                            return;
                        }

                        if (itemSys.GetKey(enviromentTarget) == true)
                        {
                            enviromentTarget.Interact();
                        }
                    }
                }
                
            }          
        }
    }
    public override void OnDeath()
    {
        Registerer.UnregisterUpdatable(this);
    }
}
