using UnityEngine;
using UnityEngine.UI;

public class DamageEffecto : MonoBehaviour
{
    public static DamageEffecto instance;
    void Start()
    {
        instance = this;
    }
}
