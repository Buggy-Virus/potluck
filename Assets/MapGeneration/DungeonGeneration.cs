using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point {
    public int x;
    public int y;
    public int z;

    public Point(int i, int j,int k) {
        this.x = i;
        this.y = j;
        this.z = k;
    }
}

public struct Face {
    public Point corner00;
    public Point corner10;
    public Point corner01;
    public Point corner11; 
    public int direction;
}

public class Portal {
    public int type;
    public Point point;
    public int direction;
    public Node node;
    public bool connected;
    public bool setup;
}

public struct Edge {
    public Node source;
    public Portal sourcePortal;
    public Node sink;
    public Portal sinkPortal;
    public int lenght;
    public int type;

    public Edge(Node source_input, Portal sourcePortal_input, Node sink_input, Portal sinkPortal_input, int type_input) {
        this.source = source_input;
        this.sourcePortal = sourcePortal_input;
        this.sink = sink_input;
        this.sinkPortal = sinkPortal_input;
        this.type = type_input;
        this.lenght = 0;
    }
}

public class Node {
    // 0 is start
    // 1 is objective
    public int id;
    public Point anchorPoint;
    public List<Portal> portals;
    public List<Edge> edges;
    public int type;

    public int cost;
    
}

public class Room {
    public int type;

    public Point anchorPoint;
    public List<Face> faces;
    public List<Portal> portals;
    public List<Node> nodes;
}

public struct RoomPrefab {
    public int xMax;
    public int yMax;
    public int zMax;

    public int xMin;
    public int yMin;
    public int zMin;

    public int type;
    public List<Tuple<int, Point>> requiredRooms;
}

public class RoomSkeleton{
    public int[,,] skeleton;

    public int xLength;
    public int yLength;
    public int zLength;

    public List<Face> faces;
    public List<Portal> portals;
    public List<Node> nodes;
}

public class SuperMap {
    public int xSize;
    public int ySize;
    public int zSize;

    public int[,,] skeleton;

    public float targetComplexity;
    public float targetSparsity;
    public float targetParallelism;

    public float complexity;
    public float horizontalSparsity;
    public float verticalSparsity;
    public float horizontalAlignMent;
    public float verticalAlignMent;
    public float parallelism;

    public List<Tuple<int, Point>> requiredRooms;
    public List<Tuple<double, int>> roomWeights; 

    public Dictionary<int, int> roomMaxes;
    public Dictionary<int, int> roomCounts;
    
    public List<Room> rooms;
    public List<Face> openFaces;
    public List<Portal> portals;
    public List<Node> nodes;
    public List<Edge> edges;

    public int[,] graph;
}

public class DungeonGeneration {

    static Dictionary<int, RoomPrefab> rooms = new Dictionary<int, RoomPrefab>{

    };

    static double Distance(Point a, Point b) {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
    }

    static Point PickNextLocation(SuperMap superMap, System.Random random) {
        int openFaceIndex = random.Next(superMap.openFaces.Count());
        Face face = superMap.openFaces[openFaceIndex];
        superMap.openFaces.RemoveAt(openFaceIndex);

        Point nextLocation = new Point();

        if (face.direction == 1 || face.direction == 4) {
            nextLocation.x = face.corner00.x;
            nextLocation.y = face.corner00.y;
            nextLocation.z = (face.corner00.z + face.corner10.z) / 2;
        } else if (face.direction == 2 || face.direction == 5) {
            nextLocation.z = face.corner00.z;
            nextLocation.y = face.corner00.y;
            nextLocation.x = (face.corner00.x + face.corner10.x) / 2;
        } else {
            nextLocation.y = face.corner00.y;
            nextLocation.x = (face.corner00.x + face.corner10.x) / 2;
            nextLocation.z = (face.corner00.z + face.corner10.z) / 2;
        }

        return nextLocation;
    }

    static RoomPrefab GetRoomPrefab(int type) {
        return new RoomPrefab();
    }

