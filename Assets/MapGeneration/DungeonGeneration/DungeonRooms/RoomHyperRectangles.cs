using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct HyperRectangle {
    public Point point000;
    public Point point111;
    public int direction;
    public List<int> intersections;
    public List<Face> faces;

    public HyperRectangle(int direction, Point point000, Point point111, List<Face> faces) {
        this.point000 = point000;
        this.point111 = point111;

        this.direction = direction;

        this.faces = faces;

        this.intersections = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7};
    }

    public HyperRectangle(int direction, int x00, int y00, int z00, int x11, int y11, int z11, List<Face> faces) {
        this.point000 = new Point(x00, y00, z00);
        this.point111 = new Point(x11, y11, z11);

        this.direction = direction;

        this.faces = faces;

        this.intersections = new List<int>() {0, 1, 2, 3, 4, 5, 6, 7};
    }

    public HyperRectangle(HyperRectangle original, int x, int y, int z) {
        this.point000 = original.point000 + new Point(x, y, z);
        this.point111 = original.point111 + new Point(x, y, z);
        
        this.direction = original.direction;

        this.faces = original.faces;

        this.intersections = original.intersections;
    }
}

public class RoomHyperRectangles {
    static (Point, Point) GetRectanglePoints(HyperRectangle next, int l, int w, int intersection) {
        Point point000 = new Point();
        Point point111 = new Point();
        
        if (intersection == 0) {
            int extendLength = (l - w) / 2;

            if (next.direction == 0) {
                point000 = new Point(next.point000.x, next.point000.z - extendLength, next.point000.y);
                point111 = new Point(next.point000.x + w, next.point111.z + extendLength, next.point111.y);
            } else {
                point000 = new Point(next.point000.x - extendLength, next.point000.z, next.point000.y);
                point111 = new Point(next.point111.x + extendLength, next.point000.z + w, next.point111.y);
            }
        } else if (intersection == 1) {
            int extendLength = (l - w) / 2;

            if (next.direction == 0) {
                point000 = new Point(next.point111.x - w, next.point000.z - extendLength, next.point000.y);
                point111 = new Point(next.point111.x, next.point111.z + extendLength, next.point111.y);
            } else {
                point000 = new Point(next.point000.x - extendLength, next.point111.z - w, next.point000.y);
                point111 = new Point(next.point111.x + extendLength, next.point111.z, next.point111.y);
            }
        } else if (intersection == 2) {
            int extendLength = l - w;
            
            if (next.direction == 0) {
                point000 = new Point(next.point000.x, next.point000.z - extendLength, next.point000.y);
                point111 = new Point(next.point000.x + w, next.point000.z, next.point111.y);
            } else {
                point000 = new Point(next.point000.x - extendLength, next.point000.z, next.point000.y);
                point111 = new Point(next.point000.x, next.point000.z + w, next.point111.y);
            }
        } else if (intersection == 3) {
            int extendLength = l - w;
            
            if (next.direction == 0) {
                point000 = new Point(next.point000.x, next.point111.z, next.point000.y);
                point111 = new Point(next.point000.x + w, next.point111.z + extendLength, next.point111.y);
            } else {
                point000 = new Point(next.point111.x, next.point000.z, next.point000.y);
                point111 = new Point(next.point111.x + extendLength, next.point000.z + w, next.point111.y);
            }
        } else if (intersection == 4) {
            int extendLength = l - w;
            
            if (next.direction == 0) {
                int mid = (next.point000.x + next.point111.x) / 2;

                point000 = new Point(mid - w / 2, next.point000.z - extendLength, next.point000.y);
                point111 = new Point(mid + (w + 1) / 2, next.point000.z, next.point111.y);
            } else {
                int mid = (next.point000.z + next.point111.z) / 2;

                point000 = new Point(next.point000.x - extendLength, mid - w / 2, next.point000.y);
                point111 = new Point(next.point000.x, mid + (w + 1) / 2, next.point111.y);
            }
        } else if (intersection == 5) {
            int extendLength = l - w;
            
            if (next.direction == 0) {
                int mid = (next.point000.x + next.point111.x) / 2;

                point000 = new Point(mid - w / 2, next.point111.z, next.point000.y);
                point111 = new Point(mid + (w + 1) / 2, next.point111.z + extendLength, next.point111.y);
            } else {
                int mid = (next.point000.z + next.point111.z) / 2;

                point000 = new Point(next.point111.x, mid - w / 2, next.point000.y);
                point111 = new Point(next.point111.x + extendLength, mid + (w + 1) / 2, next.point111.y);
            }
        } else if (intersection == 6) {
            int extendLength = l - w;
            
            if (next.direction == 0) {
                point000 = new Point(next.point111.x - w, next.point000.z - extendLength, next.point000.y);
                point111 = new Point(next.point111.x, next.point000.z, next.point111.y);
            } else {
                point000 = new Point(next.point000.x - extendLength, next.point111.z - w, next.point000.y);
                point111 = new Point(next.point000.x, next.point111.z, next.point111.y);
            }
        } else if (intersection == 7) {
            int extendLength = l - w;
            
            if (next.direction == 0) {
                point000 = new Point(next.point111.x - w, next.point111.z, next.point000.y);
                point111 = new Point(next.point111.x, next.point111.z + extendLength, next.point111.y);
            } else {
                point000 = new Point(next.point111.x, next.point111.z - w, next.point000.y);
                point111 = new Point(next.point111.x + extendLength, next.point111.z, next.point111.y);
            }
        }

        return (point000, point111);
    }

