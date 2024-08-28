using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Generator2D;

public class LockAndKeyGeneration
{
    public List<door> dungeon_doors;
    public int layer;

    int id;
    Room startRoom;

    public List<DungeonNode> dungeonNodes;
    public List<DungeonNode> dungeonNodes1;
    public float chance_of_one_way_door;

    public int keyAmount;

    [System.Serializable]
    public class LockAndKey
    {
        public string name;
        public int id;
        public bool isOneWay;
        public bool gen = true;

        public LockAndKey(string _name, int _id)
        {
            name = _name;
            id = _id;
        }

        public bool equ(LockAndKey other)
        {
            return other.name == name && id == other.id;
        }
    }

    public class DungeonNode
    {
        public int id;
        public Room selfRoom;

        public List<Room> connect_rooms = new List<Room>();

        public LockAndKey _lock;

        public List<LockAndKey> keys;

        public DungeonNode(Room room, int _id)
        {
            id = _id;
            selfRoom = room;
        }
    }

    public void build_dg()
    {
        List<Room> room_list = new List<Room>();

        foreach(door door in dungeon_doors)
        {
            if(door.height == layer && !room_list.Contains(door.room))
            {
                room_list.Add(door.room);
            }
        }

        startRoom = room_list[Random.Range(0, room_list.Count)];

        dungeonNodes = new List<DungeonNode>();

        DungeonNode dgNode = new DungeonNode(startRoom, 0);

        dgNode.connect_rooms = new List<Room>();

        dungeonNodeInfoFinder(dgNode);

        DungeonNode dgNode1 = new DungeonNode(startRoom, 0);

        dgNode1.connect_rooms = new List<Room>();

        dungeonNodeInfoFinder_v1(dgNode1);

    }

    void dungeonNodeInfoFinder(DungeonNode dungeonNode)
    {
        
        dungeonNode.connect_rooms = new List<Room>();

        foreach (door door in dungeon_doors)
        {
            if (door.room == dungeonNode.selfRoom)
            {
                foreach (door d in door.connects)
                {
                    if (!dungeonNode.connect_rooms.Contains(d.room))
                    {
                        dungeonNode.connect_rooms.Add(d.room);
                    }
                }
            }
        }

        dungeonNodes.Add(dungeonNode);

        foreach (door door in dungeon_doors)
        {
            if (door.room == dungeonNode.selfRoom)
            {
                foreach(door d in door.connects)
                {
                    if (!ListContainRoom(d, dungeonNodes))
                    {
                        id++;
                        DungeonNode dungeonNode1 = new DungeonNode(d.room, id);
                        dungeonNodeInfoFinder(dungeonNode1);
                    }
                }
            }
        }
    }

    void dungeonNodeInfoFinder_v1(DungeonNode dungeonNode)
    {
        Queue<DungeonNode> queue = new Queue<DungeonNode>();
        queue.Enqueue(dungeonNode);

        dungeonNodes1 = new List<DungeonNode>();

        while (queue.Count > 0)
        {
            DungeonNode current = queue.Dequeue();

            foreach (door door in dungeon_doors)
            {
                if (door.room == current.selfRoom)
                {
                    foreach (door d in door.connects)
                    {
                        Room connectedRoom = d.room;
                        if (!current.connect_rooms.Contains(connectedRoom))
                        {
                            current.connect_rooms.Add(connectedRoom);
                            if(!ListContainRoom(d, dungeonNodes1))
                            {
                                id++;
                                DungeonNode connectedNode = new DungeonNode(connectedRoom, id);
                                queue.Enqueue(connectedNode);
                            }
                            
                        }
                    }
                }
            }

            dungeonNodes1.Add(current);
        }
    }

    bool ListContainRoom(door door, List<DungeonNode> _dungeonNodes)
    {
        foreach (DungeonNode dnode in _dungeonNodes)
        {
            if (door.room == dnode.selfRoom)
            {
                return true;
            }
        }

        return false;
    }

    public void set_rseed(int rseed)
    {
        Random.InitState(rseed);
    }

    public void lockAndKeyGen()
    {
        int numNodes = Random.Range(0, dungeonNodes.Count);

        for (int i = 0; i < numNodes; i++)
        {
            if (i > 0)
            {
                int roomidx = Random.Range(1, dungeonNodes.Count);

                int k = 0;

                while(k < 100)
                {
                    int keyloc = Random.Range(0, roomidx);

                    DungeonNode parent = dungeonNodes[roomidx];
                    DungeonNode key_location = dungeonNodes[keyloc];

                    if (key_location.keys == null)
                        key_location.keys = new List<LockAndKey>();

                    LockAndKey dungeonLockAndKey = new LockAndKey("dg1", roomidx);


                    int idx1 = dungeonNodes1.FindIndex(x => x.selfRoom.Equals(dungeonNodes[roomidx].selfRoom));

                    if (idx1 < roomidx && Random.Range(0f, 1f) < chance_of_one_way_door)
                    {
                        //test
                        dungeonLockAndKey.isOneWay = true;
                    }
                    else
                    {
                        //test
                        dungeonLockAndKey.isOneWay = false;
                    }

                    if(key_location.keys.Count < keyAmount)
                    {
                        parent._lock = dungeonLockAndKey;
                        key_location.keys.Add(dungeonLockAndKey);
                        break;
                    }

                    k++;
                }
                
            }

        }
    }
}
