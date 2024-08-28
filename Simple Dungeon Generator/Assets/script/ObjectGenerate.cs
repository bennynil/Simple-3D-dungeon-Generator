using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static gameSetting;
using System.Linq;

public class objectGenerator : MonoBehaviour
{
    public LayerMask floor;
    public LayerMask objectLayer;
    public LayerMask other;

    public bool isLight;
    public bool hallway;

    public Transform Light;

    private float fill;

    [SerializeField] public style style_decoration;

    public string[] wallcheck;

    public List<GameObject> objectcheck;

    public List<door_switch> doors = new List<door_switch>();

    public Transform decorate;

    [HideInInspector] public Transform wall;

    [HideInInspector] public Transform ceil;

    public int cellSize;

    public float roomHeight;

    [SerializeField] objectGenerator door;

    public void setData(float _fill, style _style_decoration)
    {
        fill = _fill;
        style_decoration = _style_decoration;
    }

    public void rseed(int rseed)
    {
        Random.InitState(rseed);
    }

    public List<GameObject> wall_GenerateObj()
    {
        if(cellSize <= 0) { return null; }
        List<GameObject> _obj = new List<GameObject>();
        Vector2[] di = new Vector2[4] {Vector2.up * cellSize / 2f, Vector2.down * cellSize / 2f, Vector2.right * cellSize / 2f, Vector2.left * cellSize / 2};

        int[] de = new int[4] { 0, 2, 1, 3 };
        bool j = false;

        style_decoration.setObjectSetCount();
        for (int i = 0; i < 4; i++)
        {
            DgGo gameobj = style_decoration.getObject(style.ListName.walls);

            Collider[] check_if_collide = Physics.OverlapBox(transform.position + new Vector3(di[i].x, 0, di[i].y), new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, floor);
           
            

            if (wallcheck[de[i]].Split("_")[0] == "door")
            {

                DgGo tgen = isLight ? style_decoration.getObject(style.ListName.doorLights) : style_decoration.getObject(style.ListName.doors);
                if(tgen != null)
                {
                    GameObject door = Instantiate(tgen.go, transform.position + new Vector3(di[i].x, -0.1f, di[i].y), Quaternion.Euler(0f, de[i] * 90f, 0f));
                    door_switch s = door.GetComponent<door_switch>();

                    objectGenerator og = door.GetComponent<objectGenerator>();

                    if(og != null)
                    {
                        og.style_decoration = style_decoration;

                        if (hallway)
                        {
                            og.hallway = true;
                        }

                        if (!isLight || j)
                        {
                            if(og.Light != null)
                            {
                                DestroyImmediate(og.Light.gameObject);
                            }
                            
                        }
                    }

                    if (s != null)
                    {
                        s._decorate = decorate;
                        s.wall = wall;
                        s.ceil = ceil;
                        string[] ss = wallcheck[de[i]].Split("_")[1].Split(",");
                        int x = int.Parse(ss[0]);
                        int y = int.Parse(ss[1]);

                        s.loc = new Vector2Int(x, y);

                        roomGenerate rg = s.wall.parent.gameObject.GetComponent<roomGenerate>();

                        if(rg != null)
                        {
                            if(rg.doors != null)
                            {
                                rg.doors.Add(s);
                                
                            }
                            else
                            {
                                rg.doors = new List<door_switch>();
                                rg.doors.Add(s);






                            }
                        }


                        doors.Add(s);
                    }

                    if(!j && isLight && !hallway)
                    {
                        j = true;
                    }

                    door.transform.parent = wall;
                    door.name = name;
                }
            }
            else if (check_if_collide.Length == 1)
            {
                string name = "wallObject_" + i;
                if (!j && isLight && !hallway) { gameobj = style_decoration.getObject(style.ListName.wallLights);}
                if(gameobj != null)
                {
                    GameObject sideWall = Instantiate(gameobj.go, transform.position + new Vector3(di[i].x, -0.1f, di[i].y), Quaternion.Euler(0f, de[i] * 90f, 0f));

                    objectGenerator sideg = sideWall.GetComponent<objectGenerator>();

                    if (sideg != null)
                    {
                        sideg.decorate = decorate;
                        sideg.wall = wall;
                        sideg.ceil = ceil;

                        sideg.style_decoration = style_decoration;
                        if (hallway)
                        {
                            sideg.hallway = true;
                        }

                        if (!isLight || j)
                        {
                            if(sideg.Light != null)
                            {
                                DestroyImmediate(sideg.Light.gameObject);
                            }
                        }
                        
                    }

                    if (!j && isLight && !hallway)
                    {
                        j = true;
                    }

                    sideWall.transform.parent = wall;
                    sideWall.name = name;
                    _obj.Add(sideWall);
                }
                
            }


        }

        DgGo gametestobj = style_decoration.getObject(style.ListName.ceilings);



        if (gametestobj != null)
        {
            GameObject sideWall = Instantiate(gametestobj.go, transform.position + new Vector3(0, roomHeight, 0), Quaternion.Euler(0, 0, 180));
            sideWall.transform.parent = ceil;
            sideWall.layer = 0;
            sideWall.name = "ceilObject_";
        }




        return _obj;
    }

