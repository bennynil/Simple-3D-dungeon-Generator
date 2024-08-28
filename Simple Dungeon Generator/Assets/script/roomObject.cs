using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Generator2D;

[CreateAssetMenu]
public class roomObject : ScriptableObject
{
    public style style;
    public GameObject room;

    public Vector2Int roomSize;
    public List<door> doorObj;
    public List<door> doorTemp;

    public int layer_of_room;
    public int layer_of_room_temp;
    public bool random;
    public bool can_spawn_object;
    public roomObject Clone()
    {
        roomObject obj = ScriptableObject.CreateInstance<roomObject>();
        obj.style = style;
        obj.roomSize = new Vector2Int(roomSize.x, roomSize.y);
        obj.doorObj = doorObj.ConvertAll(doo => doo.Clone());
        obj.layer_of_room = layer_of_room;
        

        obj.room = room;

        obj.random = random;

        obj.can_spawn_object = can_spawn_object;
        return obj;
    }
}

[System.Serializable]
public class door
{
    public Vector2Int doorPos;
    public int height;
    public bool connected;

    [System.NonSerialized]
    public Room room;
    [System.NonSerialized]
    public List<door> connects;
    public bool passage;

    public door(Vector2Int doorPos, int _height, bool p)
    {
        this.doorPos = doorPos;
        height = _height;
        passage = p;
    }

    public void add(door door)
    {
        if(connects == null)
            connects = new List<door>();
        connects.Add(door);
    }

    public door Clone()
    {
        return new door(new Vector2Int(doorPos.x,doorPos.y), height, passage);
    }
}