    static (RoomSkeleton, List<HyperRectangle>, Point, Point, bool) TransposeMap(RoomSkeleton roomSkeleton, Point point000, Point point111, List<HyperRectangle> rects) {
        int xTrans = 0;
        int yTrans = 0;
        int zTrans = 0;
        bool transpose = false;
        bool noRoom = false;
        if (point000.x < 0) {
            transpose = true;
            xTrans = -point000.x;
        }
        if (point000.y < 0) {
            transpose = true;
            yTrans = -point000.y;
        }
        if (point000.z < 0) {
            transpose = true;
            zTrans = -point000.z;
        }
        if (point111.x >= roomSkeleton.skeleton.GetLength(0)) {
            transpose = true;
            if (xTrans != 0) {
                noRoom = true;
            } else {
                xTrans = point111.x - roomSkeleton.skeleton.GetLength(0) - 1;
            }
        }
        if (point111.y >= roomSkeleton.skeleton.GetLength(1)) {
            transpose = true;
            if (yTrans != 0) {
                noRoom = true;
            } else {
                yTrans = point111.y - roomSkeleton.skeleton.GetLength(1) - 1;
            }
        }
        if (point111.z >= roomSkeleton.skeleton.GetLength(2)) {
            transpose = true;
            if (zTrans != 0) {
                noRoom = true;
            } else {
                zTrans = point111.z - roomSkeleton.skeleton.GetLength(2) - 1;
            }
        }

        if (noRoom) {
            return (roomSkeleton, rects, point000, point111, false);
        }

        bool transposeSuccess = true;

        if (transpose) {
            (roomSkeleton.skeleton, transposeSuccess) = Utils.transpose(roomSkeleton.skeleton, xTrans, yTrans, zTrans);
            if (transposeSuccess) {
                List<HyperRectangle> newRects = new List<HyperRectangle>();
                foreach (HyperRectangle rect in rects) {
                    newRects.Add(new HyperRectangle(rect, xTrans, zTrans, yTrans));
                }
                rects = newRects;
            } else {
                return (roomSkeleton, rects, point000, point111, false);
            }
        }

        point000 = point000 + new Point(xTrans, yTrans, zTrans);
        point111 = point111 + new Point(xTrans, yTrans, zTrans);

        return (roomSkeleton, rects, point000, point111, true);
    }

