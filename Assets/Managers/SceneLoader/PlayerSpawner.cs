using UnityEngine;
using UnityEngine.Events;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPosBySceneID;
    public UnityEvent[] Interact;
    void Awake()
    {
        SpawnPlayer();
    }
    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, spawnPosBySceneID[TransitionData.PreviousSceneName]);
        player.transform.SetParent(null);

        if (Interact[TransitionData.PreviousSceneName] != null)
        {
            Interact[TransitionData.PreviousSceneName].Invoke();
        }
    }
}
