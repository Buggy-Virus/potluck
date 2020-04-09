using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
    public static double Distance(Point a, Point b) {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
    }

    public static Point Average(Point a, Point b) {
        return new Point((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
    }
}