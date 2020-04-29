using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSkeleton{
    public int[,,] skeleton;

    public int xLength;
    public int yLength;
    public int zLength;

    public Point midPoint;
    public List<Face> faces;
    public List<Face> openFaces;
    public List<Portal> portals;
    public List<Node> nodes;
}