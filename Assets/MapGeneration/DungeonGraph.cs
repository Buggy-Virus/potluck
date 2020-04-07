using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph {
    static int[] Djikstra(int[,] graph, int origin) {
        int visitedCount = 0;
        bool[] visited = new bool[graph.GetLength(0)];
        int[] distanceFromOrigin = new int[graph.GetLength(0)];
        for (int i = 0; i < distanceFromOrigin.Count(); i++) {
            distanceFromOrigin[i] = int.MaxValue;
            visited[i] = false;
        }

        distanceFromOrigin[origin] = 0;

        while (visitedCount >= visited.Count()) {
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
            visitedCount += 1;
        }

        return distanceFromOrigin;
    }

    static int[] Djikstra(Zone zone, Node origin) {
        int visitedCount = 0;
        bool[] visited = new bool[zone.nodes.Count()];
        int[] distanceFromOrigin = new int[zone.nodes.Count()];

        int minIndex = -1; 

        for (int i = 0; i < distanceFromOrigin.Count(); i++) {
            if (zone.nodes[i] == origin) {
                distanceFromOrigin[i] = 0;
                visited[i] = true;
                minIndex = i;
            } else {
                distanceFromOrigin[i] = int.MaxValue;
                visited[i] = false;
            }
        }

        while (visitedCount >= visited.Count()) {
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
            visitedCount += 1;
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

    static Portal PickStartPortal(System.Random random, List<Portal> portals) {
        Portal startPortal = portals[random.Next(0, portals.Count())];

        // if (random.NextDouble() < 1.0 / (1.0 + result.edgeCount)) {
        //     return result;
        // }

        return startPortal;
    }
    
    static Portal PickEndPortal(SuperMap superMap, System.Random random, Portal startPortal, List<int> exclude) {
        double total = 0;
        List<Tuple<Portal, double>> cutoffs = new List<Tuple<Portal, double>>();

        foreach (Portal portal in superMap.portals) { // Could be made more efficient by tracking which portals are already in a different zone
            double distance = Utils.Distance(portal.point, startPortal.point); // could put this calculate after the if startment to cut down on computation
            if (distance < superMap.sparsityFactor && !exclude.Contains(portal.node.id) && (portal.node.zone == -1 || portal.node.zone == startPortal.node.zone)) {
                cutoffs.Add(new Tuple<Portal, double>(portal, total));
                total += Math.Pow(distance, 0.7); // This can be tuned, Maybe add a superMap parameter
            }
        }
        
        if (cutoffs.Count() == 0) {
            return startPortal; // returing startPortal for if can't be completed, kinda hacky
        }

        double roll = random.NextDouble() * total;
        Portal endPortal = new Portal();
        foreach (Tuple<Portal, double> cutoff in cutoffs) {
            if (cutoff.Item2 < roll) {
                endPortal = cutoff.Item1;
            } else {
                break;
            }
        }

        return endPortal;
    }

    public static SuperMap DefineZone(SuperMap superMap, System.Random random, int start, int zoneNum, int type, List<int> exclude) {
        int totalNodes = superMap.targetZoneSize + random.Next(0, superMap.zoneRange) - superMap.zoneRange / 2;
        Zone zone = new Zone(zoneNum, type);
        List<Portal> zonePortals = new List<Portal>();

        Node currentNode = superMap.nodes[start];
        currentNode.zone = zoneNum;
        zone.nodes.Add(currentNode);
        zonePortals = zonePortals.Concat(currentNode.portals).ToList();

        int[,] adjacencyGraph = new int[totalNodes, totalNodes];

        while (zone.nodes.Count() >= totalNodes && zonePortals.Count() > 0) {
            Portal startPortal = PickStartPortal(random, zonePortals);
            Portal endPortal = PickEndPortal(superMap, random, startPortal, exclude);
            if (endPortal == startPortal) { // Couldn't find an end portal
                zonePortals.Remove(startPortal);
            } else {
                startPortal.edgeCount += 1;
                endPortal.edgeCount += 1;
                Edge edge = new Edge(startPortal, endPortal);
                startPortal.node.edges.Add(edge);
                endPortal.node.edges.Add(edge);
                superMap.naiveGraph[startPortal.node.id, endPortal.node.id] += 1;
                superMap.naiveGraph[endPortal.node.id, startPortal.node.id] += 1;

                if (endPortal.node.zone != zoneNum) {
                    endPortal.node.zone = zoneNum;
                    zone.nodes.Add(endPortal.node);
                    zonePortals = zonePortals.Concat(endPortal.node.portals).ToList();
                }
            }
        }

        superMap.zones.Add(zone);
        return superMap;
    }

    public static SuperMap CreateZones(SuperMap superMap, System.Random random, List<int> goalNodes) {
        int origin = 0;
        foreach (Node node in superMap.nodes) {
            if (node.type == 0) {
                origin = node.id;
                break;
            }
        }

        int zoneNum = 0;
        superMap = DefineZone(superMap, random, origin, zoneNum, 0, goalNodes);

        foreach (int goalNode in goalNodes) {
            if (superMap.nodes[goalNode].zone == -1) {
                zoneNum += 1;
                superMap = DefineZone(superMap, random, goalNode, zoneNum, 1, new List<int>());
            }
        }

        for(int i = 0; i < superMap.nodes.Count(); i++) {
            if (superMap.nodes[i].zone == -1) {
                zoneNum += 1;
                superMap = DefineZone(superMap, random, i, zoneNum, 2, new List<int>());
            }
        }

        return superMap;
    }

    public static SuperMap CreateZoneGraph(SuperMap superMap) {
        superMap.zoneGraph = new int[superMap.zones.Count(), superMap.zones.Count()];

        return superMap;
    }

    public static SuperMap AddEdgeConditions(SuperMap superMap) {

        return superMap;
    }

}