using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point {
    public int x;
    public int y;
    public int z;

    public Point(int i, int j,int k) {
        this.x = i;
        this.y = j;
        this.z = k;
    }

    public static bool operator ==(Point a, Point b) {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }   

    public static bool operator !=(Point a, Point b) {
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }   
}