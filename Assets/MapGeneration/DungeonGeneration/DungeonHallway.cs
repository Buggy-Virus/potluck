using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonHallway {
    static bool CheckStraight(Dictionary<Point, Point> previous, Point adjacent) {
        return (adjacent.x != previous[adjacent].x && previous[adjacent].x != previous[previous[adjacent]].x) ||
               (adjacent.y != previous[adjacent].y && previous[adjacent].y != previous[previous[adjacent]].y) ||
               (adjacent.z != previous[adjacent].z && previous[adjacent].z != previous[previous[adjacent]].z);
    }

    static bool CheckStraight(Dictionary<Point, Point> previous, Point adjacent, Point current) {
        return (adjacent.x != current.x && previous[current].x != previous[previous[current]].x) ||
               (adjacent.y != current.y && previous[current].y != previous[previous[current]].y) ||
               (adjacent.z != current.z && previous[current].z != previous[previous[current]].z);
    }

    static (Dictionary<Point, int>, Dictionary<Point, Point>, HashSet<Point>, HashSet<Point>) UpdateAdjacent(Dictionary<Point, int> distance, Dictionary<Point, Point> previous, HashSet<Point> visitedNodes, HashSet<Point> queuedNodes, SuperMap superMap, Edge edge, Point current, Point adjacent) {
        if (!visitedNodes.Contains(adjacent)) {
            if (Utils.CheckEmpty(superMap.skeleton, adjacent + new Point(-edge.width, 0, -edge.width), adjacent + new Point(edge.width, edge.height, edge.width))) {
                int newDistance = distance[current] + 1;
                if (!distance.ContainsKey(adjacent) || distance[adjacent] >= newDistance) {
                    queuedNodes.Add(adjacent);
                    if (!previous.ContainsKey(adjacent) || distance[adjacent] != newDistance || !CheckStraight(previous, adjacent)) {
                        previous[adjacent] = current;
                    }
                    distance[adjacent] = newDistance;
                }
            } else {
                visitedNodes.Add(adjacent);
            }
        }
        
        return (distance, previous, visitedNodes, queuedNodes);
    }

    static Point GetMinimumQueued(Dictionary<Point, int> distance, HashSet<Point> queuedNodes) {
        int min = int.MaxValue;
        Point minPoint = new Point();
        foreach (Point point in queuedNodes) {
            if (distance[point] < min) {
                min = distance[point];
                minPoint = point;
            }
        }
        
        return minPoint;
    }

    static List<Point> GetPath(Dictionary<Point, Point> previous, Point endPoint) {
        List<Point> path = new List<Point>() {endPoint};
        Point current = endPoint;
        while(previous.ContainsKey(current)) {
            path.Add(previous[current]);
            current = previous[current];
        }
        path.Reverse();

        return path;
    }

    static List<Point> DjikstraHallway (SuperMap superMap, Edge edge, Point start, Point end) {
        Dictionary<Point, int> distance = new Dictionary<Point, int>();
        Dictionary<Point, Point> previous = new Dictionary<Point, Point>();
        HashSet<Point> visitedNodes = new HashSet<Point>();
        HashSet<Point> queuedNodes = new HashSet<Point>();

        distance[start] = 0;
        Point current = start; 
        queuedNodes.Add(start);

        while(!visitedNodes.Contains(end) && queuedNodes.Count() > 0) {
            visitedNodes.Add(current);
            queuedNodes.Remove(current);
            int newDistance = distance[current] + 1;

            (distance, previous, visitedNodes, queuedNodes) = UpdateAdjacent(distance, previous, visitedNodes, queuedNodes, superMap, edge, current, new Point(current.x + 1, current.y, current.z));
            (distance, previous, visitedNodes, queuedNodes) = UpdateAdjacent(distance, previous, visitedNodes, queuedNodes, superMap, edge, current, new Point(current.x, current.y + 1, current.z));
            (distance, previous, visitedNodes, queuedNodes) = UpdateAdjacent(distance, previous, visitedNodes, queuedNodes, superMap, edge, current, new Point(current.x, current.y, current.z + 1));
            (distance, previous, visitedNodes, queuedNodes) = UpdateAdjacent(distance, previous, visitedNodes, queuedNodes, superMap, edge, current, new Point(current.x - 1, current.y, current.z));
            (distance, previous, visitedNodes, queuedNodes) = UpdateAdjacent(distance, previous, visitedNodes, queuedNodes, superMap, edge, current, new Point(current.x, current.y - 1, current.z));
            (distance, previous, visitedNodes, queuedNodes) = UpdateAdjacent(distance, previous, visitedNodes, queuedNodes, superMap, edge, current, new Point(current.x, current.y, current.z - 1));

            current = GetMinimumQueued(distance, queuedNodes);
        }

        if (visitedNodes.Contains(end)) {
            return GetPath(previous, end);
        } else {
            return new List<Point>();
        }
    }

    static SuperMap DrawEdge(SuperMap superMap, Edge edge) {
        foreach(Point point in edge.path) {
            superMap.skeleton = DrawSegmentDirection(superMap.skeleton, point, edge, 1);
        }

        return superMap;
    }

    static bool CheckEmpty(int[,,] map, Edge startEdge, Edge endEdge, Point a, Point b) {
        if (a.x < 0 || a.y < 0 || a.z < 0 || b.x < 0 || b.y < 0 || b.z < 0 || 
            a.x >= map.GetLength(0) || a.y >= map.GetLength(1) || a.z >= map.GetLength(2) ||
            b.x >= map.GetLength(0) || b.y >= map.GetLength(1) || b.z >= map.GetLength(2)) {
            return false;
        }

        for (int i = Math.Min(a.x, b.x); i <= Math.Max(a.x, b.x); i++) {
            for (int j = Math.Min(a.y, b.y); j <= Math.Max(a.y, b.y); j++) {
                for (int k = Math.Min(a.z, b.z); k <= Math.Max(a.z, b.z); k++) {
                    Point point = new Point(i, j, k);
                    if(map[i, j, k] != 0 && !startEdge.points.Contains(point) && !endEdge.points.Contains(point)) {
                        Debug.Log("Conflict at x = " + i + " y = " + j + " z = " + k);
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // Set of methods to check whether space is empty in a specific direction off a point
    delegate bool CheckDirectionMethod (int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current);
    static bool CheckDirection1(int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current) {
        Point point00 = new Point(current.x + 1, current.y, current.z - edge.width);
        Point point11 = new Point(current.x + 1, current.y + edge.height, current.z + edge.width);
        return CheckEmpty(skeleton, startEdge, endEdge, point00, point11) && !pathPoints[current.x + 1, current.y, current.z];
    }

    static bool CheckDirection2(int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y + 1, current.z - edge.width);
        Point point11 = new Point(current.x + edge.width, current.y + 1, current.z + edge.width);
        return CheckEmpty(skeleton, startEdge, endEdge, point00, point11) && !pathPoints[current.x, current.y + 1, current.z];
    }

    static bool CheckDirection3(int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y, current.z + 1);
        Point point11 = new Point(current.x + edge.width, current.y + edge.height, current.z + 1);
        return CheckEmpty(skeleton, startEdge, endEdge, point00, point11) && !pathPoints[current.x, current.y, current.z + 1];
    }

    static bool CheckDirection4(int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current) {
        Point point00 = new Point(current.x - 1, current.y, current.z - edge.width);
        Point point11 = new Point(current.x - 1, current.y + edge.height, current.z + edge.width);
        return CheckEmpty(skeleton, startEdge, endEdge, point00, point11) && !pathPoints[current.x - 1, current.y, current.z];
    }

    static bool CheckDirection5(int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y - 1, current.z - edge.width);
        Point point11 = new Point(current.x + edge.width, current.y - 1, current.z + edge.width);
        return CheckEmpty(skeleton, startEdge, endEdge, point00, point11)  && !pathPoints[current.x, current.y - 1, current.z];
    }

    static bool CheckDirection6(int[,,] skeleton, bool[,,] pathPoints, Edge edge, Edge startEdge, Edge endEdge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y, current.z - 1);
        Point point11 = new Point(current.x + edge.width, current.y + edge.height, current.z - 1);
        return CheckEmpty(skeleton, startEdge, endEdge, point00, point11)  && !pathPoints[current.x, current.y, current.z - 1];
    }

    static List<CheckDirectionMethod> CheckDirections = new List<CheckDirectionMethod>() {
        CheckDirection1, 
        CheckDirection2,
        CheckDirection3,
        CheckDirection4,
        CheckDirection5,
        CheckDirection6 
    };

    delegate int[,,] DrawSegmentMethod (int[,,] skeleton, Point point, Edge edge, int value);

    static int[,,] DrawSegmentDirection(int[,,] skeleton, Point point, Edge edge, int value) {
        return DrawSegmentSkeleton(skeleton, edge, point + new Point(-edge.width, 0, -edge.width), point + new Point(edge.width, edge.height, edge.width), value);
    }

    static int[,,] DrawSegmentDirection14(int[,,] skeleton, Point point, Edge edge, int value) {
        return DrawSegmentSkeleton(skeleton, edge, point + new Point(0, 0, -edge.width), point + new Point(0, edge.height, edge.width), value);
    }

    static int[,,] DrawSegmentDirection25(int[,,] skeleton, Point point, Edge edge, int value) {
        return  DrawSegmentSkeleton(skeleton, edge, point + new Point(-edge.width, 0, -edge.width), point + new Point(edge.width, 0, edge.width), value);
    }

    static int[,,] DrawSegmentDirection36(int[,,] skeleton, Point point, Edge edge, int value) {
        return DrawSegmentSkeleton(skeleton, edge, point + new Point(-edge.width, 0, 0), point + new Point(edge.width, edge.height, 0), value);
    }

    static List<DrawSegmentMethod> DrawSegments= new List<DrawSegmentMethod>() {
        DrawSegmentDirection14,
        DrawSegmentDirection25,
        DrawSegmentDirection36,
        DrawSegmentDirection14,
        DrawSegmentDirection25,
        DrawSegmentDirection36
    };

    static bool[,,] FlipPoint(bool[,,] points, Point point) {
        points[point.x, point.y, point.z] = !points[point.x, point.y, point.z];
        return points;
    }

    delegate (int, int, int) UpdateDistancesMethod(int xLeft, int yLeft, int zLeft);

    static (int, int, int) UpdateDistances1(int xLeft, int yLeft, int zLeft) {
        xLeft -= 1;
        return (xLeft, yLeft, zLeft);
    }

    static (int, int, int) UpdateDistances2(int xLeft, int yLeft, int zLeft) {
        yLeft -= 1;
        return (xLeft, yLeft, zLeft);
    }

    static (int, int, int) UpdateDistances3(int xLeft, int yLeft, int zLeft) {
        zLeft -= 1;
        return (xLeft, yLeft, zLeft);
    }

    static (int, int, int) UpdateDistances4(int xLeft, int yLeft, int zLeft) {
        xLeft += 1;
        return (xLeft, yLeft, zLeft);
    }

    static (int, int, int) UpdateDistances5(int xLeft, int yLeft, int zLeft) {
        yLeft += 1;
        return (xLeft, yLeft, zLeft);
    }

    static (int, int, int) UpdateDistances6(int xLeft, int yLeft, int zLeft) {
        zLeft += 1;
        return (xLeft, yLeft, zLeft);
    }

    static List<UpdateDistancesMethod> UpdateDistancesMethods = new List<UpdateDistancesMethod>() {
        UpdateDistances1,
        UpdateDistances2,
        UpdateDistances3,
        UpdateDistances4,
        UpdateDistances5,
        UpdateDistances6
    };

    delegate Point NextPointMethod (Point current);

    static Point NextPoint1(Point current) {
        return current + new Point(1, 0, 0);
    } 

    static Point NextPoint2(Point current) {
        return current + new Point(0, 1, 0);
    } 

    static Point NextPoint3(Point current) {
        return current + new Point(0, 0, 1);
    } 

    static Point NextPoint4(Point current) {
        return current + new Point(-1, 0, 0);
    } 

    static Point NextPoint5(Point current) {
        return current + new Point(0, -1, 0);
    } 

    static Point NextPoint6(Point current) {
        return current + new Point(0, 0, -1);
    } 

    static List<NextPointMethod> NextPointMethods = new List<NextPointMethod>() {
        NextPoint1,
        NextPoint2,
        NextPoint3,
        NextPoint4,
        NextPoint5,
        NextPoint6
    };

    delegate int[,,] CapPathMethod (int[,,] skeleton, Edge edge, Point point, int height, int width, int value);

    static int[,,] CapPath1(int[,,] skeleton, Edge edge, Point point, int height, int width, int value) {
        for (int i = 1; i <= width; i++) {
            skeleton = DrawSegmentDirection14(skeleton, point + new Point(i, 0, 0), edge, value);
        }
        return skeleton;
    }

    static int[,,] CapPath2(int[,,] skeleton, Edge edge, Point point, int height, int width, int value) {
        for (int i = 1; i <= height; i++) {
            skeleton = DrawSegmentDirection25(skeleton, point + new Point(0, i, 0), edge, value);
        }
        return skeleton;
    }

    static int[,,] CapPath3(int[,,] skeleton, Edge edge, Point point, int height, int width, int value) {
        for (int i = 1; i <= width; i++) {
            skeleton = DrawSegmentDirection36(skeleton, point + new Point(0, 0, i), edge, value);
        }
        return skeleton;
    }

    static int[,,] CapPath4(int[,,] skeleton, Edge edge, Point point, int height, int width, int value) {
        for (int i = 1; i <= width; i++) {
            skeleton = DrawSegmentDirection14(skeleton, point + new Point(-i, 0, 0), edge, value);
        }
        return skeleton;
    }

    static int[,,] CapPath5(int[,,] skeleton, Edge edge, Point point, int height, int width, int value) {
        return skeleton;
    }

    static int[,,] CapPath6(int[,,] skeleton, Edge edge, Point point, int height, int width, int value) {
        for (int i = 1; i <= width; i++) {
            skeleton = DrawSegmentDirection36(skeleton, point + new Point(0, 0, -i), edge, value);
        }
        return skeleton;
    }

    static List<CapPathMethod> CapPathMethods = new List<CapPathMethod>() {
        CapPath1,
        CapPath2,
        CapPath3,
        CapPath4,
        CapPath5,
        CapPath6
    };

    static int[,,] DrawSegmentSkeleton(int[,,] hallwaySkeleton, Edge edge, Point a, Point b, int value) {
        for (int i = Math.Min(a.x, b.x); i <= Math.Max(a.x, b.x); i++) {
            for (int j = Math.Min(a.y, b.y); j <= Math.Max(a.y, b.y); j++) {
                for (int k = Math.Min(a.z, b.z); k <= Math.Max(a.z, b.z); k++) {
                    hallwaySkeleton[i, j, k] += value;
                    int newValue = hallwaySkeleton[i, j, k];
                    Point point = new Point(i, j, k);
                    if (newValue > 0 && !edge.points.Contains(point)) {
                        edge.points.Add(point);
                    } else if (newValue <= 0 && edge.points.Contains(point)) {
                        edge.points.Remove(point);
                    }
                }
            }
        }

        return hallwaySkeleton;
    }

    static int[,,] SetSegmentSkeleton(int[,,] hallwaySkeleton, Point a, Point b, int value) {
        for (int i = Math.Min(a.x, b.x); i <= Math.Max(a.x, b.x); i++) {
            for (int j = Math.Min(a.y, b.y); j <= Math.Max(a.y, b.y); j++) {
                for (int k = Math.Min(a.z, b.z); k <= Math.Max(a.y, b.z); k++) {
                    hallwaySkeleton[i, j, k] = value;
                }
            }
        }

        return hallwaySkeleton;
    }

    // Try for a path to move forward
    static (bool, Edge, int[,,], bool[,,], int, int, int) AttemptForward(SuperMap superMap, Edge edge, int[,,] hallwaySkeleton, bool[,,] hallwayPoints, Edge endEdge, Edge startEdge, int xLeft, int yLeft, int zLeft, Point endPoint) {
        bool wentForward = true;
        Point current = edge.path.Last();
        int direction = edge.directions.Last();
        // Check the durrent direction, and try to go forward. If it goes forward we update wentForward
        if (CheckDirections[direction - 1](superMap.skeleton, hallwayPoints, edge, startEdge, endEdge, current)) {
            int directionIndex = direction - 1;
            Point newPoint = NextPointMethods[directionIndex](current);
            hallwaySkeleton = DrawSegmentDirection(hallwaySkeleton, newPoint, edge, 1);
            (xLeft, yLeft, zLeft) = UpdateDistancesMethods[directionIndex](xLeft, yLeft, zLeft);
            edge.path.Add(newPoint);
            hallwayPoints = FlipPoint(hallwayPoints, newPoint);
            edge.directions.Add(direction);
        } else {
            wentForward = false;
            return (wentForward, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft);
        }

        return (wentForward, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft);
    }

    // Try for a point to turn
    static (bool, Edge, int[,,], bool[,,], int, int, int, Dictionary<Point, List<int>>) AttemptTurn(SuperMap superMap, Edge edge, int[,,] hallwaySkeleton, bool[,,] hallwayPoints, Edge endEdge, Edge startEdge, int xLeft, int yLeft, int zLeft, bool favorable, Dictionary<Point, List<int>> failedTurns) {
        Point current = edge.path.Last();
        int direction = edge.directions.Last();

        // Create scores for how desirable a direction is based upon how much distance is left
        int[] turnScores = new int[]{xLeft, yLeft, zLeft, -xLeft, -yLeft, -zLeft};
        
        // can't turn forward or backward
        turnScores[direction - 1] = int.MinValue;
        turnScores[(direction + 2) % 6] = int.MinValue;
        int availableDirections = 4;

        if (favorable) {
            for (int i = 0; i < turnScores.Count(); i++) {
                if (turnScores[i] <= 0 && turnScores[i] != int.MinValue) {
                    turnScores[i] = int.MinValue;
                    availableDirections -= 1;
                }
            }
        }
        
        if (failedTurns.ContainsKey(current)) {
            foreach (int failedDirection in failedTurns[current]) {
                turnScores[failedDirection - 1] = int.MinValue;
                availableDirections -= 1;
            }
        }

        // Look for a good turn until we exhaust all possible directions
        // the turn direction with the best score is chosen first
        // if we exhaust all available direction foundTurn will remain
        // false and be returned
        bool foundTurn = false;
        while (availableDirections > 0 && !foundTurn) {
            // Find the direction with the best score
            int max = int.MinValue;
            int maxIndex = -1;
            for (int i = 0; i < turnScores.Count(); i++) {
                if (turnScores[i] > max) {
                    max = turnScores[i];
                    maxIndex = i;
                }
            }
            
            // check if we can turn in the direction of the best available direction
            if (CheckDirections[maxIndex](superMap.skeleton, hallwayPoints, edge, startEdge, endEdge, current)) {
                // If so, we found a turn and update the direction, and move the path
                // one unit in the new direction
                foundTurn = true;
                edge.directions.Add(maxIndex + 1);
                Point next = NextPointMethods[maxIndex](current);
                edge.path.Add(next);
                hallwayPoints = FlipPoint(hallwayPoints, next);
                hallwaySkeleton = DrawSegmentDirection(hallwaySkeleton, next, edge, 1);
                (xLeft, yLeft, zLeft) = UpdateDistancesMethods[maxIndex](xLeft, yLeft, zLeft);
            } else {
                // Otherwise that's one less available direction, and we don't check
                // that direction again
                availableDirections -= 1;
                turnScores[maxIndex] = int.MinValue;
                // Add it to failed turns so we don't check it again if we end up here in the future
                if (failedTurns.ContainsKey(current)) {
                    failedTurns[current].Add(maxIndex + 1);
                } else {
                    failedTurns[current] = new List<int>() {maxIndex + 1};
                }
            }
        }

        if (foundTurn && favorable) {
            Debug.Log("Found Favorable Turn");
        }

        return (foundTurn, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft, failedTurns);
    }

    // Go back along the path, update whether a possible pathways are deadends
    static (Edge, int[,,], bool[,,], int, int, int, Dictionary<Point, List<int>>) BackTrack(Edge edge, int[,,] hallwaySkeleton, bool[,,] hallwayPoints, int xLeft, int yLeft, int zLeft, Dictionary<Point, List<int>> failedTurns) {
        // If the direction changes during the backtrack, that means we were forced to backtrack a
        // turn, meaning it was a failed turn
        int lastDirection = edge.directions.Last();
        Point lastPoint = edge.path.Last();

        if (edge.path.Count() > 1) {
            Point pivotPoint = edge.path[edge.path.Count() - 2];
            int pivotDirection = edge.directions[edge.directions.Count() - 2];
            if (lastDirection != pivotDirection) {
                if (failedTurns.ContainsKey(pivotPoint)) {
                    failedTurns[pivotPoint].Add(lastDirection);
                } else {
                    failedTurns[pivotPoint] = new List<int>() {lastDirection};
                }
            }
        }

        hallwaySkeleton = DrawSegmentDirection(hallwaySkeleton, lastPoint, edge, -1);
        (xLeft, yLeft, zLeft) = UpdateDistancesMethods[(lastDirection + 3) % 6](xLeft, yLeft, zLeft);
        hallwayPoints = FlipPoint(hallwayPoints, lastPoint);
        edge.path.RemoveAt(edge.path.Count() - 1);
        edge.directions.RemoveAt(edge.directions.Count() - 1);

        return (edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft, failedTurns);
    }

    // Return if the current direction is bringing the path closer to the end
    static bool GoodDirection(int direction, int xLeft, int yLeft, int zLeft) {
        return (direction == 1 && xLeft > 0) || 
               (direction == 2 && yLeft > 0) || 
               (direction == 3 && zLeft > 0) || 
               (direction == 4 && xLeft < 0) || 
               (direction == 5 && yLeft < 0) ||
               (direction == 6 && zLeft < 0);
    }

    static bool GoodAdjacent(int direction, int xLeft, int yLeft, int zLeft) {
        return (direction == 1 && (yLeft != 0 || zLeft != 0)) || 
               (direction == 2 && (xLeft != 0 || zLeft != 0)) || 
               (direction == 3 && (xLeft != 0 || yLeft != 0)) || 
               (direction == 4 && (yLeft != 0 || zLeft != 0)) || 
               (direction == 5 && (xLeft != 0 || zLeft != 0)) ||
               (direction == 6 && (xLeft != 0 || yLeft != 0));
    }

    static Point GetClosestSegment(Portal portal, Point point) {
        Debug.Log("GetClosestSegment");
        Point closestPoint = new Point();
        double minDistance = int.MaxValue;
        Debug.Log(portal.connectingEdge.path.Count());
        foreach (Point segment in portal.connectingEdge.path) {
            double currentDistance = Utils.Distance(segment, point);
            if (currentDistance < minDistance) {
                closestPoint = segment;
                minDistance = currentDistance;
            }
        }

        return closestPoint;
    }

    static Point GetPortalAdjacent(Portal portal) {
        if (portal.direction == 1) {
            return new Point(1, 0, 0) + portal.point;
        } else if (portal.direction == 2) {
            return new Point(0, 1, 0) + portal.point;
        } else if (portal.direction == 3) {
            return new Point(0, 0, 1) + portal.point;
        } else if (portal.direction == 4) {
            return new Point(-1, 0, 0) + portal.point;
        } else if (portal.direction == 5) {
            return new Point(0, -1, 0) + portal.point;
        } else {
            return new Point(0, 0, -1) + portal.point;
        }
    }

    // Create a path through the supermap between two portals
    static int[,,] CreateEdgePath(SuperMap superMap, Edge edge) {
        Debug.Log("Starting CreateEdgePath");

        int[,,] hallwaySkeleton = new int[0,0,0];
        bool[,,] hallwayPoints;

        Point startPoint = new Point();
        Edge startEdge = new Edge();
        Point endPoint = new Point();
        Edge endEdge = new Edge();
        int startDirect = edge.source.direction;

        // If the source edge is already setup
        // if setup, then instead of the startpoint being the source portal start
        // we find the edge that actually hits the start portal, and then make the
        // point with the minimum distance between that edge on the sink portal the start point
        if (edge.source.setup) {
            startPoint = GetClosestSegment(edge.source, edge.sink.point);
            startEdge = edge.source.connectingEdge;
        } else {
            startPoint = GetPortalAdjacent(edge.source);
            edge.source.connectingEdge = edge;
            edge.source.setup = true;
            edge.hitSource = true;
        }

        // Similar thing with the sink portal
        if (edge.sink.setup) {
            endPoint = GetClosestSegment(edge.sink, startPoint);
            endEdge = edge.sink.connectingEdge;
        } else {
            endPoint = GetPortalAdjacent(edge.sink);
            edge.sink.connectingEdge = edge;
            edge.sink.setup = true;
            edge.hitSink = true;
        }

        Debug.Log("startPoint x = " + startPoint.x + ", y = " + startPoint.y + ", z = " + startPoint.z);
        Debug.Log("endPoint x = " + endPoint.x + ", y = " + endPoint.y + ", z = " + endPoint.z);

        // While running to imply we were able to find a good path
        // if we can't find a good path we try the pathfinding algo
        // again but with a thinner tunnel
        bool running = true;
        int runningCounter = 0;
        while (running && runningCounter < 2) {
            runningCounter += 1;
            Debug.Log("Edge Path Running");
            hallwaySkeleton = new int[superMap.xSize,superMap.ySize,superMap.zSize];
            hallwayPoints = new bool[superMap.xSize,superMap.ySize,superMap.zSize];
            edge.path.Add(startPoint);
            edge.directions.Add(edge.source.direction);
            hallwaySkeleton =  DrawSegments[edge.source.direction - 1](hallwaySkeleton, startPoint, edge, 1);
            hallwayPoints = FlipPoint(hallwayPoints, startPoint);

            // Set the distance left until we reach our goal
            int xLeft = endPoint.x - startPoint.x;
            int yLeft = endPoint.y - startPoint.y;
            int zLeft = endPoint.z - startPoint.z;

            bool tryForward = true;
            bool tryFavorableTurn = true;
            bool tryTurn = true;
            Dictionary<Point, List<int>> failedTurns = new Dictionary<Point, List<int>>();

            int pathCounter = 0;
            while ((xLeft != 0 || yLeft != 0 || zLeft != 0) && edge.path.Count() > 0 && pathCounter < 200) {
                Debug.Log("xLeft " + xLeft + " yLeft " + yLeft + " zLeft " + zLeft);
                pathCounter++;
                if (tryForward && GoodDirection(edge.directions.Last(), xLeft, yLeft, zLeft)) {
                    Debug.Log("Good TF");
                    (tryForward, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft) = AttemptForward(superMap, edge, hallwaySkeleton, hallwayPoints, endEdge, startEdge, xLeft, yLeft, zLeft, edge.path.Last());
                    if (tryForward) {
                        tryFavorableTurn = true;
                        tryTurn = true;
                    }
                } else if (tryFavorableTurn) {
                    Debug.Log("Good TT");
                    (tryFavorableTurn, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft, failedTurns) = AttemptTurn(superMap, edge, hallwaySkeleton, hallwayPoints, endEdge, startEdge, xLeft, yLeft, zLeft, true, failedTurns);
                    if (tryFavorableTurn) {
                        tryForward = true;
                    }
                } else if (tryForward && GoodAdjacent(edge.directions.Last(), xLeft, yLeft, zLeft)) {
                    Debug.Log("OK TF");
                    (tryForward, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft) = AttemptForward(superMap, edge, hallwaySkeleton, hallwayPoints, endEdge, startEdge, xLeft, yLeft, zLeft, edge.path.Last());
                    if (tryForward) {
                        tryFavorableTurn = true;
                        tryTurn = true;
                    }
                } else if (tryTurn) {
                    Debug.Log("TT");
                    (tryTurn, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft, failedTurns) = AttemptTurn(superMap, edge, hallwaySkeleton, hallwayPoints, endEdge, startEdge, xLeft, yLeft, zLeft, false, failedTurns);
                    if (tryTurn) {
                        tryFavorableTurn = true;
                        tryForward = true;
                    }
                } else if (tryForward) {
                    Debug.Log("TF");
                    (tryForward, edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft) = AttemptForward(superMap, edge, hallwaySkeleton, hallwayPoints, endEdge, startEdge, xLeft, yLeft, zLeft, edge.path.Last());
                    if (tryForward) {
                        tryFavorableTurn = true;
                        tryTurn = true;
                    }
                } else {
                    Debug.Log("Backtrack");
                    (edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft, failedTurns) = BackTrack(edge, hallwaySkeleton, hallwayPoints, xLeft, yLeft, zLeft, failedTurns);
                    tryTurn = true;
                }
            }

            // pathfinding algo is complete, check if it drew a path
            // If it didn't and we are at min height/width give up
            // else shrink the tunnel width
            // if it did then set running to false and complete
            if (edge.path.Count() == 0 || pathCounter >= 100) {
                if (edge.height == 0 && edge.width == 0) {
                    Debug.Log("Failed to create path");
                    edge.connectable = false; // We should try to do something cooler than this
                    running = false;
                } else {
                    Debug.Log("Path too thick, thinning path and trying again");
                    edge.height = Math.Max(0, edge.height - 1);
                    edge.width = Math.Max(0, edge.width - 1);
                }
            } else {
                Debug.Log("Successfully generated path");
                running = false;
            }
        }

        return hallwaySkeleton;
    }

    public static SuperMap PlaceHallway(SuperMap superMap, int[,,] hallwaySkeleton) {
        // This isn't super reasonable at the moment, it doesn't have walls and currently draws in
        // what is in the hallway skeleton
        for (int i = 0; i < superMap.xSize; i++) {
            for (int j = 0; j < superMap.ySize; j++) {
                for (int k = 0; k < superMap.zSize; k++) {
                    if (hallwaySkeleton[i, j, k] > 0) {
                        superMap.skeleton[i, j, k] = 1;
                    }
                }
            }
        }

        return superMap;
    } 

    // Create an edge path for each edge in the superMap
    public static SuperMap CreateEdgePaths(SuperMap superMap) {
        Debug.Log("Starting Create Edge Paths");

        Dictionary<Edge, bool> setupEdges = new Dictionary<Edge, bool>();

        foreach (Node node in superMap.nodes) {
            foreach (Edge edge in node.edges) {
                if (!setupEdges.ContainsKey(edge)) {
                    setupEdges[edge] = true;
                    int[,,] hallwaySkeleton;
                    hallwaySkeleton = CreateEdgePath(superMap, edge);

                    superMap = PlaceHallway(superMap, hallwaySkeleton);
                }
            }
        }

        return superMap;
    }
}