    public static RoomSkeleton GenerateRoomSkeleton(System.Random random, RoomPrefab roomPrefab, Tuple<Point, Point> bounds) {
        int xMax = bounds.Item2.x - bounds.Item1.x;
        int yMax = bounds.Item2.y - bounds.Item1.y;
        int zMax = bounds.Item2.z - bounds.Item1.z;

        RoomSkeleton roomSkeleton = new RoomSkeleton();
        roomSkeleton.skeleton = new int[xMax,yMax,zMax];

        for (int x = 0; x < xMax; x++) {
            for (int y = 0; y < yMax; y++) {
                roomSkeleton.skeleton[x, y, 0] = 1;
                roomSkeleton.skeleton[x, y, zMax - 1] = 1;
            }
        }

        for (int x = 0; x < xMax; x++) {
            for (int z = 0; z < zMax; z++) {
                roomSkeleton.skeleton[x, 0, z] = 1;
                roomSkeleton.skeleton[x, yMax - 1, z] = 1;
            }
        }

        for (int y = 0; y < yMax; y++) {
            for (int z = 0; z < zMax; z++) {
                roomSkeleton.skeleton[0, y, z] = 1;
                roomSkeleton.skeleton[xMax - 1, y, z] = 1;
            }
        }

        return roomSkeleton;
    }

