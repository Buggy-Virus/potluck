using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration {

    // Generates and places rooms within the dungeon
    static SuperMap GenerateDungeonRooms(SuperMap superMap, System.Random random) {

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

        // Repeatedly add new rooms until all required rooms are placed and total rooms exceeds target room count
        while(superMap.requiredRooms.Count > 0 && superMap.roomCount >= superMap.targetRoomCount) {
            Point nextRoomLocation = DungeonRoom.PickNextLocation(superMap, random);
            (Point, Point) nextRoomBounds = DungeonRoom.MeasureFreeSpace(superMap, nextRoomLocation);
            
            // Pick the next room, if it isn't a legitimate pick, keep going, the face will have been evicted so we're ok
            int nextRoomType = DungeonRoom.PickNextRoom(superMap, random, nextRoomBounds, nextRoomLocation);
            if (nextRoomType != -1) {
                RoomPrefab nextRoomPrefab = DungeonRoom.GetRoomPrefab(nextRoomType);
                RoomSkeleton nextRoomSkeleton = DungeonRoomSkeleton.GenerateRoomSkeleton(random, nextRoomPrefab, nextRoomBounds);
                Point nextRoomAnchorPoint = DungeonRoom.PickAnchorPoint(superMap, random, nextRoomLocation, nextRoomBounds, nextRoomSkeleton); 
                superMap = DungeonRoom.PlaceRoom(superMap, nextRoomSkeleton, nextRoomAnchorPoint, nextRoomPrefab);
            }
        }

        return superMap;
    }

    // Generate the dungeon graph of zones, connections between zones, and connection between rooms
    static SuperMap GenerateDungeonGraph(SuperMap superMap, System.Random random) {
        superMap.naiveGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];
        superMap.roomGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];
        superMap.specialGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];

        superMap = DungeonGraph.InitializeRoomGraph(superMap);
        List<int> goalNodes = new List<int>();
        foreach (Node node in superMap.nodes) {
            if (node.goal) {
                goalNodes.Add(node.id);
            }
        }
        superMap = DungeonGraph.CreateZones(superMap, random, goalNodes);
        superMap = DungeonGraph.CreateZoneGraph(superMap, random);
        superMap = DungeonGraph.AddEdgeConditions(superMap, random);

        return superMap;
    }

    static SuperMap GenerateDungeonHallways(SuperMap superMap, System.Random random) {
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
        superMap = GenerateDungeonRooms(superMap, random);
        superMap = GenerateDungeonGraph(superMap, random);
        superMap = GenerateDungeonHallways(superMap, random);
        superMap = GenerateDungeonZones(superMap, random);
        superMap = PlaceDungeonObjects(superMap, random);
        superMap = PlaceDungeonMobs(superMap, random);

        return superMap;
    }
}
