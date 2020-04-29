using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room {
    public int type;
    public bool origin;
    public bool goal;

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

    public Room(RoomPrefab roomPrefab, Point anchorPoint, RoomSkeleton roomSkeleton) {
        this.origin = roomPrefab.origin;
        this.goal = roomPrefab.goal;
        this.type = roomPrefab.type;

        this.anchorPoint = anchorPoint;
        this.midPoint = new Point(anchorPoint.x + roomSkeleton.xLength / 2, anchorPoint.y + roomSkeleton.yLength / 2, anchorPoint.z + roomSkeleton.zLength / 2); 
        this.faces = roomSkeleton.faces;
        this.portals = roomSkeleton.portals;
        this.nodes = roomSkeleton.nodes;
    }
}