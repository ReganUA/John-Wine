using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Unit player;
    void Awake()    
    {
        instance = this;
    }
}
