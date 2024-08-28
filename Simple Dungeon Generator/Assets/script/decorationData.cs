using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decorationData : MonoBehaviour
{
    public Transform _wall;
    public Transform _nearWall;

    public Vector3 setPivot(string pivot)
    {
        if(pivot == "object" || pivot == "other")
        {
            return new Vector3(0, 0, 0);
        }
        else if(pivot == "wall")
        {
            return  -_wall.localPosition;

        }
        else if(pivot == "nearWall")
        {
            return -_nearWall.localPosition;
        }


        return new Vector3(0, 0, 0);
    }
}
