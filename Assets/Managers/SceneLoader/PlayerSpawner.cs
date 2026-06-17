using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPosBySceneID;
    void Awake()
    {
        SpawnPlayer();
    }
    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, spawnPosBySceneID[TransitionData.PreviousSceneName]);
        player.transform.SetParent(null);
    }
}
