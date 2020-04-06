using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration {

    // Generates and places rooms within the dungeon
    static SuperMap GenerateDungeonRooms(SuperMap superMap, System.Random random) {

        // Pick and place first room for required rooms
        int firstRequiredRoomInded = random.Next(0, superMap.requiredRooms.Count);
        Tuple<int, Point> firstRequiredRoom = superMap.requiredRooms[firstRequiredRoomInded];
        RoomPrefab firstRoomPrefab = DungeonRoom.GetRoomPrefab(firstRequiredRoom.Item1);
        superMap.requiredRooms.RemoveAt(firstRequiredRoomInded);
        RoomSkeleton firstRoomSkeleton = DungeonRoom.GenerateRoomSkeleton(random, firstRoomPrefab, new Tuple<Point, Point>(new Point(0,0,0), new Point(superMap.xSize, superMap.ySize, superMap.zSize)));
        superMap = DungeonRoom.PlaceRoom(superMap, firstRoomSkeleton, new Point(0, 0, 0), firstRoomPrefab);

        // Repeatedly add new rooms until all required rooms are placed and total rooms exceeds target room count
        while(superMap.requiredRooms.Count > 0 && superMap.roomCount >= superMap.targetRoomCount) {
            Point nextRoomLocation = DungeonRoom.PickNextLocation(superMap, random);
            Tuple<Point, Point> nextRoomBounds = DungeonRoom.MeasureFreeSpace(superMap, nextRoomLocation);

            int nextRoomType = DungeonRoom.PickNextRoom(superMap, random, nextRoomBounds, nextRoomLocation);
            RoomPrefab nextRoomPrefab = DungeonRoom.GetRoomPrefab(nextRoomType);
            RoomSkeleton nextRoomSkeleton = DungeonRoom.GenerateRoomSkeleton(random, nextRoomPrefab, nextRoomBounds);
            Point nextRoomAnchorPoint = DungeonRoom.PickAnchorPoint(superMap, random, nextRoomLocation, nextRoomBounds, nextRoomSkeleton); 
            superMap = DungeonRoom.PlaceRoom(superMap, nextRoomSkeleton, nextRoomAnchorPoint, nextRoomPrefab);
        }

        return superMap;
    }

    static SuperMap GenerateDungeonGraph(SuperMap superMap, System.Random random) {
        superMap.naiveGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];
        superMap.roomGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];
        superMap.specialGraph = new int[superMap.nodes.Count(), superMap.nodes.Count()];

        DungeonGraph.InitializeRoomGraph(superMap);

        return superMap;
    }

    static SuperMap GenerateDungeonHallways(SuperMap superMap, System.Random random) {

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
