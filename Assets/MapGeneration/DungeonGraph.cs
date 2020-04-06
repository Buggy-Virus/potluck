using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph {
    static int[] Djikstra(int[,] graph, int origin) {
        bool[] visited = new bool[graph.GetLength(0)];
        int[] distanceFromOrigin = new int[graph.GetLength(0)];
        for (int i = 0; i < distanceFromOrigin.Count(); i++) {
            distanceFromOrigin[i] = int.MaxValue;
            visited[i] = false;
        }

        distanceFromOrigin[origin] = 0;

        List<Tuple<int, int>> assigned = new List<Tuple<int, int>>();
        assigned.Add(new Tuple<int, int>(origin, 0));

        while (assigned.Count() > 0) {
            int minIndex = -1;
            int minDist = int.MaxValue;
            for (int i = 0; i < distanceFromOrigin.Count(); i++) {
                if (!visited[i] && distanceFromOrigin[i] < minDist) {
                    minIndex = i;
                    minDist = distanceFromOrigin[i];
                }
            }

            if (minIndex == -1) {
                break;
            }

            for (int i = 0; i < graph.GetLength(0); i++) {
                if (graph[minIndex,i] == 1) {
                    int newDist = distanceFromOrigin[minIndex] + 1;
                    if (newDist < distanceFromOrigin[i]) {
                        distanceFromOrigin[i] = newDist;
                    }
                }
            }

            visited[minIndex] = true;
        }

        return distanceFromOrigin;
    }

    public static SuperMap InitializeRoomGraph(SuperMap superMap) {
        foreach (Node node in superMap.nodes) {
            foreach (Edge edge in superMap.edges) {
                superMap.roomGraph[node.id, edge.sink.id] = edge.type;
            }
        }

        return superMap;
    }

    public static SuperMap DefineZone(SuperMap superMap, int start, int zoneNum, List<int> exclude) {
        
    }

    public static SuperMap CreateZones(SuperMap superMap, List<int> goalNodes) {
        int origin = 0;
        foreach (Node node in superMap.nodes) {
            if (node.type == 0) {
                origin = node.id;
                break;
            }
        }

        int zoneNum = 0;
        superMap = DefineZone(superMap, origin, zoneNum, goalNodes); 

        return superMap;
    }

    public static SuperMap CreateEdges(SuperMap superMap) {

        return superMap;
    }

    public static SuperMap AddEdgeConditions(SuperMap superMap) {

        return superMap;
    }

}