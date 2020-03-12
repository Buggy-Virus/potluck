using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct ReqRoom {
    public int roomType;
    public Vector3 depRoomPoint;
}

struct Room {
    public List<int> reqRooms;
}

struct RoomWeight {
    public int roomType;
    public float weight;
}

public class DungeonGeneration {

    static Dictionary<int, Room> rooms = new Dictionary<int, Room>{

    };

    static int pickFirstRoom(List<int> roomQueue, System.Random random) {

    }

    static int pickRoom(Vector3 origin, Vector3 end, List<int> roomQueue, List<RoomWeight> roomWeights, System.Random random) {

    }

    static int[,,] placeRoom(Vector3 origin, Vector3 end, Room room, System.Random random) {

    }

    static public int[,,] GenerateDungeonSkeleton(int height, int length, int width, System.Random random) {
        int[,,] skeleton = new int[length, height, width];

        float complexity;

        Dictionary<int, int> roomMaxes;
        Dictionary<int, int> roomCounts;

        List<ReqRoom> requiredRooms;

        List<RoomWeight> roomWeights; 

        int firstRoom = pickFirstRoom(roomQueue, random);


    }
}
