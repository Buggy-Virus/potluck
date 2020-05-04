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

    public static Point operator +(Point a, Point b) {
        return new Point(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public override int GetHashCode() {
        return 37 * x + 31 * y + 29 * z;
    }

    public override bool Equals(object obj) {
        return obj is Point && Equals((Point) obj);
    }

    public bool Equals(Point p) {
        return x == p.x && y == p.y && z == p.z;
    }
}