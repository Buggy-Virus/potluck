using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph {
    static bool IsConnected(int[,] graph) {
        List<int> visited = new List<int>(){0};
        List<int> queued = new List<int>();
        for (int i = 0; i < graph.GetLength(0); i++) {
            if (graph[0,i] > 0) {
                visited.Add(i);
                queued.Add(i);
            }
        }

        while (queued.Count() > 0) {
            int current = queued[0];
            queued.RemoveAt(0);
            for (int i = 0; i < graph.GetLength(0); i++) {
                if (graph[0,i] > 0 && !visited.Contains(i)) {
                    visited.Add(i);
                    queued.Add(i);
                }
            }
        }

        return visited.Count() == graph.GetLength(0);
    }

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
                Edge edge = new Edge(startPortal, endPortal);
                superMap.naiveGraph[startPortal.node.id, endPortal.node.id] += 1;
                superMap.naiveGraph[endPortal.node.id, startPortal.node.id] += 1;

                if (endPortal.node.zone != zoneNum) {
                    endPortal.node.zone = zoneNum;
                    zone.nodes.Add(endPortal.node);
                    zonePortals = zonePortals.Concat(endPortal.node.portals).ToList();
                }
            }
        }

        int xMid = 0;
        int yMid = 0;
        int zMid = 0;
        foreach (Node node in zone.nodes) {
            xMid += node.midPoint.x;
            yMid += node.midPoint.y;
            zMid += node.midPoint.z;
        }
        xMid /= zone.nodes.Count();
        yMid /= zone.nodes.Count();
        zMid /= zone.nodes.Count();
        zone.midPoint = new Point(xMid, yMid, zMid);

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

    public static SuperMap CreateZoneGraph(SuperMap superMap, System.Random random) {
        superMap.zoneGraph = new int[superMap.zones.Count(), superMap.zones.Count()];
        superMap.zoneEdgeGraph = new List<Edge>[superMap.zones.Count(), superMap.zones.Count()];

        double distanceThreshold = superMap.sparsityFactor * 2;
        bool connected = false;

        while (!connected) {
            foreach (Zone sourceZone in superMap.zones) {
                foreach (Zone sinkZone in superMap.zones) {
                    if (superMap.zoneGraph[sourceZone.id, sinkZone.id] == 0 && Utils.Distance(sourceZone.midPoint, sinkZone.midPoint) < distanceThreshold) { // Distance threshold check should have some randomness added to it
                        int edgeRoll = superMap.targetZoneEdges + (random.Next(0, superMap.zoneEdgesRange) - superMap.zoneEdgesRange / 2);
                        superMap.zoneGraph[sourceZone.id, sinkZone.id] = edgeRoll;
                        superMap.zoneGraph[sinkZone.id, sourceZone.id] = edgeRoll;

                        double sourceTotal = 0;
                        List<Tuple<Portal, double>> sourceCutoffs = new List<Tuple<Portal, double>>();
                        foreach(Node node in sourceZone.nodes) {
                            foreach(Portal portal in node.portals) {
                                sourceCutoffs.Add(new Tuple<Portal, double>(portal, sourceTotal));
                                sourceTotal += Math.Pow(1.0 / Utils.Distance(sinkZone.midPoint, portal.point), 0.7); // This metric could be changed or tuned
                            }
                        }

                        double sinkTotal = 0;
                        List<Tuple<Portal, double>> sinkCutoffs = new List<Tuple<Portal, double>>();
                        foreach(Node node in sinkZone.nodes) {
                            foreach(Portal portal in node.portals) {
                                sourceCutoffs.Add(new Tuple<Portal, double>(portal, sinkTotal));
                                sinkTotal += Math.Pow(1.0 / Utils.Distance(sourceZone.midPoint, portal.point), 0.7); // This metric could be changed or tuned
                            }
                        }
                        
                        for (int i = 0; i < edgeRoll; i++) {
                            double sourecRoll = random.NextDouble() * sourceTotal;
                            Portal sourcePortal = new Portal();
                            foreach (Tuple<Portal, double> cutoff in sourceCutoffs) {
                                if (sourecRoll > cutoff.Item2) {
                                    sourcePortal = cutoff.Item1;
                                } else {
                                    break;
                                }
                            }

                            double sinkRoll = random.NextDouble() * sinkTotal;
                            Portal sinkPortal = new Portal();
                            foreach (Tuple<Portal, double> cutoff in sinkCutoffs) {
                                if (sinkRoll > cutoff.Item2) {
                                    sinkPortal = cutoff.Item1;
                                } else {
                                    break;
                                }
                            }

                            Edge edge = new Edge(sourcePortal, sinkPortal);
                            if (sourceZone.id < sinkZone.id) {
                                superMap.zoneEdgeGraph[sourceZone.id, sinkZone.id].Add(edge);
                            } else {
                                superMap.zoneEdgeGraph[sinkZone.id, sourceZone.id].Add(edge);
                            }
                        }
                    }
                }

                distanceThreshold += superMap.sparsityFactor;
                connected = IsConnected(superMap.zoneGraph);
            }
        }        

        return superMap;
    }

    public static SuperMap AddEdgeConditions(SuperMap superMap) {

        return superMap;
    }

}