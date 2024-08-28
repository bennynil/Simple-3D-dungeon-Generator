using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collider_switch : MonoBehaviour
{
    public BoxCollider box;

    public List<Collider> colliders;
    void Start()
    {
        if(box != null)
            box.enabled = false;

        if(colliders == null){ return; }
        foreach(Collider c in colliders)
        {
            c.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
