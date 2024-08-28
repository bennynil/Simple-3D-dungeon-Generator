using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LockAndKeyGeneration;

public class door_switch : MonoBehaviour
{
    public GameObject _putdoor;

    public GameObject _putwall;

    public Transform floorcheck;


    public Transform _decorate;

    public Transform wall;

    public Transform ceil;

    public Vector2Int loc;

    [SerializeField] LayerMask layer;

    [SerializeField] int gridsize = 5;

    public LockAndKey doorlock;

    [SerializeField] bool Openable;

    roomGenerate rg;

    private void Start()
    {
        if (!isDoorValid())
        {

            useWall();

            if(_putwall != null)
            {
                objectGenerator ogenerator = _putwall.GetComponent<objectGenerator>();

                if (ogenerator != null)
                {
                    ogenerator.Decorate_GenerateObj();
                }
            }
        }

        if(doorlock != null)
        {
            if (!doorlock.gen)
            {
                doorlock = null;
            }
        }
    }

    public void useDoor()
    {
        _putwall.SetActive(false);
        _putdoor.SetActive(true);
    }

    public void useWall()
    {
        _putwall.SetActive(true);
        _putdoor.SetActive(false);
    }

    public bool isDoorValid()
    {
        Collider[] check_if_collide = Physics.OverlapBox(transform.position + transform.rotation * new Vector3(0f, 0f, gridsize / 2), new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, layer);

        Collider[] check_if_collide_2 = Physics.OverlapBox(transform.position + transform.rotation * new Vector3(0f, 0f, -gridsize / 2), new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, layer);


        return check_if_collide.Length + check_if_collide_2.Length >= 2;
    }

    public void decorate()
    {
        objectGenerator tg = _putwall.GetComponent<objectGenerator>();
        if (tg != null)
        {
            tg.decorate = _decorate;
            tg.wall = wall;


            tg.ceil = ceil;

            _putwall.GetComponent<objectGenerator>().Decorate_GenerateObj();
        }
    }

    public bool setLock(LockAndKey _lock)
    {
        if (Openable && isDoorValid())
        {
            Debug.Log(transform.parent.parent.name);
            doorlock = _lock;

            return true;
        }

        return false;
    }

    public bool interactInRoom(GameObject interact_object)
    {
        Transform pp = transform.parent.parent;
        if(pp != null && rg == null) { rg = pp.gameObject.GetComponent<roomGenerate>(); }
        Vector3 point = new Vector3(pp.position.x - rg.grid / 2f, pp.position.y, pp.position.z + rg.grid / 2f);
        Vector3 point1 = new Vector3(pp.position.x + rg.room.roomSize.x * rg.grid, pp.position.y, pp.position.z + rg.room.roomSize.y * rg.grid);
        Vector3 pos = interact_object.transform.position;


        return pos.x >= point.x && pos.x <= point1.x && pos.y >= point.y && pos.z >= point.z && pos.z <= point1.z;
    }

    public bool setOneWayDoor()
    {
        if (isDoorValid())
        {

            return true;
        }

        return false;
    }
}
