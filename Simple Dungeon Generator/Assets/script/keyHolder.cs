using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LockAndKeyGeneration;

public class keyHolder : MonoBehaviour, LockAndKeyType
{
    public LockAndKey Key;

    void LockAndKeyType.InteractLockAndKey(List<LockAndKey> keys, GameObject interact_Go)
    {
        keys.Add(Key);
        Destroy(gameObject);

        return;
    }

    private void Start()
    {
        if (!Key.gen)
        {
            Destroy(gameObject);
        }
    }
}
