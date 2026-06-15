using UnityEngine;

public abstract class EmitterSO : ScriptableObject
{
    [SerializeField] protected ParticleSystem _particleSystemPrefab;
    public abstract ParticleSystem Emit(PositionArgs posArgs, Transform parent = null);
}