    public void build_wallblock()
    {
        foreach(door_switch door in doors)
        {
            if(!door.isDoorValid())
            {

                door.useWall();
                door.decorate();
            }
        }
    }

    public void Decorate_GenerateObj()
    {

        if(style_decoration == null)
        {
            style_decoration = door.style_decoration;
        }

        fill = style_decoration.side_fill_rate;
        style_decoration.setObjectSetCount();

        if(cellSize > 0)
        {
            foreach(GameObject go in objectcheck)
            {
                go.transform.localPosition = new Vector3(Random.Range(-cellSize / 2f, cellSize / 2f), 0f, Random.Range(-cellSize / 2f, cellSize / 2f));
            }
        }

        objectcheck = objectcheck.OrderBy(x => Random.Range(0, objectcheck.Count())).ToList();

        foreach(GameObject obj in objectcheck)
        {
            



            if(Random.Range(0f,1f) < fill)
            {

                if (obj != null)
                {
                    

                    Quaternion offsetr = Quaternion.Euler(0, 180, 0);

                    if (obj.tag == "OBJECT")
                    {

                        Instantiate_check(style_decoration.getObject(style.ListName.objectSet), obj.transform.position, false, "object" , Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f));
                    }
                    else if (obj.tag == "WALLOBJECT")
                    {
                        Instantiate_check(style_decoration.getObject(style.ListName.wallObjectSet), obj.transform.position, false, "wall" ,  offsetr * obj.transform.rotation);
                    }
                    else if (obj.tag == "NEARWALLOBJECT")
                    {
                        if (hallway)
                        {
                            Instantiate_check(style_decoration.getObject(style.ListName.onHallway), obj.transform.position, false, "nearWall", offsetr * obj.transform.rotation);
                        }
                        else
                        {
                            Instantiate_check(style_decoration.getObject(style.ListName.nearWallObjectSet), obj.transform.position, false, "nearWall", offsetr * obj.transform.rotation);
                        }
                        
                    }
                    else if (obj.tag == "OTHER")
                    {
                        Instantiate_check(style_decoration.getObject(style.ListName.otherObject), obj.transform.position, true, "other" , Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f));
                    }
                }
            }

        }
    }

    Vector3 _scale(Vector3 toscale, Vector3 vector)
    {
        return new Vector3(toscale.x * vector.x, toscale.y * vector.y, toscale.z * vector.z);
    }

    public bool Instantiate_check(DgGo dgGo, Vector3 position, bool collide, string point, Quaternion rotation)
    {
        LayerMask layerMask = objectLayer;
        if (collide)
            layerMask = other;

        if(dgGo == null)
        {
            return false;
        }
        else if(dgGo.go == null)
        {
            return false;
        }

        Vector3 postor = dgGo.go.GetComponent<decorationData>().setPivot(point);

        postor = rotation * _scale(postor, dgGo.go.transform.localScale);

        BoxCollider boxCollider = dgGo.go.GetComponent<BoxCollider>();

        float width = (point == "object") ? gameSetting.playerWidth : 0f;

        if (Physics.OverlapBox(postor + boxCollider.center + position, _scale(boxCollider.size / 2, dgGo.go.transform.localScale) + new Vector3(1f, 0, 1f) * width / 2, rotation, layerMask).Length > 0)
        {

            return false;
        }

        GameObject place = Instantiate(dgGo.go, postor + position, rotation);

        place.transform.parent = decorate;

        return true;
    }
}
