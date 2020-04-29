using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RoomPrefab {
    public int xMax;
    public int yMax;
    public int zMax;

    public int xMin;
    public int yMin;
    public int zMin;

    public bool hasPreference;
    public double preference;
    public bool hasDispreference;
    public double dispreference;
    public double weight;

    public int type;
    public List<int> requiredRooms;
    public RoomSkeletonMethod roomSkeletonMethod;

    public RoomPrefab(int type, int xMax, int yMax, int zMax, int xMin, int yMin, int zMin, bool hasPreference, double preference, bool hasDispreference, double dispreference, double weight, List<int> requiredRooms, RoomSkeletonMethod roomSkeletonMethod) {
        this.type = type;
        
        this.xMax = xMax;
        this.yMax = yMax;
        this.zMax = zMax;
        this.xMin = xMin;
        this.yMin = yMin;
        this.zMin = zMin;

        this.hasPreference = hasPreference;
        this.preference = preference;
        this.hasDispreference = hasDispreference;
        this.dispreference = dispreference;
        this.weight = weight;
        
        this.requiredRooms = requiredRooms;
        this.roomSkeletonMethod = roomSkeletonMethod;
    }
}
