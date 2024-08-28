using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using static LockAndKeyGeneration;


public class Generator2D : MonoBehaviour {
    enum CellType {
        None,
        Room,
        Hallway
    }

    [System.Serializable]
    public class Room {
        public RectInt bounds;
        public roomObject roomObject;

        public Vector2Int location;
        public Vector2Int size;

        public int layer;

        public Room(Vector2Int _location, Vector2Int _size) {
            bounds = new RectInt(_location, _size);
            location = _location;
            size = _size;
        }

        public bool Equals(Room other)
        {
            if(other == null) { return false; }
            return location == other.location && layer == other.layer;
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }

    [System.Serializable]
    public class layer_info
    {
        public roomObject[] rooms;
        public int[] number_for_rooms;
        public int num_of_room;
    }

    [SerializeField]
    Vector2Int size;
    
    int roomCount;
    [SerializeField]
    Vector2Int roomMaxSize;

    [SerializeField]
    Vector2Int roomMinSize;

    Random random;
    Grid2D<CellType> grid;
    public List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;


    roomObject[] roomObjects;
    int[] num_of_each_room;
    [SerializeField] int randint;
    [SerializeField] float costFunction_longer;
    [SerializeField] float height_floor;
    [SerializeField] int interval_source;
    public List<layer_info> layers;
    int current_layer_in_dgenerato;
    int room_in_current_layer;

    List<objectGenerator> objectGenerators;
    List<roomGenerate> roomGenerates;
    List<Room> all_rooms;
    List<roomGenerate> all_roomGenerators;

    [SerializeField] int gridSize;
    [SerializeField] float doorChance;

    [SerializeField] GameObject p;

    [SerializeField] int keysInRoom;

    public void multiLayerGenerate()
    {

        objectGenerators = new List<objectGenerator>();
        roomGenerates = new List<roomGenerate>();

        if(all_rooms == null)
        {
            all_rooms = new List<Room>();
        }
        else
        {
            all_rooms.Clear();
        }

        all_roomGenerators = new List<roomGenerate>();

        if(rooms != null)
        {
            rooms.Clear();
        }

        current_layer_in_dgenerato = 0;

        room_in_current_layer = 0;

        for(int i = 0; i < layers.Count; i++)
        {
            current_layer_in_dgenerato = i;

            roomGenerates.Clear();

            roomObjects = layers[i].rooms;
            num_of_each_room = layers[i].number_for_rooms;



            roomCount = layers[i].num_of_room + 1 - 1;

            Debug.Log("current Layer: " + i);
            Generate();
            foreach (roomGenerate generate_room in roomGenerates)
            {
                generate_room.block();
            }
        }

        List<GameObject> gos = new List<GameObject>();

        foreach (objectGenerator objectGenerator in objectGenerators)
        {
            objectGenerator.rseed(randint);
            gos.AddRange(objectGenerator.wall_GenerateObj());


        }

        foreach (objectGenerator objectGenerator1 in objectGenerators)
        {
            objectGenerator1.Decorate_GenerateObj();
        }

        foreach (GameObject _gameobject in gos)
        {
            objectGenerator t = _gameobject.GetComponent<objectGenerator>();

            t.Decorate_GenerateObj();


        }

        lock_and_key_tool();

        lightBake(transform);
    }

    void lightBake(Transform transform)
    {
        foreach(Transform t in transform)
        {
            Light light = t.gameObject.GetComponent<Light>();


            if (light != null)
            {
                light.renderMode = LightRenderMode.ForceVertex;
                light.lightmapBakeType = LightmapBakeType.Baked;
            }

            lightBake(t);

        }
    }

