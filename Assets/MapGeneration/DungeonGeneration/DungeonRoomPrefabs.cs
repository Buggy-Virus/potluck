using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomPrefabs {
    static RoomPrefab simpleRoom = new RoomPrefab(20, 20, 20, 5, 5, 5, 0, false, 0.0, false, 0.0, 0.0, new List<int>(), DungeonRoomSkeleton.SimpleRoomSkeleton);

    public static Dictionary<int, RoomPrefab> typeToRoomPrefab = new Dictionary<int, RoomPrefab>() {
        {0, simpleRoom}
    };
}