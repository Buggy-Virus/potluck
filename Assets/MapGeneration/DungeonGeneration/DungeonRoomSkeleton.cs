using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate RoomSkeleton RoomSkeletonMethod (System.Random random, int xMax, int yMax, int zMax, int xMin, int yMin, int zMin);

public class DungeonRoomSkeleton { 

    // Fills a hyperRectangle with a simple room to the bounds of the hyperRectangle
    public static RoomSkeleton SimpleRoomSkeleton(System.Random random, int xMax, int yMax, int zMax, int xMin, int yMin, int zMin) {
        RoomSkeleton roomSkeleton = new RoomSkeleton();
        int xSize = random.Next(xMin, xMax);
        int ySize = random.Next(yMin, yMax);
        int zSize = random.Next(zMin, zMax);
        roomSkeleton.skeleton = new int[xSize,ySize,zSize];

        roomSkeleton.xLength = xSize;
        roomSkeleton.yLength = ySize;
        roomSkeleton.zLength = zSize;
        int xEnd = xSize - 1;
        int yEnd = ySize - 1;
        int zEnd = zSize - 1;
        roomSkeleton.midPoint = new Point(xSize / 2, ySize / 2, zSize / 2);

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                roomSkeleton.skeleton[x, y, 0] = 1;
                roomSkeleton.skeleton[x, y, zEnd] = 1;
            }
        }

        for (int x = 0; x < xSize; x++) {
            for (int z = 0; z < zSize; z++) {
                roomSkeleton.skeleton[x, 0, z] = 1;
                roomSkeleton.skeleton[x, yEnd, z] = 1;
            }
        }

        for (int y = 0; y < ySize; y++) {
            for (int z = 0; z < zSize; z++) {
                roomSkeleton.skeleton[0, y, z] = 1;
                roomSkeleton.skeleton[xEnd, y, z] = 1;
            }
        }

        Node node = new Node();
        node.midPoint = roomSkeleton.midPoint;
        node.edges = new List<Edge>();
        roomSkeleton.nodes = new List<Node>() {node};

        Portal xPortalClose = new Portal();
        xPortalClose.point = new Point(0, 1, zSize / 2);
        roomSkeleton.skeleton[0, 1, zSize / 2] = 1000;
        roomSkeleton.skeleton[0, 2, zSize / 2] = 1000;
        xPortalClose.type = 0;
        xPortalClose.width = 0;
        xPortalClose.height = 0;
        xPortalClose.direction = 4;
        xPortalClose.node = node;
        xPortalClose.edgeCount = 0;
        xPortalClose.setup = false;
        xPortalClose.edges = new List<Edge>();

        Portal zPortalClose = new Portal();
        zPortalClose.point = new Point(xSize / 2, 1, 0);
        roomSkeleton.skeleton[xEnd / 2, 1, 0] = 1000;
        roomSkeleton.skeleton[xEnd / 2, 2, 0] = 1000;
        zPortalClose.type = 0;
        zPortalClose.width = 0;
        zPortalClose.height = 0;
        zPortalClose.direction = 4;
        zPortalClose.node = node;
        xPortalClose.edgeCount = 0;
        zPortalClose.setup = false;
        zPortalClose.edges = new List<Edge>();

        Portal xPortalFar = new Portal();
        xPortalFar.point = new Point(xEnd, 1, zSize / 2);
        roomSkeleton.skeleton[xEnd, 1, zSize / 2] = 1000;
        roomSkeleton.skeleton[xEnd, 2, zSize / 2] = 1000;
        xPortalFar.type = 0;
        xPortalFar.width = 0;
        xPortalFar.height = 0;
        xPortalFar.direction = 4;
        xPortalFar.node = node;
        xPortalFar.edgeCount = 0;
        xPortalFar.setup = false;
        xPortalFar.edges = new List<Edge>();

        Portal zPortalFar = new Portal();
        zPortalFar.point = new Point(xSize / 2, 1, zEnd);
        roomSkeleton.skeleton[xSize / 2, 1, zEnd] = 1000;
        roomSkeleton.skeleton[xSize / 2, 2, zEnd] = 1000;
        zPortalFar.type = 0;
        zPortalFar.width = 0;
        zPortalFar.height = 0;
        zPortalFar.direction = 4;
        zPortalFar.node = node;
        zPortalFar.edgeCount = 0;
        zPortalFar.setup = false;
        zPortalFar.edges = new List<Edge>();

        roomSkeleton.portals = new List<Portal>() {xPortalClose, xPortalFar, zPortalClose, zPortalFar};
        node.portals = roomSkeleton.portals;

        Face xFaceClose = new Face();
        xFaceClose.direction = 4;
        xFaceClose.corner00 = new Point(0, 0, 0);
        xFaceClose.corner10 = new Point(0, yEnd, 0);
        xFaceClose.corner01 = new Point(0, 0, zEnd);
        xFaceClose.corner11 = new Point(0, yEnd, zEnd);

        Face xFaceFar = new Face();
        xFaceFar.direction = 1;
        xFaceFar.corner00 = new Point(xEnd, 0, 0);
        xFaceFar.corner10 = new Point(xEnd, yEnd, 0);
        xFaceFar.corner01 = new Point(xEnd, 0, zEnd);
        xFaceFar.corner11 = new Point(xEnd, yEnd, zEnd);

        Face yFaceClose = new Face();
        yFaceClose.direction = 5;
        yFaceClose.corner00 = new Point(0, 0, 0);
        yFaceClose.corner10 = new Point(xEnd, 0, 0);
        yFaceClose.corner01 = new Point(0, 0, zEnd);
        yFaceClose.corner11 = new Point(xEnd, 0, zEnd);

        Face yFaceFar = new Face();
        yFaceFar.direction = 2;
        yFaceFar.corner00 = new Point(0, yEnd, 0);
        yFaceFar.corner10 = new Point(xEnd, yEnd, 0);
        yFaceFar.corner01 = new Point(0, yEnd, zEnd);
        yFaceFar.corner11 = new Point(xEnd, yEnd, zEnd);

        Face zFaceClose = new Face();
        zFaceClose.direction = 6;
        zFaceClose.corner00 = new Point(0, 0, 0);
        zFaceClose.corner10 = new Point(xEnd, 0, 0);
        zFaceClose.corner01 = new Point(0, yEnd, 0);
        zFaceClose.corner11 = new Point(xEnd, yEnd, 0);

        Face zFaceFar = new Face();
        zFaceFar.direction = 3;
        zFaceFar.corner00 = new Point(0, 0, zEnd);
        zFaceFar.corner10 = new Point(xEnd, 0, zEnd);
        zFaceFar.corner01 = new Point(0, yEnd, zEnd);
        zFaceFar.corner11 = new Point(xEnd, yEnd, zEnd);

        roomSkeleton.faces = new List<Face>() {xFaceClose, xFaceFar, yFaceClose, yFaceFar, zFaceClose, zFaceFar};
        roomSkeleton.openFaces = roomSkeleton.faces;

        return roomSkeleton;
    }

    public static RoomSkeleton SimpleOriginSkeleton(System.Random random, int xMax, int yMax, int zMax, int xMin, int yMin, int zMin) {
        RoomSkeleton roomSkeleton = SimpleRoomSkeleton(random, xMax, yMax, zMax, xMin, yMin, zMin);
        roomSkeleton.nodes[0].origin = true;

        return roomSkeleton;
    }

    public static RoomSkeleton SimpleGoalSkeleton(System.Random random, int xMax, int yMax, int zMax, int xMin, int yMin, int zMin) {
        RoomSkeleton roomSkeleton = SimpleRoomSkeleton(random, xMax, yMax, zMax, xMin, yMin, zMin);
        roomSkeleton.nodes[0].goal = true;

        return roomSkeleton;
    }
    
    public static RoomSkeleton GenerateRoomSkeleton(System.Random random, RoomPrefab roomPrefab, (Point, Point) bounds) {
        int xMax = Math.Min(bounds.Item2.x - bounds.Item1.x, roomPrefab.xMax);
        int yMax = Math.Min(bounds.Item2.y - bounds.Item1.y, roomPrefab.yMax);
        int zMax = Math.Min(bounds.Item2.z - bounds.Item1.z, roomPrefab.zMax);

        return roomPrefab.roomSkeletonMethod(random, xMax, yMax, zMax, roomPrefab.xMin, roomPrefab.yMin, roomPrefab.zMin);
    }
}