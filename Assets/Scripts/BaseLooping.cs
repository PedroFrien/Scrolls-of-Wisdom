using UnityEngine;

public abstract class BaseLooping : MonoBehaviour
{
    protected Vector3 spawnPosition;
    protected Quaternion spawnRotation;

    public virtual void SetSpawnPoint(Vector3 position, Quaternion rotation)
    {
        spawnPosition = position;
        spawnRotation = rotation;
    }

    public virtual void Reset()
    {
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        SpecificReset();
    }

    public abstract void SpecificReset();


}
