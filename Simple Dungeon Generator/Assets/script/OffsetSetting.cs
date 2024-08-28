using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetSetting : MonoBehaviour
{
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] Vector3 x;
    public Vector3 getOffset()
    {
        float xx = boxCollider.size.x * x.x * transform.localScale.x;
        float yy = boxCollider.size.y * x.y * transform.localScale.y;
        float zz = boxCollider.size.z * x.z * transform.localScale.z;
        return new Vector3 (xx, yy, zz);
    }
}
