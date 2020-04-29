using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomPrefabs {
    static RoomPrefab simpleOrigin = new RoomPrefab(1, true, false, 
                                                  8, 8, 8, 
                                                  5, 5, 5, 
                                                  false, 0.0, false, 0.0, 1.0, 
                                                  new List<int>(), 
                                                  DungeonRoomSkeleton.SimpleOriginSkeleton);

    static RoomPrefab simpleGoal = new RoomPrefab(2, false, true, 
                                                  10, 10, 10, 
                                                  6, 6, 6, 
                                                  false, 0.0, false, 0.0, 1.0, 
                                                  new List<int>(), 
                                                  DungeonRoomSkeleton.SimpleGoalSkeleton);

    static RoomPrefab simpleRoom = new RoomPrefab(3, false, false, 
                                                  6, 6, 6, 
                                                  4, 4, 4, 
                                                  false, 0.0, false, 0.0, 1.0, 
                                                  new List<int>(), 
                                                  DungeonRoomSkeleton.SimpleRoomSkeleton);

    public static Dictionary<int, RoomPrefab> typeToRoomPrefab = new Dictionary<int, RoomPrefab>() {
        {1, simpleOrigin},
        {2, simpleGoal},
        {3, simpleRoom}
    };
}