using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Generator2D;
using static LockAndKeyGeneration;

public class roomGenerate : MonoBehaviour
{
    public roomObject room;

    public int grid;

    List<GameObject> room_gen_floor;

    List<GameObject> room_gen_object;

    public Vector3Int level;

    public Vector2Int loc;

    public int randomSeed;

    public int lightInterval = 0;

    public List<door_switch> doors;

    [HideInInspector] public Room G2Room;

    public int key;

    public int _lock;

    objectGenerator ogen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool door(int i, int j)
    {
        foreach(door d in room.doorObj)
        {

            if(i == d.doorPos.x && j == d.doorPos.y)
            {
                return true;
            }
        }


        return false;
    }

    public void block()
    {
        if(room_gen_floor != null)
        {
             
            foreach(GameObject go in room_gen_floor)
            {
                objectGenerator goge = go.GetComponent<objectGenerator>();
                if(goge != null) { goge.build_wallblock(); }
            }
        }
    }

    public void rseed(int rseed)
    {
        //test
        Random.InitState(rseed); ;

        //tst

        //test
    }

    public void ClearScene()
    {
        if(level.x == 1 || level.y == 1)
        {
            return;
        }

        while (transform.childCount > 0)
        {
            foreach (Transform t in transform)
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }

    bool useLight(int i, int j)
    {

        bool booli = false;

        if ((i + 1 <= room.roomSize.x / 2f && i % lightInterval == 0) || (i + 1 > room.roomSize.x / 2f && (room.roomSize.x - i + 1) % lightInterval == 0))
        {
            booli = true;
        }

        bool boolj = false;

        if((j + 1 <= room.roomSize.y / 2f && j % lightInterval == 0) || (j + 1 > room.roomSize.y / 2f && (room.roomSize.y - j + 1) % lightInterval == 0))
        {
            boolj = true;
        }

        if(i == 0 || i == room.roomSize.x - 1)
        {
            if(j == 0 || j == room.roomSize.y - 1 || boolj)
            {
                return true;
            }
            return false;
        }
        else if(j == 0 || j == room.roomSize.y - 1)
        {
            if (i == 0 || i == room.roomSize.x - 1 || booli)
            {
                return true;
            }
            return false;
        }

        return false;
    }

    public void Gen()
    {
        //tst_test
        room_gen_floor = new List<GameObject>();

        room_gen_object = new List<GameObject>();



        GameObject decorat = new GameObject("decorate");
        decorat.transform.parent = transform;
        decorat.transform.localPosition = Vector3.zero;

        GameObject wall = new GameObject("wall");
        wall.transform.parent = transform;
        wall.transform.localPosition = Vector3.zero;

        GameObject ceil = new GameObject("ceil");
        ceil.transform.parent = transform;
        ceil.transform.localPosition = Vector3.zero;


        GameObject _floor = new GameObject("floors");
        _floor.transform.parent = transform;
        _floor.transform.localPosition = Vector3.zero;

        room.style.setObjectSetCount();

        if (level.x == 0)
        {

            for (int i = 0; i < room.roomSize.x; i++)
            {
                for (int j = 0; j < room.roomSize.y; j++)
                {

                    DgGo floor = room.style.getObject(style.ListName.floors);

                    if(floor == null) { continue; }

                    GameObject floor_obj = Instantiate(floor.go, new Vector3(transform.position.x + i * grid, transform.position.y + 0.1f, transform.position.z + j * grid), Quaternion.identity, _floor.transform);

                    objectGenerator floor_obj_ge = floor_obj.GetComponent<objectGenerator>();



                    if(useLight(i,j)) 
                    {
                        floor_obj_ge.isLight = true;
                    }

                    floor_obj_ge.wallcheck = new string[] { "._0,0", "._0,0", "._0,0", "._0,0" };

                    if (i + 1 == room.roomSize.x && door(i + 1, j))
                    {
                        floor_obj_ge.wallcheck[1] = "door_" + (i + 1) + "," + j;
                        
                    }

                    if (i - 1 == -1 && door(i - 1, j))
                    {
                        floor_obj_ge.wallcheck[3] = "door_" + (i - 1) + "," + j;
                    }

                    if (j + 1 == room.roomSize.y && door(i, j + 1))
                    {
                        floor_obj_ge.wallcheck[0] = "door_" + i + "," + (j + 1);
                    }

                    if (j - 1 == -1 && door(i, j - 1))
                    {
                        floor_obj_ge.wallcheck[2] = "door_" + i + "," + (j - 1);
                    }


                    floor_obj_ge.decorate = decorat.transform;

                    floor_obj_ge.style_decoration = room.style;

                    floor_obj_ge.wall = wall.transform;
                    floor_obj_ge.ceil = ceil.transform;

                    room_gen_floor.Add(floor_obj);
                }
            }
        }

        if(level.y == 0)
        {
            foreach (GameObject floor in room_gen_floor)
            {


                //testtest_tst
                objectGenerator og = floor.GetComponent<objectGenerator>();

                if (og == null)
                {
                    continue;
                }

                og.style_decoration = room.style;

                foreach (GameObject t in og.wall_GenerateObj())
                {
                    room_gen_object.Add(t);
                }
            }



            //room_gen_object.AddRange(room_gen_floor);


            foreach (GameObject t in room_gen_object)
            {
                objectGenerator og_o = t.GetComponent<objectGenerator>();
                if (og_o == null)
                {
                    continue;
                }

                og_o.style_decoration = room.style;

                og_o.decorate = decorat.transform;
                og_o.wall = wall.transform;
                og_o.ceil = ceil.transform;

                og_o.Decorate_GenerateObj();
            }
        }

        if(level.z == 0)
        {
            Debug.Log("?");
            ogen = GetComponent<objectGenerator>();
            ogen.style_decoration = room.style;
            Debug.Log(ogen.style_decoration);
            ogen.decorate = decorat.transform;

            List<DgGo> objs = new List<DgGo>();

            float targetsize = room.roomSize.x * room.roomSize.y * room.style.fill_rate;

            float size = 0;

            foreach (DgGo go in room.style.must_obj)
            {
                GameObject _go = go.go;

                BoxCollider box_collider = _go.GetComponent<BoxCollider>();
                for (int i = 0; i < go.weight; i++)
                {
                    size += box_collider.size.x * _go.transform.localScale.x * box_collider.size.y * _go.transform.localScale.y;

                    objs.Add(go);
                }



            }

            if (room.style.objectSet != null)
            {
                if (room.style.objectSet.Length > 0)
                {
                    while (size < targetsize)
                    {
                        DgGo obj = room.style.getObject(style.ListName.objectSet);

                        if (obj == null)
                            continue;

                        GameObject _obj = obj.go;
                        BoxCollider box_collider = _obj.GetComponent<BoxCollider>();

                        size += box_collider.size.x * _obj.transform.localScale.x * box_collider.size.y * _obj.transform.localScale.y;

                        objs.Add(obj);

                    }
                }
            }

            foreach (DgGo obj in objs)
            {
                int trys = 0;

                while (trys < 100)
                {
                    Vector3 pos = new Vector3(Random.Range(0, grid * room.roomSize.x) - grid / 2f, 0.1f, Random.Range(0, grid * room.roomSize.y) - grid / 2f);

                    Vector3 localpos = decorat.transform.TransformPoint(pos);

                    if (ogen.Instantiate_check(obj, localpos, false, "object", Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)))
                    {
                        break;
                    }
                    trys++;
                }
            }


            List<DgGo> objs1 = new List<DgGo>();

            float targetsize1 = room.roomSize.x * room.roomSize.y * room.style.other_fill_rate;

            float size1 = 0;

            foreach (DgGo go in room.style.must_other_obj)
            {
                GameObject _go = go.go;

                BoxCollider box_collider = _go.GetComponent<BoxCollider>();
                for (int i = 0; i < go.weight; i++)
                {
                    size1 += box_collider.size.x * _go.transform.localScale.x * box_collider.size.y * _go.transform.localScale.y;

                    objs.Add(go);
                }



            }

            if (room.style.otherObject != null)
            {
                if (room.style.otherObject.Length > 0)
                {
                    while (size1 < targetsize1)
                    {
                        DgGo obj = room.style.getObject(style.ListName.otherObject);

                        if (obj == null)
                            continue;

                        GameObject _obj = obj.go;
                        BoxCollider box_collider = _obj.GetComponent<BoxCollider>();

                        size1 += box_collider.size.x * _obj.transform.localScale.x * box_collider.size.y * _obj.transform.localScale.y;

                        objs1.Add(obj);

                    }
                }
            }

            foreach (DgGo obj in objs1)
            {
                int trys = 0;

                while (trys < 100)
                {
                    Vector3 pos = new Vector3(Random.Range(0, grid * room.roomSize.x) - grid / 2f, 0.1f, Random.Range(0, grid * room.roomSize.y) - grid / 2f);

                    Vector3 localpos = decorat.transform.TransformPoint(pos);

                    if (ogen.Instantiate_check(obj, localpos, true, "other", Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)))
                    {
                        break;
                    }
                    trys++;
                }
            }
        }

    }

    public void Gen_LockAndKey(DungeonNode dungeon, List<DungeonNode> dungeonNodes, List<DungeonNode> dungeonNodes1)
    {
        bool isLock = false;

        if (dungeon._lock != null)
        {
            if (dungeon._lock.isOneWay)
            {
                Room find_room = null;
                int room1 = dungeonNodes.FindIndex(x => x.selfRoom == dungeon.selfRoom);
                int room2 = dungeonNodes1.FindIndex(x => x.selfRoom == dungeon.selfRoom);

                for (int i = room2; i > 1; i--)
                {

                    if(dungeon.connect_rooms.FindIndex(x => x.Equals(dungeonNodes1[i].selfRoom)) >= 0)
                    {
                        find_room = dungeonNodes1[i].selfRoom;
                        break;
                    }
                }

                if(find_room != null)
                {
                    foreach (door_switch door in doors)
                    {
                        foreach (door d in room.doorTemp)
                        {
                            if(d.connects != null)
                            {
                                if (d.connects.Find(x => x.room.Equals(find_room)) != null && door.loc == new Vector2Int(d.doorPos.x - loc.x, d.doorPos.y - loc.y))
                                {

                                    door.setOneWayDoor();
                                    isLock = true;
                                }
                            }
                            
                        }

                    }
                }
                else
                {

                    isLock = false;
                    
                }
                
            }
            else
            {
                foreach (door_switch door in doors)
                {

                    if (door.setLock(dungeon._lock))
                    {

                        isLock = true;
                    }
                    
                }
            }

            if(!isLock && dungeon._lock.gen)
            {
                dungeon._lock.gen = false;
            }
        }

        if(dungeon.keys == null)
        {
            return;
        }

        foreach(LockAndKey key in dungeon.keys)
        {
            if (!room.can_spawn_object)
            {
                key.gen = false;
                continue;
            }
            if (key.isOneWay)
                break;
            int trys = 0;
            while (trys < 100)
            {
                Vector3 pos = new Vector3(Random.Range(0, grid * room.roomSize.x) - grid / 2f, 0f, Random.Range(0, grid * room.roomSize.y) - grid / 2f);

                BoxCollider boxCollider = room.style.key1.go.GetComponent<BoxCollider>();
                if(ogen == null) { ogen = GetComponent<objectGenerator>(); }
                if (Physics.OverlapBox(boxCollider.center + transform.TransformPoint(pos), boxCollider.size / 2, Quaternion.identity, ogen.objectLayer).Length == 0)
                {

                    GameObject place = Instantiate(room.style.key1.go, transform.TransformPoint(pos), Quaternion.identity, transform);
                    place.GetComponent<keyHolder>().Key = key;
                    break;
                }

                
                trys++;
            }

            if(trys == 100)
            {
                key.gen = false;
            }
        }
    }

    public void spawnPlayer(GameObject player)
    {
        int trys = 0;
        while (trys < 100)
        {
            Vector3 pos = new Vector3(Random.Range(0, grid * room.roomSize.x) - grid / 2f, 0f, Random.Range(0, grid * room.roomSize.y) - grid / 2f);

            BoxCollider boxCollider = player.GetComponent<BoxCollider>();
            if (ogen == null) { ogen = GetComponent<objectGenerator>(); }
            if (Physics.OverlapBox(boxCollider.center + transform.TransformPoint(pos), boxCollider.size / 2, Quaternion.identity, ogen.objectLayer).Length == 0)
            {

                Instantiate(player, transform.TransformPoint(pos), Quaternion.identity);

                break;
            }


            trys++;
        }

        if(trys == 100)
        {
            Instantiate(player, transform.TransformPoint(Vector3.zero), Quaternion.identity);
        }
    }
}
