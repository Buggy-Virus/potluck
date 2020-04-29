using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperMap {
    // =============================== Parameters
    // Total Size Of Map
    public int xSize;
    public int ySize;
    public int zSize;

    // Target number of rooms in the dungeon
    public int targetRoomCount;

    // Target number of Zones in the dungeon
    public int targetZoneSize;
    // Amount by which the number of zones can vary
    public int zoneRange;

    // Target number of edges between two connected zones
    public int targetZoneEdges;
    // Amount by which the number of zone edges can vary
    public int zoneEdgesRange;

    // Average horizontal distance between two rooms when one is built off the other horizontally
    public float horizontalSparsity;
    // Average vertical distance betwen two rooms when one is built off the other vertically
    public float verticalSparsity;
    // Amount by which the sparsity can vary
    public float sparsityFactor;
    // range by which a room can be horizontally misaligned with the room it's built off of
    public float horizontalAlignMent;
    // range by which a room can be vertically misaligned with the room it's built off of
    public float verticalAlignMent;

    // Value used to judge how heavily weighted portal distance should be when connecting portals via edges
    // factor should be between 1 and 0, with lower values weighting it more weakly
    public float portalDistanceFactor;

    // First room to place in the dungeon, set to -1 to ignore
    public int firstRoom;
    // List of required rooms that need to be placed in the dungeon
    public List<RequiredRoom> requiredRooms;
    // Probability weights for the non required rooms that get added to the dungeon
    // Are used when deciding what room to place next
    public Dictionary<int, double> roomWeights; 

    // Probability weights for what condition should be placed on an edge connecting two zones
    public Dictionary<int, double> zoneEdgeWeights;

    // The allowed max number of a type of room in the dungeon
    public Dictionary<int, int> roomMaxes;
    
    // =============================== Generated Values
    // The total number of rooms
    public float roomCount;
    // the number of rooms per room type
    public Dictionary<int, int> roomCounts;
    
    // The skeleton of the map
    // After being painted represents the real map
    public int[,,] skeleton;

    // Sets of nodes within the dungeon that are naively connected
    public List<Zone> zones;
    // All rooms in the dungeon
    public List<Room> rooms;
    // All faces of rooms which haven't been attempted to build a room off of yet
    public List<Face> openFaces;
    // All portals in the dungeon
    public List<Portal> portals;
    // All nodes in the dungeon
    public List<Node> nodes;

    // The index ofthe current key being placed to lock an edge
    public int currentKey;

    // Adjacency graph for node connections
    public int[,] naiveGraph;
    // Adjacency graph for nodes connected by being in the same room
    public int[,] roomGraph;
    // Adjacency graph for nodes connected via special edges (not used)
    public int[,] specialGraph;
    // Adjacency graph for connections between zones
    public int[,] zoneGraph;
    // Array tracking all edges connecting two specific zones
    public List<Edge>[,] zoneEdgeGraph;
}