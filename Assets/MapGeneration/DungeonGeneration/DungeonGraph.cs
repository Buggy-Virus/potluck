using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph {

    // Checks if all the nodes in a graph are connected
    static bool IsConnected(int[,] graph) {
        List<int> visited = new List<int>(){0};
        List<int> queued = new List<int>(){0};

        // If there are still nodes we've seen but haven't visited
        // Visit them
        while (queued.Count() > 0) {
            // mark it visited and remove it from the queue
            int current = queued[0];
            queued.RemoveAt(0);
            visited.Add(current);
            // Look at all the connections to the node, if it isn't visited
            // Add it to the queue
            for (int i = 0; i < graph.GetLength(0); i++) {
                if (graph[current, i] > 0 && !visited.Contains(i)) {
                    queued.Add(i);
                }
            }
        }

        // If we end up visiting all the nodes it's connected
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

    // Initialize the room graph, putting 1 if two nodes are in the same room
    public static SuperMap InitializeRoomGraph(SuperMap superMap) { // IDK about this, I have to go over graph again
        foreach (Room room in superMap.rooms) {
            foreach (Node a in room.nodes) {
                foreach (Node b in room.nodes) {
                    superMap.roomGraph[a.id, b.id] = 1;
                }
            }
        }

        return superMap;
    }

    // Pick a random portal in a zone to add an edge, the passed portals
    // is meant to tbe the current zone portals
    static Portal PickStartPortal(System.Random random, List<Portal> portals) {
        // currently we just grab a random one of the available
        Portal startPortal = portals[random.Next(0, portals.Count())];

        // if (random.NextDouble() < 1.0 / (1.0 + result.edgeCount)) {
        //     return result;
        // }

        return startPortal;
    }
    
    // Pick a portal in an edge to connect to a start portal
    static Portal PickEndPortal(SuperMap superMap, System.Random random, Portal startPortal, List<int> exclude) {
        // build a probability distribution to pick a new portal from the superMap portals
        double total = 0;
        List<(Portal, double)> cutoffs = new List<(Portal, double)>();

        foreach (Portal portal in superMap.portals) { // Could be made more efficient by tracking which portals are already in a different zone
            double distance = Utils.Distance(portal.point, startPortal.point); // could put this calculate after the if startment to cut down on computation
            // Check if it is within a reasonable distance, and the endpoint is not excluded, and the node either has no zone or is in the starts own zone
            if (distance < superMap.sparsityFactor && !exclude.Contains(portal.node.id) && (portal.node.zone == -1 || portal.node.zone == startPortal.node.zone)) {
                cutoffs.Add((portal, total));
                total += Math.Pow(distance, superMap.portalDistanceFactor);
            }
        }
        
        // If we didn't get any viable portals to add
        if (cutoffs.Count() == 0) {
            return startPortal; // returing startPortal for if can't be completed, kinda hacky
        }

        // Pick the endportal from the probability distribution
        double roll = random.NextDouble() * total;
        Portal endPortal = new Portal();
        foreach ((Portal, double) cutoff in cutoffs) {
            if (cutoff.Item2 < roll) {
                endPortal = cutoff.Item1;
            } else {
                break;
            }
        }

        return endPortal;
    }

    // Create a zone based on the superMaps target zone size, and connecting nearby nodes from a starting node
    // excludes a set of nodes if listed
    static SuperMap DefineZone(SuperMap superMap, System.Random random, int start, int zoneNum, int type, List<int> exclude) {
        // Pick the target total nodes in the zone based on the supermap parameter
        int totalNodes = superMap.targetZoneSize + random.Next(0, superMap.zoneRange) - superMap.zoneRange / 2;
        // Create the zone object
        Zone zone = new Zone(zoneNum, type);
        // Initialize a list of all portals in the zone
        List<Portal> zonePortals = new List<Portal>();

        // Add the first node to the zone
        Node currentNode = superMap.nodes[start];
        currentNode.zone = zoneNum;
        zone.nodes.Add(currentNode);
        // Add the first node's portals to the zonePortals
        zonePortals = zonePortals.Concat(currentNode.portals).ToList();

        int[,] adjacencyGraph = new int[totalNodes, totalNodes];

        // While the number of nodes is less than the target total nodes
        // and there are still zone portals to connect
        while (zone.nodes.Count() >= totalNodes && zonePortals.Count() > 0) {
            // Pick the next portal to connect
            Portal startPortal = PickStartPortal(random, zonePortals);
            // Pick an eligible portal to connect it to, and thus the next node to add
            Portal endPortal = PickEndPortal(superMap, random, startPortal, exclude);
            // If it connected with itself we couldn't find a good end portal, so that
            // portal is dead and we evict it as a potential start portal
            if (endPortal == startPortal) { // Couldn't find an end portal
                zonePortals.Remove(startPortal);
            } else {
                // If we got a good end portal we add a new edge between the two nodes
                Edge edge = new Edge(startPortal, endPortal);
                superMap.naiveGraph[startPortal.node.id, endPortal.node.id] += 1;
                superMap.naiveGraph[endPortal.node.id, startPortal.node.id] += 1;

                // If the endportal node was not already in the zone we add it
                if (endPortal.node.zone != zoneNum) {
                    endPortal.node.zone = zoneNum;
                    zone.nodes.Add(endPortal.node);
                    zonePortals = zonePortals.Concat(endPortal.node.portals).ToList();
                }
            }
        }

        // Calculate the midpoint based on the midpoints of all the zones
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

        // Add the new zone
        superMap.zones.Add(zone);
        return superMap;
    }

    // Add all the nodes in the supermap to a distinct zone
    // goalNodes don't get added to the same zone as the start
    public static SuperMap CreateZones(SuperMap superMap, System.Random random, List<int> goalNodes) {
        // set the origin index equal to the node with type matching the start node
        int origin = 0;
        foreach (Node node in superMap.nodes) {
            if (node.origin) {
                origin = node.id;
                break;
            }
        }

        // Create the initial zone with the start node
        int zoneNum = 0;
        superMap = DefineZone(superMap, random, origin, zoneNum, 0, goalNodes);

        // Go through each of the goalnodes and define zones around them if they already aren't part of one
        foreach (int goalNode in goalNodes) {
            if (superMap.nodes[goalNode].zone == -1) {
                zoneNum += 1;
                superMap = DefineZone(superMap, random, goalNode, zoneNum, 1, new List<int>());
            }
        }

        // Go through all the nodes and check if they are part of a zone, if not, define a zone around them
        for(int i = 0; i < superMap.nodes.Count(); i++) {
            if (superMap.nodes[i].zone == -1) {
                zoneNum += 1;
                superMap = DefineZone(superMap, random, i, zoneNum, 2, new List<int>());
            }
        }

        return superMap;
    }

    // From a superMap which has had its zones defined, then connect the zones in a way such that
    // There won' be sequence breaks and all zones will be accessible
    public static SuperMap CreateZoneGraph(SuperMap superMap, System.Random random) {
        // Create an adjacency matrix for the zones, also track all the edges between zones
        superMap.zoneGraph = new int[superMap.zones.Count(), superMap.zones.Count()];
        superMap.zoneEdgeGraph = new List<Edge>[superMap.zones.Count(), superMap.zones.Count()];

        // Set an initial distance threshold for how close two zones need to be, this cna be changed
        double distanceThreshold = superMap.sparsityFactor * 2;
        bool connected = false;

        // While the zone graph is not connected introduce more connections to the zone graph
        while (!connected) {
            // Consider a connection between every two zones
            foreach (Zone sourceZone in superMap.zones) {
                foreach (Zone sinkZone in superMap.zones) {
                    // If the source and sink zone are different, aren't connected, and their distance is below the distance threshold we add some edges
                    if (sourceZone.id != sinkZone.id && superMap.zoneGraph[sourceZone.id, sinkZone.id] == 0 && Utils.Distance(sourceZone.midPoint, sinkZone.midPoint) < distanceThreshold) { // Distance threshold check should have some randomness added to it
                        // Roll to see how many edges between the two zones we're going to add between the zones
                        int edgeRoll = superMap.targetZoneEdges + (random.Next(0, superMap.zoneEdgesRange) - superMap.zoneEdgesRange / 2);
                        superMap.zoneGraph[sourceZone.id, sinkZone.id] = edgeRoll;
                        superMap.zoneGraph[sinkZone.id, sourceZone.id] = edgeRoll;

                        // Create a probability distribution across the source's portals
                        double sourceTotal = 0;
                        List<Tuple<Portal, double>> sourceCutoffs = new List<Tuple<Portal, double>>();
                        foreach(Node node in sourceZone.nodes) {
                            foreach(Portal portal in node.portals) {
                                sourceCutoffs.Add(new Tuple<Portal, double>(portal, sourceTotal));
                                // Weight it by the distance between the portal and the sink midpoint
                                sourceTotal += Math.Pow(1.0 / Utils.Distance(sinkZone.midPoint, portal.point), superMap.portalDistanceFactor);
                            }
                        }

                        // Create a probability distribution across the sink's portals
                        double sinkTotal = 0;
                        List<Tuple<Portal, double>> sinkCutoffs = new List<Tuple<Portal, double>>();
                        foreach(Node node in sinkZone.nodes) {
                            foreach(Portal portal in node.portals) {
                                sourceCutoffs.Add(new Tuple<Portal, double>(portal, sinkTotal));
                                // Weight it by the distane between the portal and the source midpoint
                                sinkTotal += Math.Pow(1.0 / Utils.Distance(sourceZone.midPoint, portal.point), superMap.portalDistanceFactor);
                            }
                        }
                        
                        // For the number of connections we rolled there would be
                        // Randomly select a sink and source from their probability distributions
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

                            // Add an edge between the two portals, and between the zones
                            Edge edge = new Edge(sourcePortal, sinkPortal);
                            if (sourceZone.id < sinkZone.id) {
                                superMap.zoneEdgeGraph[sourceZone.id, sinkZone.id].Add(edge);
                            } else {
                                superMap.zoneEdgeGraph[sinkZone.id, sourceZone.id].Add(edge);
                            }
                        }
                    }
                }
            }

            // After going through all the possible connections, check if it's connected  and update connected
            // Up the sparsity factor so that we'll look further out for making connections the second time around
            // If everything isn't connected
            distanceThreshold += superMap.sparsityFactor;
            connected = IsConnected(superMap.zoneGraph);
        }        

        return superMap;
    }

    // Paint zone edges between two zones with conditionals such that there is still a possible naive path through the dungeon
    static SuperMap PaintZoneEdges(SuperMap superMap, System.Random random, int currentZone, List<Edge> edges, List<int> AccessibleZones, bool first) {
        // we can tune how we choose the conditionals
        // 1 naive
        // 2 keyed
        // 3 key in zone
        // 4 skill based
        // 5 unlockable one way
        // 6 one directional slide
        // List<Tuple<double, int>> cutoffs = new  List<Tuple<double, int>>(){ // This should be kicked out to utilities so it isn't created every time
        //     new Tuple<double, int>(0, 1),
        //     new Tuple<double, int>(0.08, 2),
        //     new Tuple<double, int>(0.50, 3),
        //     new Tuple<double, int>(0.60, 4),
        //     new Tuple<double, int>(0.70, 5),
        //     new Tuple<double, int>(0.85, 6)
        // }; // This should be something in the superMap

        // If it's the first we have it either naive or keyed so that it is accessible
        if (first) {
            // Pick a random edge And remove it from the edges to paint
            Edge firstEdge = edges[random.Next(0, edges.Count())];
            edges.Remove(firstEdge);

            // Roll for edge type, for first we only consider edges under 0.5
            double firstRoll = random.NextDouble() * 0.5;
            int firstConditional = 0;
            foreach (KeyValuePair<int, double> cutoff in superMap.zoneEdgeWeights) {
                if (firstRoll > cutoff.Value) {
                    firstConditional = cutoff.Key;
                } else {
                    break;
                }
            }
            firstEdge.conditional = firstConditional;

            // If the conditional is keyed, add the key in one of the currently accessible zones
            if (firstConditional == 2) {
                firstEdge.key = superMap.currentKey;
                superMap.zones[AccessibleZones[random.Next(0, AccessibleZones.Count())]].keys.Add(superMap.currentKey);
                superMap.currentKey += 1;
            }
        }

        // Do it for the rest of the edges, now allowing edges that aren't necessarily accessible
        while (edges.Count() > 0) {
            // Pick a random edge And remove it from the edges to paint
            Edge edge = edges[random.Next(0, edges.Count())];
            edges.Remove(edge);

            // Roll for edge type
            double firstRoll = random.NextDouble();
            int conditional = 0;
            foreach (KeyValuePair<int, double> cutoff in superMap.zoneEdgeWeights) {
                if (firstRoll > cutoff.Value) {
                    conditional = cutoff.Key;
                } else {
                    break;
                }
            } 
            edge.conditional = conditional;

            // If the conditional is keyed, add the key in one of the currently accessible zones
            if (conditional == 2) {
                edge.key = superMap.currentKey;
                superMap.zones[AccessibleZones[random.Next(0, AccessibleZones.Count())]].keys.Add(superMap.currentKey);
                superMap.currentKey += 1;
            } else if (conditional == 3) {
                // Adding the key in zone instead
                edge.key = superMap.currentKey;
                superMap.zones[currentZone].keys.Add(superMap.currentKey);
                superMap.currentKey += 1;
            }
        }

        return superMap;
    }

    // For all the zones add conditionals on the connecting edges, such that they can all be accessed
    public static SuperMap AddEdgeConditions(SuperMap superMap, System.Random random) {
        bool[] visited = new bool[superMap.zones.Count()];
        // Initialize accessible zones with the original zone accessible
        List<int> accessibleZones = new List<int>{0};
        // Initialize the origin as visited
        visited[0] = true;

        // queue all the zones connected to the origin zone
        List<int> queued = new List<int>();
        for (int i = 0; i < superMap.zoneGraph.GetLength(0); i++) {
            if (superMap.zoneGraph[0,i] != 0) {
                queued.Add(i);
            }
        }
        
        // While the queue is not empty, add paint the edges to all adjacent zones which have been setup
        while (queued.Count() > 0) {
            // Pick one at random from the queued
            int current = queued[random.Next(0, queued.Count())];
            queued.Remove(current);

            List<int> adjacentAccessible = new List<int>();

            // If not visited and it's connected add it to the queue
            for (int i = 0; i < superMap.zoneGraph.GetLength(0); i++) {
                if (!visited[i] && superMap.zoneGraph[current, i] != 0) {
                    queued.Add(i);
                } else if (superMap.zoneGraph[current, i] != 0) {
                    adjacentAccessible.Add(i);
                }
            }

            // Paint the edges of all adjacent already accessible zones
            bool first = true;
            while (adjacentAccessible.Count() > 0) {
                int adjacentZone = adjacentAccessible[random.Next(0, adjacentAccessible.Count())];
                if (current < adjacentZone) {
                    superMap = PaintZoneEdges(superMap, random, current, superMap.zoneEdgeGraph[current, adjacentZone], accessibleZones, first);
                } else {
                    superMap = PaintZoneEdges(superMap, random, current, superMap.zoneEdgeGraph[adjacentZone, current], accessibleZones, first);
                }
                adjacentAccessible.Remove(adjacentZone);
                first = false;
            }

            // Set as visited, and add it as now an accesible edge
            visited[current] = true;
            accessibleZones.Add(current);
            
        }

        return superMap;
    }

}