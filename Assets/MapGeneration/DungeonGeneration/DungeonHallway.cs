using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonHallway {

    // Set of methods to check whether space is empty in a specific direction off a point
    public delegate bool CheckDirectionMethod (SuperMap superMap, Edge edge, Point current);
    static bool CheckDirection1(SuperMap superMap, Edge edge, Point current) {
        Point point00 = new Point(current.x + 1, current.z - edge.width, current.y);
        Point point11 = new Point(current.x + 1, current.z + edge.width, current.y + edge.height);
        return Utils.CheckEmpty(superMap, point00, point11);
    }

    static bool CheckDirection2(SuperMap superMap, Edge edge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y + 1, current.z - edge.width);
        Point point11 = new Point(current.x + edge.width, current.y + 1, current.z + edge.width);
        return Utils.CheckEmpty(superMap, point00, point11);
    }

    static bool CheckDirection3(SuperMap superMap, Edge edge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y, current.z + 1);
        Point point11 = new Point(current.x + edge.width, current.y + edge.height, current.z + 1);
        return Utils.CheckEmpty(superMap, point00, point11);
    }

    static bool CheckDirection4(SuperMap superMap, Edge edge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y + 1, current.z - edge.width);
        Point point11 = new Point(current.x + edge.width, current.y + 1, current.z + edge.width);
        return Utils.CheckEmpty(superMap, point00, point11);
    }

    static bool CheckDirection5(SuperMap superMap, Edge edge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y - 1, current.z - edge.width);
        Point point11 = new Point(current.x + edge.width, current.y - 1, current.z + edge.width);
        return Utils.CheckEmpty(superMap, point00, point11);
    }

    static bool CheckDirection6(SuperMap superMap, Edge edge, Point current) {
        Point point00 = new Point(current.x - edge.width, current.y, current.z - 1);
        Point point11 = new Point(current.x + edge.width, current.y + edge.height, current.z - 1);
        return Utils.CheckEmpty(superMap, point00, point11);
    }

    static List<CheckDirectionMethod> CheckDirections = new List<CheckDirectionMethod>() {
        CheckDirection1, 
        CheckDirection2,
        CheckDirection3,
        CheckDirection4,
        CheckDirection5,
        CheckDirection6 
    };

    // Try for a path to move forward
    static (SuperMap, Edge, bool, int, int, int) AttemptForward(SuperMap superMap, Edge edge, int xLeft, int yLeft, int zLeft, Point endPoint) {
        bool wentForward = true;
        Point current = edge.path.Last();
        int direction = edge.directions.Last();
        if (direction == 1 && CheckDirection1(superMap, edge, current) || new Point(current.x + 1, current.y, current.z) == endPoint) {
            edge.path.Add(new Point(current.x + 1, current.y, current.z));
            edge.directions.Add(direction);
            xLeft -= 1;
        } else if (direction == 2 && CheckDirection2(superMap, edge, current) || new Point(current.x, current.y + 1, current.z) == endPoint) {
            edge.path.Add(new Point(current.x, current.y + 1, current.z));
            edge.directions.Add(direction);
            yLeft -= 1;
        } else if (direction == 3 && CheckDirection3(superMap, edge, current) || new Point(current.x, current.y, current.z + 1) == endPoint) {
            edge.path.Add(new Point(current.x, current.y, current.z + 1));
            edge.directions.Add(direction);
            zLeft -= 1;
        } else if (direction == 4 && CheckDirection4(superMap, edge, current) || new Point(current.x + 1, current.y, current.z) == endPoint) {
            edge.path.Add(new Point(current.x - 1, current.y, current.z));
            edge.directions.Add(direction);
            xLeft += 1;
        } else if (direction == 5 && CheckDirection5(superMap, edge, current) || new Point(current.x, current.y + 1, current.z) == endPoint) {
            edge.path.Add(new Point(current.x, current.y - 1, current.z));
            edge.directions.Add(direction);
            yLeft += 1;
        } else if (direction == 6 && CheckDirection6(superMap, edge, current) || new Point(current.x, current.y, current.z + 1) == endPoint) {
            edge.path.Add(new Point(current.x, current.y, current.z - 1));
            edge.directions.Add(direction);
            zLeft += 1;
        } else {
            wentForward = false;
        }

        return (superMap, edge, wentForward, xLeft, yLeft, zLeft);
    }

    // Try for a point to turn
    static (SuperMap, Edge, bool, int, int, int, Dictionary<Point, List<int>>) AttemptTurn(SuperMap superMap, Edge edge, int xLeft, int yLeft, int zLeft, Dictionary<Point, List<int>> failedTurns) {
        Point current = edge.path.Last();
        int direction = edge.directions.Last();
        bool hasFailedTurns = failedTurns.ContainsKey(current);

        int xAbsLeft = Math.Abs(xLeft);
        int yAbsLeft = Math.Abs(yLeft);
        int zAbsLeft = Math.Abs(zLeft);

        int[] turnScores = new int[]{xLeft, yLeft, zLeft, -xLeft, -yLeft, -zLeft};
        turnScores[direction - 1] = int.MinValue;
        int availableDirections = 5;
        if (hasFailedTurns) {
            foreach (int failedDirection in failedTurns[current]) {
                turnScores[failedDirection - 1] = int.MinValue;
                availableDirections -= 1;
            }
        }

        bool foundTurn = false;
        while (availableDirections > 0 && !foundTurn) {
            int max = int.MinValue;
            int maxIndex = -1;
            for (int i = 0; i < turnScores.Count(); i++) {
                if (turnScores[i] > max) {
                    max = turnScores[i];
                    maxIndex = i;
                }
            }
            
            if (CheckDirections[maxIndex](superMap, edge, current)) {
                foundTurn = true;
                direction = maxIndex + 1;
                edge.directions.Add(direction);
                if (maxIndex == 0) {
                    edge.path.Add(new Point(current.x + 1, current.y, current.z));
                    xLeft -= 1;
                } else if (maxIndex == 1) {
                    edge.path.Add(new Point(current.x, current.y + 1, current.z));
                    yLeft -= 1;
                } else if (maxIndex == 2) {
                    edge.path.Add(new Point(current.x, current.y, current.z + 1));
                    zLeft -= 1;
                } else if (maxIndex == 3) {
                    edge.path.Add(new Point(current.x - 1, current.y, current.z));
                    xLeft += 1;
                } else if (maxIndex == 4) {
                    edge.path.Add(new Point(current.x, current.y - 1, current.z));
                    yLeft += 1;
                } else if (maxIndex == 5) {
                    edge.path.Add(new Point(current.x, current.y, current.z - 1));
                    zLeft += 1;
                }
            } else {
                availableDirections -= 1;
                turnScores[maxIndex] = int.MinValue;
                if (hasFailedTurns) {
                    failedTurns[current].Add(maxIndex + 1);
                } else {
                    failedTurns[current] = new List<int>() {maxIndex + 1};
                }
            }
        }

        return (superMap, edge, foundTurn, xLeft, yLeft, zLeft, failedTurns);
    }

    // Go back along the path
    static (Edge, int, int, int, Dictionary<Point, List<int>>) BackTrack(Edge edge, int xLeft, int yLeft, int zLeft, Dictionary<Point, List<int>> failedTurns) {
        edge.path.RemoveAt(edge.path.Count() - 1);
        edge.directions.RemoveAt(edge.directions.Count() - 1);

        return (edge, xLeft, yLeft, zLeft, failedTurns);
    }

    // Return if the current direction is bringing the path closer to the end
    static bool GoodDirection(int direction, int xLeft, int yLeft, int zLeft) {
        bool result = (direction != 1 || xLeft > 0);
        result &= (direction != 2 || yLeft > 0);
        result &= (direction != 3 || zLeft > 0);
        result &= (direction != 4 || xLeft < 0);
        result &= (direction != 5 || yLeft < 0);
        result &= (direction != 6 || zLeft < 0);
        return result;
    }

    // Create a path through the supermap between two portals
    static SuperMap CreateEdgePath(SuperMap superMap, Edge edge) {
        Point startPoint = edge.source.point;
        Point endPoint = edge.sink.point;
        int startDirect = edge.source.direction;

        // If the source edge is already setup
        // if setup, then instead of the startpoint being the source portal start
        // we find the edge that actually hits the start portal, and then make the
        // point with the minimum distance between that edge on the sink portal the start point
        if (edge.source.setup) {
            // Find the edge connected to the portal
            Edge connectorEdge = new Edge();
            foreach (Edge sourceEdge in edge.source.edges) {
                if (sourceEdge.path[0] == edge.source.point || sourceEdge.path.Last() == edge.source.point) {
                    connectorEdge = sourceEdge;
                    break;
                }
            }

            // Find the point with the minimum distance
            double minDistance = int.MaxValue;
            foreach (Point point in connectorEdge.path) {
                double currentDistance = Utils.Distance(point, endPoint);
                if (currentDistance < minDistance) {
                    startPoint = point;
                    minDistance = currentDistance;
                }
            }
        } 

        // Similar thing with the sink portal
        if (edge.sink.setup) {
            // Find the edge connected to the portal
            Edge connectorEdge = new Edge();
            foreach (Edge sinkEdge in edge.sink.edges) {
                if (sinkEdge.path[0] == edge.sink.point || sinkEdge.path.Last() == edge.source.point) {
                    connectorEdge = sinkEdge;
                    break;
                }
            }

            // Find the point with the minimum distance
            double minDistance = int.MaxValue;
            foreach (Point point in connectorEdge.path) {
                double currentDistance = Utils.Distance(point, startPoint);
                if (currentDistance < minDistance) {
                    endPoint = point;
                    minDistance = currentDistance;
                }
            }
        }

        // While running to imply we were able to find a good path
        // if we can't find a good path we try the pathfinding algo
        // again but with a thinner tunnel
        bool running = true;
        while (running) {
            edge.path.Add(startPoint);
            edge.directions.Add(edge.source.direction);

            // Set the distance left until we reach our goal
            int xLeft = endPoint.x - startPoint.x;
            int yLeft = endPoint.y - startPoint.y;
            int zLeft = endPoint.z - startPoint.z;

            bool tryForward = true;
            bool tryTurn = true;
            bool notBacktracking = true;
            Dictionary<Point, List<int>> failedTurns = new Dictionary<Point, List<int>>();

            // As long as there is still distance to go, and we haven't erased the whole path
            // Continue
            // The whole path only gets erased if there is no possible path and we end up backtracking
            // over the start point
            // If we turn in a direction, but we end up needing to backtrack back over the turn, it's recorded
            // As a failed turn, so we never attempt to turn in that direction again, this keeps us from endless
            // Attempting to draw paths and creates scenarios where we can't find an available path and backtrack to the start
            while ((xLeft != 0 || yLeft != 0 || zLeft != 0) && edge.path.Count() > 0) {
                if (!notBacktracking) {
                    // If we are backtracking, then backtrack once and attempt a turn
                    // If the attempt a turn fails we will continue backtracking, otherwise we will investigate
                    // the turn
                    (edge, xLeft, yLeft, zLeft, failedTurns) = BackTrack(edge, xLeft, yLeft, zLeft, failedTurns);
                    (superMap, edge, notBacktracking, xLeft, yLeft, zLeft, failedTurns) = AttemptTurn(superMap, edge, xLeft, yLeft, zLeft, failedTurns);
                } else if ((GoodDirection(edge.directions.Last(), xLeft, yLeft, zLeft) && tryForward) || (tryForward && !tryTurn)) {
                    // If we have a good direction, and can go forward, or if we don't have a good direction, but can't turn
                    // try to go forward
                    (superMap, edge, tryForward, xLeft, yLeft, zLeft) = AttemptForward(superMap, edge, xLeft, yLeft, zLeft, endPoint);
                    if (tryForward) {
                        // if going forward succeeds, reset whether we can try turning to true
                        // We are in a new location
                        tryTurn = true;
                    }
                } else if (tryTurn) {
                    // Otherwise try to turn
                    (superMap, edge, tryTurn, xLeft, yLeft, zLeft, failedTurns) = AttemptTurn(superMap, edge, xLeft, yLeft, zLeft, failedTurns);
                    if (tryTurn) {
                        // if turning succeeds then reset trying forward to true
                        tryForward = true;
                    }
                } else {
                    // if we can't go forward or turn, start backtracking
                    notBacktracking = false;
                }
            }

            // pathfinding algo is complete, check if it drew a path
            // If it didn't and we are at min height/width give up
            // else shrink the tunnel width
            // if it did then set running to false and complete
            if (edge.path.Count() == 0) {
                if (edge.height == 0 && edge.width == 0) {
                    edge.connectable = false; // We should try to do something cooler than this
                    running = false;
                } else {
                    edge.height = Math.Max(0, edge.height - 1);
                    edge.width = Math.Max(0, edge.width - 1);
                }
            } else {
                running = false;
            }
        }

        return superMap;
    }

    // Create an edge path for each edge in the superMap
    public static SuperMap CreateEdgePaths(SuperMap superMap) {
        foreach (Node node in superMap.nodes) {
            foreach (Edge edge in node.edges) {
                if (edge.source.node == node) {
                    superMap = CreateEdgePath(superMap, edge);
                }
            }
        }

        return superMap;
    }

    static int[,,] DrawHallwaySpaceSegment(int[,,] skeleton, Point point, int direction, int width, int height, int value) {
        if (direction == 1 || direction == 4) {
            Point point00 = new Point(point.x, point.y, point.z - width);
            Point point11 = new Point(point.x, point.y + height, point.z + width);
            for (int y = point00.y; y <= point11.y; y++) {
                for (int z = point00.z; z <= point11.z; z++) {
                    skeleton[point00.x, y, z] = value;
                }
            }
        } else if (direction == 2 || direction == 5) {
            Point point00 = new Point(point.x - width, point.y, point.z - width);
            Point point11 = new Point(point.x + width, point.y, point.z + width);
            for (int x = point00.x; x <= point11.x; x++) {
                for (int z = point00.z; z <= point11.z; z++) {
                    skeleton[x, point00.y, z] = value;
                }
            }
        } else if (direction == 3 || direction == 6) {
            Point point00 = new Point(point.x - width, point.y, point.z);
            Point point11 = new Point(point.x + width, point.y + height, point.z);
            for (int x = point00.x; x <= point11.x; x++) {
                for (int y = point00.y; y <= point11.y; y++) {
                    skeleton[x, y, point00.z] = value;
                }
            }
        }        

        return skeleton;
    }

    static int[,,] DrawHallwayWallSegment(int[,,] skeleton, Point point, int direction, int width, int height, int value) {
        if (direction == 1 || direction == 4) {
            Point point00 = new Point(point.x, point.y - 1, point.z - 1 - width);
            Point point11 = new Point(point.x, point.y + 1 + height, point.z + 1 + width);
            for (int y = point00.y; y <= point11.y; y++) {
                skeleton[point00.x, y, point00.z] = value;
                skeleton[point11.x, y, point11.z] = value;
            }
            for (int z = point00.z; z <= point11.z; z++) {
                skeleton[point00.x, point00.y, z] = value;
                skeleton[point11.x, point11.y, z] = value;
            }
        } else if (direction == 2 || direction == 5) {
            Point point00 = new Point(point.x - 1 - width, point.y, point.z - 1 - width);
            Point point11 = new Point(point.x + 1 + width, point.y, point.z + 1 + width);
            for (int x = point00.x; x <= point11.x; x++) {
                skeleton[x, point00.y, point00.z] = value;
                skeleton[x, point11.y, point11.z] = value;
            }
            for (int z = point00.z; z <= point11.z; z++) {
                skeleton[point00.x, point00.y, z] = value;
                skeleton[point11.x, point11.y, z] = value;
            }
        } else if (direction == 3 || direction == 6) {
            Point point00 = new Point(point.x - 1 - width, point.y - 1, point.z);
            Point point11 = new Point(point.x + 1 + width, point.y + 1 + height, point.z);
            for (int x = point00.x; x <= point11.x; x++) {
                skeleton[x, point00.y, point00.z] = value;
                skeleton[x, point11.y, point11.z] = value;
            }
            for (int y = point00.y; y <= point11.y; y++) {
                skeleton[point00.x, y, point00.z] = value;
                skeleton[point11.x, y, point11.z] = value;
            }
        }        

        return skeleton;
    }

    static (int[,,], int[,,]) DrawHallwaySkeleton(int[,,] wallSkeleton, int[,,] spaceSkeleton, Edge edge) {
        int lastDirection = edge.directions[0];
        for (int i = 0; i < edge.path.Count(); i++) {
            Point point = edge.path[i];
            int direction = edge.directions[i];
            wallSkeleton = DrawHallwayWallSegment(wallSkeleton, point, direction, edge.width, edge.height, 1); // currently writing a generic 1
            spaceSkeleton = DrawHallwayWallSegment(spaceSkeleton, point, direction, edge.width, edge.height, 1); // Space skeleton could be recorded as simply hyper-rectangles where there are turns and where two edges intersect

            if (direction != lastDirection) {
                Point lastPoint = edge.path[i - 1];
                if (lastDirection == 1) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x + j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(spaceSkeleton, new Point(lastPoint.x + j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (lastDirection == 2) {
                    for (int j = 1; j <= edge.height; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y + j, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y + j, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (lastDirection == 3) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z + j), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z + j), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (lastDirection == 4) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x - j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x - j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (lastDirection == 5) {
                    // Nothing happens here
                } else if (lastDirection == 6) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z - j), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z - j), lastDirection, edge.width, edge.height, 1);
                    }
                }

                if (direction == 1) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x - j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x - j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (direction == 2) {
                    // Nothing happes here
                } else if (direction == 3) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z - j), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z - j), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (direction == 4) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x + j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x + j, lastPoint.y, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (direction == 5) {
                    for (int j = 1; j <= edge.height; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y + j, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y + j, lastPoint.z), lastDirection, edge.width, edge.height, 1);
                    }
                } else if (direction == 6) {
                    for (int j = 1; j <= edge.width; j++) {
                        wallSkeleton = DrawHallwayWallSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z + j), lastDirection, edge.width, edge.height, 1);
                        spaceSkeleton = DrawHallwaySpaceSegment(wallSkeleton, new Point(lastPoint.x, lastPoint.y, lastPoint.z + j), lastDirection, edge.width, edge.height, 1);
                    }
                }
            }
        }

        return (wallSkeleton, spaceSkeleton);
    }

    public static SuperMap PlaceHallways(SuperMap superMap) {
        int[,,] hallwayWallSkeleton = new int[superMap.xSize, superMap.ySize, superMap.zSize];
        int[,,] HallwaySpaceSkeleton = new int[superMap.xSize, superMap.ySize, superMap.zSize];

        foreach (Node node in superMap.nodes) {
            foreach (Edge edge in node.edges) {
                if (edge.source.node == node) {
                    (hallwayWallSkeleton, HallwaySpaceSkeleton) = DrawHallwaySkeleton(hallwayWallSkeleton, HallwaySpaceSkeleton, edge);
                }
            }
        }

        for (int i = 0; i < superMap.xSize; i++) {
            for (int j = 0; j < superMap.ySize; j++) {
                for (int k = 0; k < superMap.zSize; k++) {
                    if (hallwayWallSkeleton[i, j, k] != 0 && superMap.skeleton[i, j, k] == 0) {
                        superMap.skeleton[i, j, k] = hallwayWallSkeleton[i, j, k];
                    }
                }
            }
        }

        for (int i = 0; i < superMap.xSize; i++) {
            for (int j = 0; j < superMap.ySize; j++) {
                for (int k = 0; k < superMap.zSize; k++) {
                    if (hallwayWallSkeleton[i, j, k] == 1) {
                        superMap.skeleton[i, j, k] = 0;
                    }
                }
            }
        }

        return superMap;
    } 
}