    static bool CheckClear(RoomSkeleton roomSkeleton, HyperRectangle next, Point point000, Point point111) {
        for (int x = point000.x; x <= point111.x; x++) {
            for (int y = point000.y; y <= point111.y; y++) {
                for (int z = point000.z; z <= point111.z; z++) {
                    if (!(x >= next.point000.x && x <= next.point111.x && y >= next.point000.y && y <= next.point111.y && z >= next.point000.z && z <= next.point111.z) && roomSkeleton.skeleton[x, y, z] != 0) {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    static RoomSkeleton DrawHyperRectangle(RoomSkeleton roomSkeleton, HyperRectangle rect, int spaceValue, int wallValue, int ceilingValue, int floorValue) {
        for (int x = rect.point000.x; x <= rect.point111.x; x++) {
            for (int y = rect.point000.y; y <= rect.point111.y; y++) {
                for (int z = rect.point000.z; z <= rect.point111.z; z++) {
                    if (x == rect.point000.x || x == rect.point111.x || z == rect.point000.z || z == rect.point111.z) {
                        roomSkeleton.skeleton[x, y, z] = wallValue;
                    } else if (y == rect.point000.y) {
                        roomSkeleton.skeleton[x, y, z] = floorValue;
                    } else if (y == rect.point111.y) {
                        roomSkeleton.skeleton[x, y, z] = ceilingValue;
                    } else {
                        roomSkeleton.skeleton[x, y, z] = spaceValue;
                    }
                }
            }
        }

        return roomSkeleton;
    }

    static RoomSkeleton DrawOverlappingHyperRectangles(RoomSkeleton roomSkeleton, HyperRectangle next, HyperRectangle newRect, int spaceValue, int wallValue, int ceilingValue, int floorValue) {
        for (int x = newRect.point000.x; x <= newRect.point111.x; x++) {
            for (int y = newRect.point000.y; y <= newRect.point111.y; y++) {
                for (int z = newRect.point000.z; z <= newRect.point111.z; z++) {
                    if (x == newRect.point000.x || x == newRect.point111.x || z == newRect.point000.z || z == newRect.point111.z) {
                        if (!(x >= next.point000.x && x <= next.point111.x && y >= next.point000.y && y <= next.point111.y && z >= next.point000.z && z <= next.point111.z)) {
                            roomSkeleton.skeleton[x, y, z] = wallValue;
                        }
                    } else if (y == newRect.point000.y) {
                        roomSkeleton.skeleton[x, y, z] = floorValue;
                    } else if (y == newRect.point111.y) {
                        roomSkeleton.skeleton[x, y, z] = ceilingValue;
                    } else {
                        roomSkeleton.skeleton[x, y, z] = spaceValue;
                    }
                }
            }
        }

        return roomSkeleton;
    }

    static (RoomSkeleton, HyperRectangle, List<Face>) SetupOverlappingFaces(RoomSkeleton roomSkeleton, HyperRectangle next, Point point000, Point point111) {
        List<Face> newFaces = new List<Face>();
        
        bool face1 = false;
        bool face3 = false;
        bool face4 = false;
        bool face6 = false;

        // instead go through roomSkeleton faces
        for (int i = 0; i < next.faces.Count(); i++) {
            Face oldFace = roomSkeleton.faces[i];
            if (oldFace.direction == 1 && ((oldFace.corner00.z < point111.z && oldFace.corner00.z > point000.z) || (oldFace.corner11.z < point111.z && oldFace.corner11.z > point000.z))) {
                face1 = true;

                if (oldFace.corner00.x == point111.x) {
                    if (oldFace.corner00.z <= point000.z && oldFace.corner11.z >= point111.z) {
                        // no new face
                        // don't break up old
                    } else {
                        // remove old face
                        // add new face
                        next.faces.Remove(oldFace);
                        roomSkeleton.faces.Remove(oldFace);
                        newFaces.Add(new Face(new Point(point111.x, point000.y, point000.z), new Point(point111.x, point111.y, point111.z), 1));
                    }
                } else if (oldFace.corner00.x < point111.x) {
                    // Remove old face
                    // add new face
                    next.faces.Remove(oldFace);
                    roomSkeleton.faces.Remove(oldFace);
                    newFaces.Add(new Face(new Point(point111.x, point000.y, point000.z), new Point(point111.x, point111.y, point111.z), 1));

                    if (oldFace.corner00.z < point000.z) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(oldFace.corner00.x, point000.y, oldFace.corner00.z), new Point(oldFace.corner00.x, point111.y, point000.z), 1);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }

                    if (oldFace.corner11.z > point111.z) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(oldFace.corner00.x, point000.y, point111.z), new Point(oldFace.corner00.x, point111.y, oldFace.corner11.z), 1);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }
                } else {
                    if (point000.z < oldFace.corner00.z) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(point111.x, point000.y, point000.z), new Point(point111.x, point111.y, oldFace.corner00.z), 1));
                    }

                    if (point111.z > oldFace.corner11.z) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(point111.x, point000.y, oldFace.corner11.z), new Point(point111.x, point111.y, point111.z), 1));
                    }
                }
            } else if (oldFace.direction == 3 && ((oldFace.corner00.x < point111.x && oldFace.corner00.x > point000.x) || (oldFace.corner11.x < point111.x && oldFace.corner11.x > point000.x))) {
                face3 = true;

                if (oldFace.corner00.z == point111.z) {
                    if (oldFace.corner00.x <= point000.x && oldFace.corner11.x >= point111.x) {
                        // no new face
                        // don't break up old
                    } else {
                        // remove old face
                        // add new face
                        next.faces.Remove(oldFace);
                        roomSkeleton.faces.Remove(oldFace);
                        newFaces.Add(new Face(new Point(point000.x, point000.y, point111.z), new Point(point111.x, point111.y, point111.z), 3));
                    }
                } else if (oldFace.corner00.z < point111.z) {
                    // Remove old face
                    // add new face
                    next.faces.Remove(oldFace);
                    roomSkeleton.faces.Remove(oldFace);
                    newFaces.Add(new Face(new Point(point000.x, point000.y, point111.z), new Point(point111.x, point111.y, point111.z), 3));

                    if (oldFace.corner00.x < point000.x) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(oldFace.corner00.x, point000.y, oldFace.corner00.z), new Point(point000.x, point111.y, oldFace.corner00.z), 3);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }

                    if (oldFace.corner11.x > point111.x) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(point111.x, point000.y, oldFace.corner00.z), new Point(oldFace.corner11.x, point111.y, oldFace.corner00.z), 3);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }
                } else {
                    if (point000.x < oldFace.corner00.x) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(point000.x, point000.y, point111.z), new Point(oldFace.corner00.x, point111.y, point111.z), 3));
                    }

                    if (point111.x > oldFace.corner11.x) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(oldFace.corner11.x, point000.y, point111.z), new Point(point111.x, point111.y, point111.z), 3));
                    }
                }
            } else if (oldFace.direction == 4 && ((oldFace.corner00.z < point111.z && oldFace.corner00.z > point000.z) || (oldFace.corner11.z < point111.z && oldFace.corner11.z > point000.z))) {
                face4 = true;

                if (oldFace.corner00.x == point000.x) {
                    if (oldFace.corner00.z <= point000.z && oldFace.corner11.z >= point111.z) {
                        // no new face
                        // don't break up old
                    } else {
                        // remove old face
                        // add new face
                        next.faces.Remove(oldFace);
                        roomSkeleton.faces.Remove(oldFace);
                        newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point000.x, point111.y, point111.z), 4));
                    }
                } else if (oldFace.corner00.x > point000.x) {
                    // Remove old face
                    // add new face
                    next.faces.Remove(oldFace);
                    roomSkeleton.faces.Remove(oldFace);
                    newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point000.x, point111.y, point111.z), 4));

                    if (oldFace.corner00.z < point000.z) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(oldFace.corner00.x, point000.y, oldFace.corner00.z), new Point(oldFace.corner00.x, point111.y, point000.z), 4);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }

                    if (oldFace.corner11.z > point111.z) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(oldFace.corner00.x, point000.y, point111.z), new Point(oldFace.corner00.x, point111.y, oldFace.corner11.z), 4);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }
                } else {
                    if (point000.z < oldFace.corner00.z) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point000.x, point111.y, oldFace.corner00.z), 4));
                    }

                    if (point111.z > oldFace.corner11.z) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(point000.x, point000.y, oldFace.corner11.z), new Point(point000.x, point111.y, point111.z), 4));
                    }
                }
            } else if (oldFace.direction == 6 && ((oldFace.corner00.x < point111.x && oldFace.corner00.x > point000.x) || (oldFace.corner11.x < point111.x && oldFace.corner11.x > point000.x))) {
                face4 = true;

                if (oldFace.corner00.z == point000.z) {
                    if (oldFace.corner00.x <= point000.x && oldFace.corner11.x >= point111.x) {
                        // no new face
                        // don't break up old
                    } else {
                        // remove old face
                        // add new face
                        next.faces.Remove(oldFace);
                        roomSkeleton.faces.Remove(oldFace);
                        newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point111.x, point111.y, point000.z), 6));
                    }
                } else if (oldFace.corner00.z > point000.z) {
                    // Remove old face
                    // add new face
                    next.faces.Remove(oldFace);
                    roomSkeleton.faces.Remove(oldFace);
                    newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point111.x, point111.y, point000.z), 6));

                    if (oldFace.corner00.x < point000.x) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(oldFace.corner00.x, point000.y, oldFace.corner00.z), new Point(point000.x, point111.y, oldFace.corner00.z), 6);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }

                    if (oldFace.corner11.x > point111.x) {
                        // Add the extended bit of old
                        Face extension = new Face(new Point(point111.x, point000.y, oldFace.corner00.z), new Point(oldFace.corner11.x, point111.y, oldFace.corner00.z), 6);
                        next.faces.Add(extension);
                        roomSkeleton.faces.Add(extension);
                    }
                } else {
                    if (point000.x < oldFace.corner00.x) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(oldFace.corner00.x, point111.y, point000.z), 6));
                    }

                    if (point111.x > oldFace.corner11.x) {
                        // Add the extended bit
                        newFaces.Add(new Face(new Point(oldFace.corner11.x, point000.y, point000.z), new Point(point111.x, point111.y, point000.z), 6));
                    }
                }
            }
        }

        if (!face1) {
            newFaces.Add(new Face(new Point(point111.x, point000.y, point000.z), new Point(point111.x, point111.y, point111.z), 1));
        }
        if (!face3) {
            newFaces.Add(new Face(new Point(point000.x, point000.y, point111.z), new Point(point111.x, point111.y, point111.z), 3));
        }
        if (!face4) {
            newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point000.x, point111.y, point111.z), 4));
        }
        if (!face6) {
            newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point111.x, point111.y, point000.z), 6));
        }
        newFaces.Add(new Face(new Point(point000.x, point111.y, point000.z), new Point(point111.x, point111.y, point111.z), 2));
        newFaces.Add(new Face(new Point(point000.x, point000.y, point000.z), new Point(point111.x, point000.y, point111.z), 5));

        return (roomSkeleton, next, newFaces);
    }

    public static RoomSkeleton SetupHyperRectangles(
        RoomSkeleton roomSkeleton, System.Random random, 
        int num, int numRange, int xMax, int yMax, int zMax, 
        int length, int lengthRange, int width, int widthRange, int height, int heightRange,
        int spaceValue, int wallValue, int floorValue, int ceilingValue) {

        int rectNum = num + random.Next(-numRange, numRange);
        List<HyperRectangle> rects = new List<HyperRectangle>();

        int h = Math.Min(height + random.Next(-heightRange, heightRange), yMax);
        int l = length + random.Next(-lengthRange, lengthRange);
        int w = width + random.Next(-widthRange, widthRange);

        for (int i = 0; i < rectNum; i++) {
            if (rects.Count() == 0) {
                Point point000 = new Point(0, 0, 0);
                Point point111;

                int direction = random.Next(0,1);

                if (direction == 0) {
                    l = Math.Min(l, xMax);
                    w = Math.Min(w, zMax);
                    point111 = new Point(l, h, w);
                } else {
                    l = Math.Min(l, zMax);
                    w = Math.Min(w, xMax);
                    point111 = new Point(w, h, l);
                }

                List<Face> newFaces = new List<Face>();

                newFaces.Add(new Face(new Point(point111.x, 0, 0), new Point(point111.x, point111.y, point111.z), 1));
                newFaces.Add(new Face(new Point(0, point111.y, 0), new Point(point111.x, point111.y, point111.z), 2));
                newFaces.Add(new Face(new Point(0, 0, point111.z), new Point(point111.x, point111.y, point111.z), 3));
                newFaces.Add(new Face(new Point(0, 0, 0), new Point(0, point111.y, point111.z), 4));
                newFaces.Add(new Face(new Point(0, 0, 0), new Point(point111.x, 0, point111.z), 5));
                newFaces.Add(new Face(new Point(0, 0, 0), new Point(point111.x, point111.y, 0), 6));

                roomSkeleton.faces = roomSkeleton.faces.Concat(newFaces).ToList();

                HyperRectangle originRect = new HyperRectangle(direction, point000, point111, newFaces);

                roomSkeleton = DrawHyperRectangle(roomSkeleton, originRect, spaceValue, wallValue, ceilingValue, floorValue);                
                rects.Add(originRect);
            } else {
                bool findingRect = true;

                while (findingRect && rects.Count() > 0) {
                    HyperRectangle next = rects[random.Next(rects.Count())];

                    bool findingIntersection = true;

                    while (findingIntersection && next.intersections.Count() > 0) {
                        int intersection = next.intersections[random.Next(next.intersections.Count())];
                        next.intersections.Remove(intersection);

                        if (next.intersections.Count() == 0) {
                            rects.Remove(next);
                        }

                        int extendLength = (l - w) / 2;

                        Point point000;
                        Point point111;

                        (point000, point111)  = GetRectanglePoints(next, l, w, intersection);

                        bool TransposeSuccess;
                        (roomSkeleton, rects, point000, point111, TransposeSuccess) = TransposeMap(roomSkeleton, point000, point111, rects);

                        if (TransposeSuccess && CheckClear(roomSkeleton, next, point000, point111)) {
                            int newDirection = 0;
                            if (next.direction == 0) {
                                newDirection = 1;
                            }

                            List<Face> newFaces;
                            (roomSkeleton, next, newFaces) = SetupOverlappingFaces(roomSkeleton, next, point000, point111);
                            HyperRectangle newRect = new HyperRectangle(newDirection, point000, point111, newFaces);
                            roomSkeleton = DrawOverlappingHyperRectangles(roomSkeleton, next, newRect, spaceValue, wallValue, ceilingValue, floorValue);
                        }
                    }
                }
            }
        }

        return roomSkeleton;
    }

}