using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapFix : MonoBehaviour
{
    public string _tag;
    void Start()
    {
        BoxCollider box1 = GetComponent<BoxCollider>();
        Collider[] lap = Physics.OverlapBox(transform.position + scaleObj(box1.center, transform.localScale), scaleObj(box1.size / 2, transform.localScale), transform.rotation);
        if(_tag != "")
        {
            foreach(Collider c in lap)
            {
                if(c.gameObject != gameObject)
                {   
                    BoxCollider boxCollider = c.GetComponent<BoxCollider>();
                    if(boxCollider != null)
                    {
                        if(Vector3.Distance(c.transform.position, transform.position) <= 1.5f)
                        {
                            LapFix k = c.gameObject.GetComponent<LapFix>();
                            if (k != null)
                            {
                                if (_tag == k._tag)
                                {
                                    Destroy(c.gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    Vector3 scaleObj(Vector3 box, Vector3 size)
    {
        return new Vector3(box.x * size.x, box.y * size.y, box.z * size.z);
    }
}
