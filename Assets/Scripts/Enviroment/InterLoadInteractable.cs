using UnityEngine;

public class InterLoadInteractable : MonoBehaviour
{
    public string uniqueID; //manual
    void Start()
    {
        if (StateManager.InteractableStates.TryGetValue(uniqueID, out bool isActivated))
        {
            if (isActivated)
            {
                gameObject.GetComponent<InteractiveBaseEnviroment>().onInteract.Invoke();
            }
        }
    }

    public void OnInteractSave()
    {
        StateManager.InteractableStates[uniqueID] = true;
    }
}
