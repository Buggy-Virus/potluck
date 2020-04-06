using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int zone;
    public Point anchorPoint;
    public List<Portal> portals;
    public List<Edge> edges;
    public int type;

    public int cost;
    
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

    public Point midPoint;
    public List<Face> faces;
    public List<Portal> portals;
    public List<Node> nodes;
}

public class Room {
    public int type;

    public Point midPoint;
    public Point anchorPoint;
    public List<Face> faces;
    public List<Portal> portals;
    public List<Node> nodes;

    public Room(RoomSkeleton roomSkeleton) {
        this.faces = roomSkeleton.faces;
        this.portals = roomSkeleton.portals;
        this.nodes = roomSkeleton.nodes;
    }
}