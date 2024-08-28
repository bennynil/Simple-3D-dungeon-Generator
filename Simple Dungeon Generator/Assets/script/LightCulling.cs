using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCulling : MonoBehaviour
{

    public GameObject lightGo;

    private void OnBecameVisible()
    {
        lightGo.SetActive(true);
    }

    private void OnBecameInvisible()
    {
        lightGo.SetActive(false);
    }
}
