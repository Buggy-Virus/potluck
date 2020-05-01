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

    public static bool CheckEmpty(SuperMap superMap, Point a, Point b) {
        if (a.x < 0 || a.y < 0 || a.z < 0 || b.x < 0 || b.y < 0 || b.z < 0 || 
            a.x >= superMap.xSize || a.y >= superMap.ySize || a.z >= superMap.zSize ||
            b.x >= superMap.xSize || b.y >= superMap.ySize || b.z >= superMap.zSize) {
            return false;
        }

        for (int i = Math.Min(a.x, b.x); i <= Math.Max(a.x, b.x); i++) {
            for (int j = Math.Min(a.y, b.y); j <= Math.Max(a.y, b.y); j++) {
                for (int k = Math.Min(a.z, b.z); k <= Math.Max(a.y, b.z); k++) {
                    if(superMap.skeleton[i, j, k] != 0) {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public static bool CheckEmpty(int[,,] map, Point a, Point b) {
        if (a.x < 0 || a.y < 0 || a.z < 0 || b.x < 0 || b.y < 0 || b.z < 0 || 
            a.x >= map.GetLength(0) || a.y >= map.GetLength(1) || a.z >= map.GetLength(2) ||
            b.x >= map.GetLength(0) || b.y >= map.GetLength(1) || b.z >= map.GetLength(2)) {
            return false;
        }

        for (int i = Math.Min(a.x, b.x); i <= Math.Max(a.x, b.x); i++) {
            for (int j = Math.Min(a.y, b.y); j <= Math.Max(a.y, b.y); j++) {
                for (int k = Math.Min(a.z, b.z); k <= Math.Max(a.y, b.z); k++) {
                    if(map[i, j, k] != 0) {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}