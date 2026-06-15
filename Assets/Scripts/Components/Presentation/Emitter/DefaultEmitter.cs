using UnityEngine;
[CreateAssetMenu(fileName = "Default Emitter", menuName = "Components/Presentation/Emitter/Default Emitter")]
public class DefaultEmitter : EmitterSO
{
    public override ParticleSystem Emit(PositionArgs posArgs, Transform parent = null)
    {
        var spawned = Instantiate(_particleSystemPrefab, posArgs.position, posArgs.rotation);
        if (parent != null)
        {
            spawned.transform.SetParent(parent.transform);
        }
        return spawned;
    }
}
