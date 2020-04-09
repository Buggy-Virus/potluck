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
    public int edgeCount;
    public bool setup;
}

public struct Edge {
    public Portal source;
    public Portal sink;
    public int length;
    public int type;
    public int conditional;
    public int key;

    public Edge(Portal sourcePortal_input, Portal sinkPortal_input) {
        this.source = sourcePortal_input;
        this.sink = sinkPortal_input;
        this.type = sourcePortal_input.type;
        this.length = 0;
        this.conditional = 0;
        this.key = 0;
        sourcePortal_input.node.edges.Add(this);
        sinkPortal_input.node.edges.Add(this);
        sourcePortal_input.edgeCount += 1;
        sinkPortal_input.edgeCount += 1;
    }
}

public class Node {
    // 0 is start
    // 1 is objective
    public int id;
    public int zone;
    public Point midPoint;
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

public class Zone {
    public int id;
    public int type;

    public Point midPoint;

    public List<Node> nodes;
    public List<int> keys;

    public Zone(int id_input, int type_input) {
        this.id = id_input;
        this.type = type_input;
        this.nodes = new List<Node>();
    }
}