using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
[CreateAssetMenu]
public class style : ScriptableObject
{
    [SerializeField] public float fill_rate;
    [SerializeField] public float other_fill_rate;
    [SerializeField] public float side_fill_rate;

    //test
    float[] counts = new float[10];
    //test
    //tst

    [SerializeField] public DgGo[] objectSet;

    [SerializeField] public DgGo[] wallObjectSet;

    [SerializeField] public DgGo[] nearWallObjectSet;

    [SerializeField] public DgGo[] onHallwayObjectSet;

    [SerializeField] public DgGo[] otherObject;

    [SerializeField] public DgGo[] floors;

    [SerializeField] public DgGo[] ceilings;

    [SerializeField] public DgGo[] walls;

    [SerializeField] public DgGo[] wallLights;

    [SerializeField] public DgGo[] doors;

    [SerializeField] public DgGo[] doorLights;

    [SerializeField] public DgGo[] must_obj;
    [SerializeField] public DgGo[] must_other_obj;

    [SerializeField] public DgGo key1;
    public enum ListName
    {
        objectSet = 0,

        wallObjectSet = 1,

        nearWallObjectSet = 2,

        otherObject = 3,

        floors = 4,

        ceilings = 5,

        walls = 6,

        wallLights = 7,

        doors = 8,

        doorLights = 9,

        onHallway = 10

    }


    public void setObjectSetCount()
    {

        if(counts == null)
        {
            counts = new float[11];
        }
        else if(counts.Length != 11)
        {
            counts = new float[11];
        }

        if (counts.Sum() == 0f)
        {

            counts[0] = Count(objectSet);

            counts[1] = Count(wallObjectSet);

            counts[2] = Count(nearWallObjectSet);

            counts[3] = Count(objectSet);

            counts[4] = Count(floors);

            counts[5] = Count(ceilings);

            counts[6] = Count(walls);

            counts[7] = Count(wallLights);

            counts[8] = Count(doors);

            counts[9] = Count(doorLights);

            counts[10] = Count(onHallwayObjectSet);
        }
    }

    public float Count(DgGo[] objectSet)
    {
        float s = 0;

        if (objectSet == null)
            return 0f;

        if (objectSet.Length == 0)
            return 0f;

        foreach(DgGo go in objectSet)
        {
            s += go.weight;
        }

        return s;
    }

    public DgGo getObject(ListName list_enum)
    {
        DgGo[] DgGos = null;

        int list_index = (int)list_enum;
        
        switch (list_enum)
        {
            case ListName.objectSet:
                DgGos = objectSet;
                break;
            case ListName.wallObjectSet:
                DgGos = wallObjectSet;
                break;
            case ListName.nearWallObjectSet:
                DgGos = nearWallObjectSet;
                break;
            case ListName.otherObject:
                DgGos = otherObject;
                break;
            case ListName.floors:
                DgGos = floors;
                break;
            case ListName.ceilings:
                DgGos = ceilings;
                break;
            case ListName.walls:
                DgGos = walls;
                break;
            case ListName.wallLights:
                DgGos = wallLights;
                break;
            case ListName.doors:
                DgGos = doors;
                break;
            case ListName.doorLights:
                DgGos = doorLights;
                break;
            case ListName.onHallway:
                DgGos = onHallwayObjectSet;
                break;
        }

        if(DgGos == null) 
        {
            return null; 
        }

        DgGos.OrderBy(a => Random.Range(0, 20));

        float current_sum = 0;
        float targetsum = Random.Range(0f, counts[list_index]);

        float weightTmp = -1;

        DgGo weightTmpGo = null;

        foreach(DgGo go in DgGos)
        {
            current_sum += go.weight;

            if(go.weight > weightTmp)
            {
                weightTmp = go.weight;
                weightTmpGo = go;
            }

            if(current_sum >= targetsum)
            {
                return go;
            }
        }

        return weightTmpGo;
    }
}

[System.Serializable]
public class DgGo
{

    public GameObject go;
    public float weight;

    public DgGo()
    {
        this.go = null;
        this.weight = 0f;
    }

    public DgGo(GameObject go, float weight)
    {
        this.go = go;
        this.weight = weight;
    }
}
