using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LockAndKeyGeneration;

public class PlayerPickUpKey : MonoBehaviour
{
    RaycastHit hit;
    List<LockAndKey> lockAndKeys;
    [SerializeField] LayerMask layer;
    // Start is called before the first frame update
    void Start()
    {
        if(lockAndKeys == null)
        {
            lockAndKeys = new List<LockAndKey>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 2.5f, layer))
            {
                if (hit.collider != null)
                {
                    LockAndKeyType kh = hit.collider.gameObject.GetComponent<LockAndKeyType>();
                    if (kh != null)
                    {
                        kh.InteractLockAndKey(lockAndKeys, gameObject);
                    }
                }
            }
        }
    }
}