    public void ClearScene()
    {
        while (transform.childCount > 0)
        {
            foreach (Transform t in transform)
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }

    void Generate() {

        if(random != null)
        {
            random = new Random(randint + randint * 1 + current_layer_in_dgenerato * 10);
        }
        else
        {
            random = new Random(randint);
        }


        grid = new Grid2D<CellType>(size, Vector2Int.zero);

        if(rooms == null)
            rooms = new List<Room>();

        preRoomPlace();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    void lock_and_key_tool()
    {
        LockAndKeyGeneration lockAndKeyGeneration = new LockAndKeyGeneration();
        lockAndKeyGeneration.dungeon_doors = new List<door>();
        lockAndKeyGeneration.chance_of_one_way_door = doorChance;
        lockAndKeyGeneration.keyAmount = keysInRoom;

        foreach (Room room in all_rooms)
        {
            foreach (door door in room.roomObject.doorTemp)
            {
                
                if(door.connects != null)
                {
                    lockAndKeyGeneration.dungeon_doors.Add(door);
                }
                
                














            }
        }


        lockAndKeyGeneration.layer = layers.Count - 1;

        lockAndKeyGeneration.set_rseed(randint);

        lockAndKeyGeneration.build_dg();

        lockAndKeyGeneration.lockAndKeyGen();

        if(lockAndKeyGeneration.dungeonNodes != null)
        {
            
            foreach (DungeonNode dungeon in lockAndKeyGeneration.dungeonNodes)
            {
                int room_idx = all_roomGenerators.FindIndex(k => k.G2Room.Equals(dungeon.selfRoom));

                all_roomGenerators[room_idx].Gen_LockAndKey(dungeon, lockAndKeyGeneration.dungeonNodes, lockAndKeyGeneration.dungeonNodes1);
            }

        }

        int idx = all_roomGenerators.FindIndex(x => x.G2Room.Equals(lockAndKeyGeneration.dungeonNodes[0].selfRoom));

        all_roomGenerators[idx].spawnPlayer(p);

    }

    roomObject[] createRoomArray(roomObject[] rooms, int[] nums)
    {
        roomObject[] outcomeRoom = new roomObject[roomCount];

        int main_index = 0;
        for(int i = 0; i < rooms.Length; i++)
        {

            for(int j = 0; j < nums[i]; j++)
            {
                if(main_index < roomCount)
                {
                    if (rooms[i].random)
                    {

                        rooms[i].roomSize.x = random.Next(roomMinSize.x, roomMaxSize.x + 1);
                        rooms[i].roomSize.y = random.Next(roomMinSize.y, roomMaxSize.y + 1);

                        rooms[i].doorObj = new List<door>();
                        int dooramount = random.Next(1, 5);



                        for (int q = 1; q <= dooramount; q++)
                        {


                            //test_tst_t
                            if (q == 1)
                            {
                                rooms[i].doorObj.Add(new door(new Vector2Int(-1, random.Next(0, rooms[i].roomSize.y)), 1, false));
                            }
                            else if (q == 2)
                            {
                                rooms[i].doorObj.Add(new door(new Vector2Int(rooms[i].roomSize.x, random.Next(0, rooms[i].roomSize.y)), 1, false));
                            }
                            else if (q == 3)
                            {
                                rooms[i].doorObj.Add(new door(new Vector2Int(random.Next(0, rooms[i].roomSize.x), -1), 1, false));
                            }
                            else if (q == 4)
                            {
                                rooms[i].doorObj.Add(new door(new Vector2Int(random.Next(0, rooms[i].roomSize.x), rooms[i].roomSize.y), 1, false));
                            }
                        }


                    }
                    
                    
                    outcomeRoom[main_index] = rooms[i].Clone();
                    
                    main_index++;
                }
            }
        }

        return outcomeRoom;
    }

    void preRoomPlace()
    {
        List<Room> roomsInCurLay = new List<Room>();
        foreach(Room room in rooms)
        {
            if(room.roomObject.layer_of_room_temp >= current_layer_in_dgenerato + 1)
            {

                roomsInCurLay.Add(room);
            }
        }

        rooms = roomsInCurLay;
        room_in_current_layer = rooms.Count;

        foreach(Room room in rooms)
        {

            foreach (Vector2Int pos in room.bounds.allPositionsWithin)
            {
                grid[pos] = CellType.Room;
            }
        }


    }

    void PlaceRooms() {


        roomObject[] _roomObjects = createRoomArray(roomObjects, num_of_each_room);
        

        for (int i = 0; i < _roomObjects.Length; i++) {

            int trys = 0;

            bool add = false;

            Vector2Int location = new Vector2Int(-1, -1);

            Room newRoom = new Room(location, new Vector2Int(0, 0));

            while (!add && trys < 150)
            {
                Vector2Int _location = new Vector2Int(
                random.Next(1, size.x - _roomObjects[i].roomSize.x - 1),
                random.Next(1, size.y - _roomObjects[i].roomSize.y - 1)
                );

                Vector2Int roomSize = _roomObjects[i].roomSize;

                bool _add = true;
                Room _newRoom = new Room(_location, roomSize);
                Room buffer = new Room(_location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

                foreach (var room in rooms)
                {
                    if (Room.Intersect(room, buffer))
                    {
                        _add = false;
                        break;
                    }
                }

                if (_add)
                {
                    location = _location;
                    newRoom = _newRoom;
                    add = true;
                }
                else
                {
                    
                }

                trys += 1;
            }

            if (add) {
                rooms.Add(newRoom);
                newRoom.roomObject = _roomObjects[i];
                
                newRoom.roomObject.doorTemp = new List<door>();

                newRoom.layer = current_layer_in_dgenerato;

                foreach(door doo in newRoom.roomObject.doorObj)
                {
                   door dor = new door(new Vector2Int(doo.doorPos.x + location.x, doo.doorPos.y + location.y), doo.height + current_layer_in_dgenerato - 1, doo.passage);
                   dor.room = newRoom; 

                   newRoom.roomObject.doorTemp.Add(dor);
                }

                newRoom.roomObject.layer_of_room_temp = current_layer_in_dgenerato + newRoom.roomObject.layer_of_room;

                all_rooms.Add(newRoom);

                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size, current_layer_in_dgenerato, newRoom.roomObject.room, newRoom.roomObject, newRoom);



                foreach (Vector2Int pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }

     
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            int door = 0;
            foreach(door d in room.roomObject.doorTemp)
            {
                if(d.height == current_layer_in_dgenerato)
                {
                    door++;
                }
            }

            if(door > 0)
            {
                vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
            }
            
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < 0.125) {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(size);
        List<Vector2Int> placed = new List<Vector2Int>();
        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            float d = int.MaxValue;
            var startPos = new Vector2Int(-1, -1);
            var endPos = new Vector2Int(-1, -1);
            door doorstart = startRoom.roomObject.doorTemp[0];
            door doorend = endRoom.roomObject.doorTemp[0];
            foreach(door doorx in startRoom.roomObject.doorTemp)
            {
                foreach(door doory in endRoom.roomObject.doorTemp)
                {
                    float tmpdis = Vector2Int.Distance(doorx.doorPos, doory.doorPos);
                    if (d >= tmpdis && !doorx.connected && !doory.connected && doorx.height == current_layer_in_dgenerato && doory.height == current_layer_in_dgenerato)
                    {
                        doorstart = doorx;
                        doorend = doory;

                        startPos = doorx.doorPos;
                        endPos = doory.doorPos;

                        d = tmpdis;
                    }
                }
            }



            if(startPos.x  == -1)
            {
                doorstart = startRoom.roomObject.doorTemp[random.Next(0, startRoom.roomObject.doorTemp.Count - 1)];

                startPos = doorstart.doorPos;

                if(doorstart.height != current_layer_in_dgenerato)
                {
                    foreach(door dr_s in startRoom.roomObject.doorTemp)
                    {
                        if(dr_s.height == current_layer_in_dgenerato)
                        {
                            doorstart = dr_s;
                            startPos = dr_s.doorPos;
                            break;
                        }
                    }
                }

                doorend = endRoom.roomObject.doorTemp[random.Next(0, endRoom.roomObject.doorTemp.Count - 1)];
                
                endPos = doorend.doorPos;

                if(doorend.height != current_layer_in_dgenerato)
                {
                    foreach(door dr_e in endRoom.roomObject.doorTemp)
                    {
                        if(dr_e.height == current_layer_in_dgenerato)
                        {
                            doorend = dr_e;
                            endPos = dr_e.doorPos;
                            break;
                        }
                    }
                }

            }
            //Debug.Log(startPos + " to " + endPos);

            doorstart.connected = true;
            doorend.connected = true;

            doorstart.add(doorend);
            doorend.add(doorstart);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                pathCost.cost *= -costFunction_longer;

                if (grid[b.Position] == CellType.Room) {
                    pathCost.cost += 1000;
                } else if (grid[b.Position] == CellType.None) {
                    pathCost.cost += 5;
                } else if (grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }


                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

                int interval = 0;

                GameObject g = new GameObject("hallway");
                g.transform.parent = transform;
                g.transform.localPosition = Vector3.zero;

                GameObject decoration = new GameObject("decorate");
                decoration.transform.parent = g.transform;
                decoration.transform.localPosition = Vector3.zero;

                GameObject wall = new GameObject("walls");
                wall.transform.parent = g.transform;
                wall.transform.localPosition = Vector3.zero;

                GameObject ceil = new GameObject("ceils");
                ceil.transform.parent = g.transform;
                ceil.transform.localPosition = Vector3.zero;





                GameObject floor = new GameObject("floors");
                floor.transform.parent = g.transform;
                floor.transform.localPosition = Vector3.zero;

                startRoom.roomObject.style.setObjectSetCount();

                foreach (var pos in path) {
                    if (grid[pos] == CellType.Hallway && !vector2Int_Contains(pos, placed)) {
                        interval++;
                        bool use_light = false;
                        if (interval == interval_source)
                        {
                            use_light = true;

                            interval = 0;
                        }
                            
                        //test
                        placed.Add(pos);

                        PlaceHallway(pos, current_layer_in_dgenerato, startRoom.roomObject.style, use_light,floor.transform, decoration.transform, wall.transform, ceil.transform);
                    }
                }

            }
        }
    }

    bool vector2Int_Contains(Vector2Int V, List<Vector2Int> listV)
    {
        foreach(Vector2Int v in listV)
        {
            if(v.x == V.x && v.y == V.y) { return true; }
        }


        return false;
    }

    void PlaceRoom(Vector2Int location, Vector2Int size, int height, GameObject room, roomObject roomObject, Room room1) {

        GameObject go = Instantiate(room, new Vector3(location.x * gridSize, height * height_floor, location.y* gridSize), Quaternion.identity, transform);
        go.name = "room in" + location;
        roomGenerate rg  = go.GetComponent<roomGenerate>();

        if(rg != null)
        {
            rg.room = roomObject;

            rg.Gen();

            rg.loc = location;

            rg.G2Room = room1;

            rg.rseed(randint);
        }

        roomGenerates.Add(rg);

        all_roomGenerators.Add(rg);
    }

    void PlaceHallway(Vector2Int location, int height, style _style, bool uselight, Transform so, Transform decorate, Transform wall, Transform ceil)
    {
        //test
        GameObject go = Instantiate(_style.getObject(style.ListName.floors).go, new Vector3(location.x * gridSize, height * height_floor + 0.1f, location.y * gridSize), Quaternion.identity, so);
        objectGenerator oge = go.GetComponent<objectGenerator>();

        if(oge != null)
        {
            objectGenerators.Add(oge);

            oge.decorate = decorate;

            oge.style_decoration = _style;
            oge.wall = wall;
            oge.ceil = ceil;

            oge.hallway = true;

            if(uselight)
                oge.isLight = true;
                
        }
    }
}
