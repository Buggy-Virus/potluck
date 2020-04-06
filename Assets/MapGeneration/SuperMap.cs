using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMap {
    public int xSize;
    public int ySize;
    public int zSize;

    public int[,,] skeleton;

    public float targetRoomCount;

    public float roomCount;
    public float horizontalSparsity;
    public float verticalSparsity;
    public float horizontalAlignMent;
    public float verticalAlignMent;

    public List<Tuple<int, Point>> requiredRooms;
    public List<Tuple<double, int>> roomWeights; 

    public Dictionary<int, int> roomMaxes;
    public Dictionary<int, int> roomCounts;
    
    public List<Room> rooms;
    public List<Face> openFaces;
    public List<Portal> portals;
    public List<Node> nodes;
    public List<Edge> edges;

    public int[,] naiveGraph;
    public int[,] roomGraph;
    public int[,] specialGraph;
}