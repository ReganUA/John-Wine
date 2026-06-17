using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    void Awake()    
    {
        instance = this;
    }
}
