using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration {

    // Generates and places rooms within the dungeon
    static SuperMap GenerateDungeonRooms(SuperMap superMap, System.Random random) {
        Debug.Log("Start Generate Dungeon Rooms");

        // Pick and place first room for required rooms
        int firstRoomType;
        if (superMap.firstRoom != -1) {
            firstRoomType = superMap.firstRoom;
        } else {
            int firstRequiredRoomInded = random.Next(0, superMap.requiredRooms.Count);
            superMap.requiredRooms.RemoveAt(firstRequiredRoomInded);
            firstRoomType = superMap.requiredRooms[firstRequiredRoomInded].type;
        }
        RoomPrefab firstRoomPrefab = DungeonRoom.GetRoomPrefab(firstRoomType);
        RoomSkeleton firstRoomSkeleton = DungeonRoomSkeleton.GenerateRoomSkeleton(random, firstRoomPrefab, (new Point(0,0,0), new Point(superMap.xSize, superMap.ySize, superMap.zSize)));
        superMap = DungeonRoom.PlaceRoom(superMap, firstRoomSkeleton, new Point(superMap.xSize / 2, superMap.ySize / 2, superMap.zSize / 2), firstRoomPrefab);
        Debug.Log("Placed First Room");

        // Repeatedly add new rooms until all required rooms are placed and total rooms exceeds target room count
        while(superMap.requiredRooms.Count > 0 || superMap.roomCount <= superMap.targetRoomCount) {
            Debug.Log("Generate Rooms While Loop, Required Room Count = " + superMap.requiredRooms.Count + ", Room Count = " + superMap.roomCount);
            Point nextRoomLocation = DungeonRoom.PickNextLocation(superMap, random);
            (Point, Point) nextRoomBounds = DungeonRoom.MeasureFreeSpace(superMap, nextRoomLocation);
            
            // Pick the next room, if it isn't a legitimate pick, keep going, the face will have been evicted so we're ok
            Debug.Log("Attempting to find Next Room");
            int nextRoomType = DungeonRoom.PickNextRoom(superMap, random, nextRoomBounds, nextRoomLocation);
            if (nextRoomType != -1) {
                Debug.Log("Found next Room Type = " + nextRoomType);
                RoomPrefab nextRoomPrefab = DungeonRoom.GetRoomPrefab(nextRoomType);
                RoomSkeleton nextRoomSkeleton = DungeonRoomSkeleton.GenerateRoomSkeleton(random, nextRoomPrefab, nextRoomBounds);
                Point nextRoomAnchorPoint = DungeonRoom.PickAnchorPoint(superMap, random, nextRoomLocation, nextRoomBounds, nextRoomSkeleton); 
                superMap = DungeonRoom.PlaceRoom(superMap, nextRoomSkeleton, nextRoomAnchorPoint, nextRoomPrefab);
                Debug.Log("Placed Room");
            }
        }

        Debug.Log("Total Roomcount = " + superMap.roomCount.ToString());
        return superMap;
    }

    // Generate the dungeon graph of zones, connections between zones, and connection between rooms
    static SuperMap GenerateDungeonGraph(SuperMap superMap, System.Random random) {
        Debug.Log("Start Generate Dungeon Graph");

        superMap.naiveGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];
        superMap.roomGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];
        superMap.specialGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];

        superMap = DungeonGraph.InitializeNodeIds(superMap);
        superMap = DungeonGraph.InitializeGraphs(superMap);
        List<int> goalNodes = new List<int>();
        foreach (Node node in superMap.nodes) {
            if (node.goal) {
                goalNodes.Add(node.id);
            }
        }
        Debug.Log("Graph objects Initialized");

        superMap = DungeonGraph.CreateZones(superMap, random, goalNodes);
        superMap = DungeonGraph.CreateZoneGraph(superMap, random);
        superMap = DungeonGraph.AddEdgeConditions(superMap, random);

        Debug.Log("SuperMap graph successfully created");

        return superMap;
    }

    static SuperMap GenerateDungeonHallways(SuperMap superMap, System.Random random) {
        Debug.Log("Starting Generate Dungeon Hallways");

        superMap = DungeonHallway.CreateEdgePaths(superMap);
        superMap = DungeonHallway.PlaceHallways(superMap);

        return superMap;
    }

    static SuperMap GenerateDungeonZones(SuperMap superMap, System.Random random) {

        return superMap;
    }

    static SuperMap PlaceDungeonObjects(SuperMap superMap, System.Random random) {

        return superMap;
    }

    static SuperMap PlaceDungeonMobs(SuperMap superMap, System.Random random) {

        return superMap;
    }

    static public SuperMap GenerateDungeonSuperMap(SuperMap superMap, System.Random random) {
        Debug.Log("Start Generate Dungeon SuperMap");
        superMap = GenerateDungeonRooms(superMap, random);
        superMap = GenerateDungeonGraph(superMap, random);
        superMap = GenerateDungeonHallways(superMap, random);
        // superMap = GenerateDungeonZones(superMap, random);
        // superMap = PlaceDungeonObjects(superMap, random);
        // superMap = PlaceDungeonMobs(superMap, random);

        return superMap;
    }
}
