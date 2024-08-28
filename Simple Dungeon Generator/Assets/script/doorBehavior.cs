using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LockAndKeyGeneration;

public class doorBehavior : MonoBehaviour, LockAndKeyType
{
    bool isLock;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;
    [SerializeField] Vector3 position1;
    [SerializeField] Vector3 rotation1;
    [SerializeField] door_switch door_Switch;
    [SerializeField] LockAndKey Lock;










    float currentLerpTime;
    [SerializeField] float lerpTime;
 

    bool open;

    // Start is called before the first frame update
    void Start()
    {
        if(Lock == null)
        {
            if(door_Switch != null)
            {
                Lock = door_Switch.doorlock;
                if (Lock.gen)
                {
                    isLock = true;
                }
                else
                {
                    isLock = false;
                }
            }
            else
            {
                isLock = false;
            }
        }

    }

    void LockAndKeyType.InteractLockAndKey(List<LockAndKey> lks, GameObject go)
    {
        bool t = false;
        if(door_Switch != null)
        {
            if(Lock.isOneWay && door_Switch.interactInRoom(go))
            {
                t = true;



            }



        }

        if(lks.FindIndex(x => x.equ(Lock)) != -1 || !isLock || t)
        {
            isLock = false;
            open = !open;
            currentLerpTime = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            if(transform.localPosition != position || transform.localRotation != Quaternion.Euler(rotation))
            {
                currentLerpTime += Time.deltaTime;
                float t = currentLerpTime / lerpTime;

                transform.localPosition = Vector3.Lerp(transform.localPosition, position, t);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotation), t);
            }
        }
        else
        {
            if(transform.localPosition != position1 || transform.localRotation != Quaternion.Euler(rotation1))
            {
                currentLerpTime += Time.deltaTime;
                float t = currentLerpTime / lerpTime;

                transform.localPosition = Vector3.Lerp(transform.localPosition, position1, t);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotation1), t);
            }
        }
    }
}