    static Tuple<Point, Point> MeasureFreeSpace(SuperMap superMap, Point point) {
        // 1 xyz
        // 2 xzy
        // 3 zxy
        // 4 yxz
        
        int xMax12 = superMap.xSize;
        int xMin12 = 0;
        int zMax3 = superMap.zSize;
        int zMin3 = 0;
        int yMax4 = superMap.ySize;
        int yMin4 = 0;
        foreach (Room room in superMap.rooms) {
            foreach (Face face in room.faces) {
                if (face.direction == 1 && point.x > face.corner00.x && 
                    point.y >= face.corner00.y && point.y <= face.corner11.y && 
                    point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.x > xMin12) {
                        xMin12 = face.corner00.x;
                    }
                } else if (face.direction == 4 && point.x < face.corner00.x &&
                           point.y >= face.corner00.y && point.y <= face.corner11.y && 
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.x < xMax12) {
                        xMax12 = face.corner00.x;
                    } 
                } else if (face.direction == 2 && point.y > face.corner00.y && 
                           point.x >= face.corner00.x && point.x <= face.corner11.x && 
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.y > yMin4) {
                        yMin4 = face.corner00.y;
                    }
                } else if (face.direction == 5 && point.y < face.corner00.y &&
                           point.x >= face.corner00.x && point.x <= face.corner11.x &&
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {

                    if (face.corner00.y < yMax4) {
                        yMax4 = face.corner00.y;
                    }
                } else if (face.direction == 3 && point.z > face.corner00.z && 
                           point.x >= face.corner00.x && point.x <= face.corner11.x && 
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {
                    
                    if (face.corner00.z > zMin3) {
                        zMin3 = face.corner00.z;
                    }
                } else if (face.direction == 6 && point.z < face.corner00.z &&
                           point.x >= face.corner00.x && point.x <= face.corner11.x &&
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {

                    if (face.corner00.z < zMax3) {
                        zMax3 = face.corner00.z;
                    }
                }
            }
        }

        int yMax1 = superMap.ySize;
        int yMin1 = 0;
        int zMax2 = superMap.zSize;
        int zMin2 = 0;
        int xMax3 = superMap.xSize;
        int xMin3 = 0;
        int xMax4 = superMap.xSize;
        int xMin4 = 0;
        foreach (Room room in superMap.rooms) {
            foreach (Face face in room.faces) {
                if (face.direction == 1 && point.x > face.corner00.x) {
                    if (point.y >= face.corner00.y && point.y <= face.corner11.y && 
                        zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {

                            if (face.corner00.x < xMin3) {
                                xMin3 = face.corner00.x;
                            } 
                    } else if (yMax4 >= face.corner00.y && yMin4 <= face.corner11.y && 
                               point.z >= face.corner00.z && point.z <= face.corner11.z) {

                            if (face.corner00.x < xMin4) {
                                xMin4 = face.corner00.x;
                            } 
                    }
                } else if (face.direction == 4 && point.x < face.corner00.x) {
                    if (point.y >= face.corner00.y && point.y <= face.corner11.y && 
                        zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {

                            if (face.corner00.x < xMax3) {
                                xMax3 = face.corner00.x;
                            } 
                    } else if (yMax4 >= face.corner00.y && yMin4 <= face.corner11.y && 
                               point.z >= face.corner00.z && point.z <= face.corner11.z) {

                            if (face.corner00.x < xMax4) {
                                xMax4 = face.corner00.x;
                            } 
                    }
                } else if (face.direction == 2 && point.y > face.corner00.y && 
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.y > yMin1) {
                        yMin1 = face.corner00.y;
                    }
                } else if (face.direction == 5 && point.y < face.corner00.y &&
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {

                    if (face.corner00.y < yMax1) {
                        yMax1 = face.corner00.y;
                    }
                } else if (face.direction == 3 && point.z > face.corner00.z && 
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {
                    
                    if (face.corner00.z > zMin2) {
                        zMin2 = face.corner00.z;
                    }
                } else if (face.direction == 6 && point.z < face.corner00.z &&
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {

                    if (face.corner00.z < zMax2) {
                        zMax2 = face.corner00.z;
                    }
                }
            }
        }

        int zMax1 = superMap.zSize;
        int zMin1 = 0;
        int yMax2 = superMap.ySize;
        int yMin2 = 0;
        int yMax3 = superMap.ySize;
        int yMin3 = 0;
        int zMax4 = superMap.zSize;
        int zMin4 = 0;
        foreach (Room room in superMap.rooms) {
            foreach (Face face in room.faces) {
                if (face.direction == 2 && point.y > face.corner00.y) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                        zMax2 >= face.corner00.z && zMin2 <= face.corner11.z) {

                        if (face.corner00.y > yMin2) {
                            yMin2 = face.corner00.y;
                        }
                    } else if (xMax3 >= face.corner00.x && xMin3 <= face.corner11.x && 
                               zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {

                        if (face.corner00.y > yMin3) {
                            yMin3 = face.corner00.y;
                        }
                    }
                } else if (face.direction == 5 && point.y < face.corner00.y) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                        zMax2 >= face.corner00.z && zMin2 <= face.corner11.z) {
                        
                        if (face.corner00.y < yMax2) {
                            yMax2 = face.corner00.y;
                        }
                    } else if (xMax3 >= face.corner00.x && xMin3 <= face.corner11.x && 
                               zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {
                        
                        if (face.corner00.y < yMax3) {
                            yMax3 = face.corner00.y;
                        }
                    }
                } else if (face.direction == 3 && point.z > face.corner00.z) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                        yMax1 >= face.corner00.y && yMin1 <= face.corner11.y) {

                        if (face.corner00.z > zMin1) {
                            zMin1 = face.corner00.z;
                        }
                    } else if (xMax4 >= face.corner00.x && xMin4 <= face.corner11.x &&
                               yMax4 >= face.corner00.y && yMin4 <= face.corner11.y) {

                        if (face.corner00.z > zMin4) {
                            zMin4 = face.corner00.z;
                        }
                    }
                } else if (face.direction == 6 && point.z < face.corner00.z) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                        yMax1 >= face.corner00.y && yMin1 <= face.corner11.y) {

                        if (face.corner00.z < zMax1) {
                            zMax1 = face.corner00.z;
                        }
                    } else if (xMax4 >= face.corner00.x && xMin4 <= face.corner11.x &&
                               yMax4 >= face.corner00.y && yMin4 <= face.corner11.y) {

                        if (face.corner00.z < zMax4) {
                            zMax4 = face.corner00.z;
                        }
                    }
                }
            }
        }

        int volume1 = (xMax12 - xMin12) * (yMax1 - yMin1) * (zMax1 - zMin1);
        int volume2 = (xMax12 - xMin12) * (yMax2 - yMin2) * (zMax2 - zMin2);
        int volume3 = (xMax3 - xMin3) * (yMax3 - yMin3) * (zMax3 - zMin3);
        int volume4 = (xMax4 - xMin4) * (yMax4 - yMin4) * (zMax4 - zMin4);

        if (volume1 >= volume2 && volume1 >= volume3 && volume1 >= volume4) {
            return Tuple.Create(new Point(xMin12, yMin1, zMin1), new Point(xMax12, yMax1, zMax1));
        } else if (volume2 >= volume3 && volume2 >= volume4) {
            return Tuple.Create(new Point(xMin12, yMin2, zMin2), new Point(xMax12, yMax2, zMax2));
        } else if (volume3 >= volume4) {
            return Tuple.Create(new Point(xMin3, yMin3, zMin3), new Point(xMax3, yMax3, zMax3));
        } else {
            return Tuple.Create(new Point(xMin4, yMin4, zMin4), new Point(xMax4, yMax4, zMax4));
        }
    }

    static int PickNextRoom(SuperMap superMap, System.Random random, Tuple<Point, Point> nextRoomBounds, Point nextRoomLocation) {
        double total = 0;
        List<Tuple<double, int>> cutoffs = new List<Tuple<double, int>>();

        foreach (Tuple<int, Point> requiredRoom in superMap.requiredRooms) { 
            cutoffs.Add(new Tuple<double, int>(total, requiredRoom.Item1));
            total += 1000 / Distance(nextRoomLocation, requiredRoom.Item2);;
        }

        if (superMap.complexity < superMap.targetComplexity) {
            foreach (Tuple<double, int> roomWeight in superMap.roomWeights) {
                int type = roomWeight.Item2;
                if (superMap.roomCounts[type] < superMap.roomMaxes[type]) {
                    cutoffs.Add(new Tuple<double, int>(total, type));
                    total += roomWeight.Item1;
                }
            }
        }

        double roll = random.NextDouble() * total;
        int result = 0;

        foreach (Tuple<double, int> cutoff in cutoffs) {
            if (roll > cutoff.Item1){
                result = cutoff.Item2;
            } else {
                break;
            }
        }

        return result;
    }

    static Point PickAnchorPoint(SuperMap superMap, System.Random random, Point nextRoomLocation, Tuple<Point, Point> nextRoomBounds, RoomSkeleton nextRoomSkeleton) {
        int xMaxDistance = nextRoomBounds.Item2.x - nextRoomBounds.Item1.x - nextRoomSkeleton.xLength;
        int zMaxDistance = nextRoomBounds.Item2.z - nextRoomBounds.Item1.z - nextRoomSkeleton.zLength;
        int yMaxDistance = nextRoomBounds.Item2.y - nextRoomBounds.Item1.y - nextRoomSkeleton.yLength;

        int x;
        int y;
        int z;

        if (nextRoomLocation.x == nextRoomBounds.Item1.x) {
            x = nextRoomLocation.x + (int)(superMap.horizontalSparsity * random.NextDouble());
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else if (nextRoomLocation.z == nextRoomBounds.Item1.z) {
            z = nextRoomLocation.z + (int)(superMap.horizontalSparsity * random.NextDouble());
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else if (nextRoomLocation.y == nextRoomBounds.Item1.y) {
            y = nextRoomLocation.y + (int)(superMap.verticalSparsity * random.NextDouble());
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
        } else if (nextRoomLocation.x == nextRoomBounds.Item2.x) {
            x = nextRoomLocation.x - (int)(superMap.horizontalSparsity * random.NextDouble());
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else if (nextRoomLocation.x == nextRoomBounds.Item2.z) {
            z = nextRoomLocation.z - (int)(superMap.horizontalSparsity * random.NextDouble());
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else {
            y = nextRoomLocation.y - (int)(superMap.verticalSparsity * random.NextDouble());
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
        }

        if (x < nextRoomBounds.Item1.x) {
            x = nextRoomBounds.Item1.x;
        } else if (x > nextRoomBounds.Item2.x - nextRoomSkeleton.xLength) {
            x = nextRoomBounds.Item2.x - nextRoomSkeleton.xLength;
        }

        if (z < nextRoomBounds.Item1.z) {
            z = nextRoomBounds.Item1.z;
        } else if (z > nextRoomBounds.Item2.z - nextRoomSkeleton.zLength) {
            z = nextRoomBounds.Item2.z - nextRoomSkeleton.zLength;
        }

        if (y < nextRoomBounds.Item1.y) {
            y = nextRoomBounds.Item1.y;
        } else if (y > nextRoomBounds.Item2.y - nextRoomSkeleton.yLength) {
            y = nextRoomBounds.Item2.y - nextRoomSkeleton.yLength;
        }

        return new Point(x, y, z);
    }

    static SuperMap PlaceRoom(SuperMap superMap, RoomSkeleton roomSkeleton, Point anchorPoint, RoomPrefab roomPrefab) {
        for (int x = 0; x < roomSkeleton.skeleton.GetLength(0); x++) {
            for (int y = 0; y < roomSkeleton.skeleton.GetLength(1); y++) {
                for (int z = 0; z < roomSkeleton.skeleton.GetLength(2); z++) {
                    if (roomSkeleton.skeleton[x,y,z] != 0) {
                        superMap.skeleton[anchorPoint.x + x, anchorPoint.y + y, anchorPoint.z + z] = roomSkeleton.skeleton[x,y,z];
                    }
                }
            }
        }

        Room room = new Room();
        room.type = roomPrefab.type;
        room.anchorPoint = anchorPoint;
        room.faces = roomSkeleton.faces;
        room.portals = roomSkeleton.portals;
        room.nodes = roomSkeleton.nodes;

        superMap.rooms.Add(room);
        if (superMap.roomCounts.ContainsKey(room.type)) {
            superMap.roomCounts[room.type] += 1;
        } else {
            superMap.roomCounts[room.type] = 1;
        }
        superMap.requiredRooms = superMap.requiredRooms.Concat(roomPrefab.requiredRooms).ToList();
        superMap.openFaces = superMap.openFaces.Concat(room.faces).ToList();
        superMap.portals = superMap.portals.Concat(room.portals).ToList();
        superMap.nodes = superMap.nodes.Concat(room.nodes).ToList();
        
        return superMap;
    }

    static SuperMap GenerateDungeonRooms(SuperMap superMap, System.Random random) {
        int firstRequiredRoomInded = random.Next(0, superMap.requiredRooms.Count);
        Tuple<int, Point> firstRequiredRoom = superMap.requiredRooms[firstRequiredRoomInded];
        RoomPrefab firstRoomPrefab = GetRoomPrefab(firstRequiredRoom.Item1);
        superMap.requiredRooms.RemoveAt(firstRequiredRoomInded);

        RoomSkeleton firstRoomSkeleton = GenerateRoomSkeleton(random, firstRoomPrefab, new Tuple<Point, Point>(new Point(0,0,0), new Point(superMap.xSize, superMap.ySize, superMap.zSize)));

        superMap = PlaceRoom(superMap, firstRoomSkeleton, new Point(0, 0, 0), firstRoomPrefab);

        while(superMap.requiredRooms.Count > 0 && superMap.complexity >= superMap.targetComplexity) {
            Point nextRoomLocation = PickNextLocation(superMap, random);
            Tuple<Point, Point> nextRoomBounds = MeasureFreeSpace(superMap, nextRoomLocation);

            int nextRoomType = PickNextRoom(superMap, random, nextRoomBounds, nextRoomLocation);
            RoomPrefab nextRoomPrefab = GetRoomPrefab(nextRoomType);
            RoomSkeleton nextRoomSkeleton = GenerateRoomSkeleton(random, nextRoomPrefab, nextRoomBounds);
            Point nextRoomAnchorPoint = PickAnchorPoint(superMap, random, nextRoomLocation, nextRoomBounds, nextRoomSkeleton); 
            superMap = PlaceRoom(superMap, nextRoomSkeleton, nextRoomAnchorPoint, nextRoomPrefab);
        }

        return superMap;
    }

    static int[] Djikstra(SuperMap superMap, int origin) {
        bool[] visited = new bool[superMap.nodes.Count()];
        int[] distanceFromOrigin = new int[superMap.nodes.Count()];
        for (int i = 0; i < distanceFromOrigin.Count(); i++) {
            distanceFromOrigin[i] = int.MaxValue;
            visited[i] = false;
        }

        distanceFromOrigin[origin] = 0;

        List<Tuple<int, int>> assigned = new List<Tuple<int, int>>();
        assigned.Add(new Tuple<int, int>(origin, 0));

        while (assigned.Count() > 0) {
            int minIndex = -1;
            int minDist = int.MaxValue;
            for (int i = 0; i < distanceFromOrigin.Count(); i++) {
                if (!visited[i] && distanceFromOrigin[i] < minDist) {
                    minIndex = i;
                    minDist = distanceFromOrigin[i];
                }
            }

            if (minIndex == -1) {
                break;
            }

            for (int i = 0; i < superMap.graph.GetLength(1); i++) {
                if (superMap.graph[minIndex,i] == 1) {
                    int newDist = distanceFromOrigin[minIndex] + 1;
                    if (newDist < distanceFromOrigin[i]) {
                        distanceFromOrigin[i] = newDist;
                    }
                }
            }

            visited[minIndex] = true;
        }

        return distanceFromOrigin;
    }

    static SuperMap GenerateDungeonGraph(SuperMap superMap, System.Random random) {
        superMap.graph = new int[superMap.nodes.Count(), superMap.nodes.Count()];

        foreach (Node node in superMap.nodes) {
            foreach (Edge edge in superMap.edges) {
                superMap.graph[node.id, edge.sink.id] = edge.type;
            }
        }

        List<Portal> unconnectedPortals = superMap.portals.GetRange(0, superMap.portals.Count);

        foreach (Portal portal in superMap.portals) {
            if (!portal.connected && unconnectedPortals.Count() > 1) {
                double total = 0;
                List<Tuple<double, int>> cutoffs = new List<Tuple<double, int>>();

                int count = 0;
                foreach (Portal eligiblePortal in unconnectedPortals) {
                    if (eligiblePortal.type == portal.type) {
                        cutoffs.Add(new Tuple<double, int>(total, count));
                        total += Math.Pow(Distance(portal.point, eligiblePortal.point), 1.0);
                    }
                    count += 1;
                }

                double roll = random.NextDouble() * total;
                int result = 0;

                foreach (Tuple<double, int> cutoff in cutoffs) {
                    if (roll > cutoff.Item1){
                        result = cutoff.Item2;
                    } else {
                        break;
                    }
                }

                Portal portalResult = unconnectedPortals[result]; 
                superMap.graph[portal.node.id, portalResult.node.id] = portal.type;
                superMap.graph[portalResult.node.id, portal.node.id] = portal.type;
                Edge forwardEdge = new Edge(portal.node, portal, portalResult.node, portalResult, portal.type);
                Edge backwardEdge = new Edge(portalResult.node, portalResult, portal.node, portal, portal.type);
                portal.node.edges.Add(forwardEdge);
                portalResult.node.edges.Add(backwardEdge);
                portal.connected = true;
                portalResult.connected = true;
                unconnectedPortals.RemoveAt(result);
                unconnectedPortals.Remove(portal);
            }
        }

        int origin = 0;
        List<int> goals = new List<int>();

        foreach (Node node in superMap.nodes) {
            if (node.type == 0) {
                origin = node.id;
                break;
            } else if (node.type == 1) {
                goals.Add(node.id);
            }
        }

        int[] distanceFromOrigin = Djikstra(superMap, origin);
        bool goalsReachable = true;
        foreach (int goal in goals) {
            goalsReachable = goalsReachable && distanceFromOrigin[goal] != int.MaxValue;
        }
        
        

        // Need a way to ensure the entrance and all goals have paths
        // Prune some unnecessary hallways
        // Place locks and keys such that all keys can be accessed before locks

        return superMap;
    }

    static public SuperMap GenerateDungeonSuperMap(SuperMap superMap, System.Random random) {        
        superMap = GenerateDungeonRooms(superMap, random);
        superMap = GenerateDungeonGraph(superMap, random);
        superMap = GenerateDungeonHallways(superMap, random);
        superMap = GenerateDungeonZones(superMap, random);
        superMap = PlaceDungeonObjects(superMap, random);
        superMap = PlaceDungeonMobs(superMap, random);

        return superMap;
    }
}
