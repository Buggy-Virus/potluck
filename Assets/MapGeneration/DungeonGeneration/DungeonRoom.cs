using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom {

    // Takes a superMap and based on it's open faces chooses the next open face at random
    // to build the next room off of
    public static Point PickNextLocation(SuperMap superMap, System.Random random) {
        // Randomly pick an open face from the list of open faces
        // The next room will be build "off" of this face
        int openFaceIndex = random.Next(superMap.openFaces.Count());
        Face face = superMap.openFaces[openFaceIndex];
        // Remove the open face we randomly selected now that it 
        // Will have a room off it if possible
        // If it's not possible to have a room off it, we want it
        // evicted all the same
        superMap.openFaces.RemoveAt(openFaceIndex);

        Point nextLocation = new Point();

        // Depending on direction of the face, return x and z midpoints
        // If not facing up or down, return the y at the base of the face
        // Otherwise return the midpoint of y
        if (face.direction == 1 || face.direction == 4) {
            nextLocation.x = face.corner00.x;
            nextLocation.y = face.corner00.y;
            nextLocation.z = (face.corner00.z + face.corner11.z) / 2;
        } else if (face.direction == 2 || face.direction == 5) {
            nextLocation.y = face.corner00.y;
            nextLocation.x = (face.corner00.x + face.corner11.x) / 2;
            nextLocation.z = (face.corner00.z + face.corner11.z) / 2;
        } else {
            nextLocation.z = face.corner00.z;
            nextLocation.y = face.corner00.y;
            nextLocation.x = (face.corner00.x + face.corner11.x) / 2;
        }

        Debug.Log("Next Location Point x = " + nextLocation.x + ", y = " + nextLocation.y + ", z = " + nextLocation.z);
        return nextLocation;
    }

    // Take a room type and returns it's prefab
    public static RoomPrefab GetRoomPrefab(int type) {
        // TODO
        return DungeonRoomPrefabs.typeToRoomPrefab[type];
    }

    // Measures the free hyperRectangle off of a point based on a couple dimensions orders
    // returns the two points bounding the found hyperrectangle with the most volume 
    public static (Point, Point) MeasureFreeSpace(SuperMap superMap, Point point) {
        // Order of which the dimensions are checked
        // 1 xyz
        // 2 xzy
        // 3 zxy
        // 4 yxz
        
        // Check all faces, depending on which direction they are facing, bound a dimension
        // based on the face if it happens to overlap with the range we are checking
        // We then check for a different dimension until all three dimensions ordered
        // First Dimension
        int xMax12 = superMap.xSize;
        int xMin12 = 0;
        int zMax3 = superMap.zSize;
        int zMin3 = 0;
        int yMax4 = superMap.ySize;
        int yMin4 = 0;
        foreach (Room room in superMap.rooms) {
            foreach (Face face in room.faces) {
                if (face.direction == 1 && point.x >= face.corner00.x && 
                    point.y >= face.corner00.y && point.y <= face.corner11.y && 
                    point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.x > xMin12) {
                        xMin12 = face.corner00.x;
                    }
                } else if (face.direction == 4 && point.x <= face.corner00.x &&
                           point.y >= face.corner00.y && point.y <= face.corner11.y && 
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.x < xMax12) {
                        xMax12 = face.corner00.x;
                    } 
                } else if (face.direction == 2 && point.y >= face.corner00.y && 
                           point.x >= face.corner00.x && point.x <= face.corner11.x && 
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.y > yMin4) {
                        yMin4 = face.corner00.y;
                    }
                } else if (face.direction == 5 && point.y <= face.corner00.y &&
                           point.x >= face.corner00.x && point.x <= face.corner11.x &&
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {

                    if (face.corner00.y < yMax4) {
                        yMax4 = face.corner00.y;
                    }
                } else if (face.direction == 3 && point.z >= face.corner00.z && 
                           point.x >= face.corner00.x && point.x <= face.corner11.x && 
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {
                    
                    if (face.corner00.z > zMin3) {
                        zMin3 = face.corner00.z;
                    }
                } else if (face.direction == 6 && point.z <= face.corner00.z &&
                           point.x >= face.corner00.x && point.x <= face.corner11.x &&
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {

                    if (face.corner00.z < zMax3) {
                        zMax3 = face.corner00.z;
                    }
                }
            }
        }

        // Second Dimension
        int yMax1 = superMap.ySize;
        int yMin1 = 0;
        int zMax2 = superMap.zSize;
        int zMin2 = 0;
        int xMax3 = superMap.xSize;
        int xMin3 = 0;
        int xMax4 = superMap.xSize;
        int xMin4 = 0;
        foreach (Room room in superMap.rooms) {
            foreach (Face face in room.faces) {
                if (face.direction == 1 && point.x >= face.corner00.x) {
                    if (point.y >= face.corner00.y && point.y <= face.corner11.y && 
                        zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {

                            if (face.corner00.x < xMin3) {
                                xMin3 = face.corner00.x;
                            } 
                    } else if (yMax4 >= face.corner00.y && yMin4 <= face.corner11.y && 
                               point.z >= face.corner00.z && point.z <= face.corner11.z) {

                            if (face.corner00.x < xMin4) {
                                xMin4 = face.corner00.x;
                            } 
                    }
                } else if (face.direction == 4 && point.x <= face.corner00.x) {
                    if (point.y >= face.corner00.y && point.y <= face.corner11.y && 
                        zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {

                            if (face.corner00.x < xMax3) {
                                xMax3 = face.corner00.x;
                            } 
                    } else if (yMax4 >= face.corner00.y && yMin4 <= face.corner11.y && 
                               point.z >= face.corner00.z && point.z <= face.corner11.z) {

                            if (face.corner00.x < xMax4) {
                                xMax4 = face.corner00.x;
                            } 
                    }
                } else if (face.direction == 2 && point.y >= face.corner00.y && 
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {
                    
                    if (face.corner00.y > yMin1) {
                        yMin1 = face.corner00.y;
                    }
                } else if (face.direction == 5 && point.y <= face.corner00.y &&
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                           point.z >= face.corner00.z && point.z <= face.corner11.z) {

                    if (face.corner00.y < yMax1) {
                        yMax1 = face.corner00.y;
                    }
                } else if (face.direction == 3 && point.z >= face.corner00.z && 
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {
                    
                    if (face.corner00.z > zMin2) {
                        zMin2 = face.corner00.z;
                    }
                } else if (face.direction == 6 && point.z <= face.corner00.z &&
                           xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                           point.y >= face.corner00.y && point.y <= face.corner11.y) {

                    if (face.corner00.z < zMax2) {
                        zMax2 = face.corner00.z;
                    }
                }
            }
        }

        // Third dimension
        int zMax1 = superMap.zSize;
        int zMin1 = 0;
        int yMax2 = superMap.ySize;
        int yMin2 = 0;
        int yMax3 = superMap.ySize;
        int yMin3 = 0;
        int zMax4 = superMap.zSize;
        int zMin4 = 0;
        foreach (Room room in superMap.rooms) {
            foreach (Face face in room.faces) {
                if (face.direction == 2 && point.y >= face.corner00.y) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                        zMax2 >= face.corner00.z && zMin2 <= face.corner11.z) {

                        if (face.corner00.y > yMin2) {
                            yMin2 = face.corner00.y;
                        }
                    } else if (xMax3 >= face.corner00.x && xMin3 <= face.corner11.x && 
                               zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {

                        if (face.corner00.y > yMin3) {
                            yMin3 = face.corner00.y;
                        }
                    }
                } else if (face.direction == 5 && point.y <= face.corner00.y) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x && 
                        zMax2 >= face.corner00.z && zMin2 <= face.corner11.z) {
                        
                        if (face.corner00.y < yMax2) {
                            yMax2 = face.corner00.y;
                        }
                    } else if (xMax3 >= face.corner00.x && xMin3 <= face.corner11.x && 
                               zMax3 >= face.corner00.z && zMin3 <= face.corner11.z) {
                        
                        if (face.corner00.y < yMax3) {
                            yMax3 = face.corner00.y;
                        }
                    }
                } else if (face.direction == 3 && point.z >= face.corner00.z) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                        yMax1 >= face.corner00.y && yMin1 <= face.corner11.y) {

                        if (face.corner00.z > zMin1) {
                            zMin1 = face.corner00.z;
                        }
                    } else if (xMax4 >= face.corner00.x && xMin4 <= face.corner11.x &&
                               yMax4 >= face.corner00.y && yMin4 <= face.corner11.y) {

                        if (face.corner00.z > zMin4) {
                            zMin4 = face.corner00.z;
                        }
                    }
                } else if (face.direction == 6 && point.z <= face.corner00.z) {
                    if (xMax12 >= face.corner00.x && xMin12 <= face.corner11.x &&
                        yMax1 >= face.corner00.y && yMin1 <= face.corner11.y) {

                        if (face.corner00.z < zMax1) {
                            zMax1 = face.corner00.z;
                        }
                    } else if (xMax4 >= face.corner00.x && xMin4 <= face.corner11.x &&
                               yMax4 >= face.corner00.y && yMin4 <= face.corner11.y) {

                        if (face.corner00.z < zMax4) {
                            zMax4 = face.corner00.z;
                        }
                    }
                }
            }
        }

        // Find which of the spaces we measured has the largest volume
        // return the bounds of the hyper rectangle with the largest volume
        int volume1 = (xMax12 - xMin12) * (yMax1 - yMin1) * (zMax1 - zMin1);
        int volume2 = (xMax12 - xMin12) * (yMax2 - yMin2) * (zMax2 - zMin2);
        int volume3 = (xMax3 - xMin3) * (yMax3 - yMin3) * (zMax3 - zMin3);
        int volume4 = (xMax4 - xMin4) * (yMax4 - yMin4) * (zMax4 - zMin4);

        if (volume1 >= volume2 && volume1 >= volume3 && volume1 >= volume4) {
            return (new Point(xMin12, yMin1, zMin1), new Point(xMax12, yMax1, zMax1));
        } else if (volume2 >= volume3 && volume2 >= volume4) {
            return (new Point(xMin12, yMin2, zMin2), new Point(xMax12, yMax2, zMax2));
        } else if (volume3 >= volume4) {
            return (new Point(xMin3, yMin3, zMin3), new Point(xMax3, yMax3, zMax3));
        } else {
            return (new Point(xMin4, yMin4, zMin4), new Point(xMax4, yMax4, zMax4));
        }
    }

    // Calculates a required rooms weight probability based on how close it
    // is to its prefer and disprefer point
    static double CalculateRequiredRoomWeight(RequiredRoom requiredRoom, Point nextRoomLocation) {
        double weight = requiredRoom.weight;

        if (requiredRoom.hasPreference) {
            weight += requiredRoom.preference / Utils.Distance(nextRoomLocation, requiredRoom.preferPoint);
        }

        if (requiredRoom.hasDispreference) {
            weight -= requiredRoom.dispreference / Utils.Distance(nextRoomLocation, requiredRoom.dispreferPoint);
        }

        return weight;
    }

    // Picks a next room based on the point the next room will be at, the total size available
    // Uses the superMap's required rooms and room weights to pick
    // returns -1 if it fails to get an eligible room
    public static int PickNextRoom(SuperMap superMap, System.Random random, (Point, Point) nextRoomBounds, Point nextRoomLocation) {
        // find the max dimenstions
        int xMax = nextRoomBounds.Item2.x - nextRoomBounds.Item1.x;
        int yMax = nextRoomBounds.Item2.y - nextRoomBounds.Item1.y;
        int zMax = nextRoomBounds.Item2.z - nextRoomBounds.Item1.z;
        
        // Setup a probability distribution for the rooms
        double total = 0;
        List<(double, (int, int))> cutoffs = new List<(double, (int, int))>();

        // Add each required room to the distribution based on their
        // prefer and disprefer point, and the constant weight
        // Only add if they fit
        int requiredRoomIndex = -1;
        foreach (RequiredRoom requiredRoom in superMap.requiredRooms) { 
            requiredRoomIndex ++;
            RoomPrefab requiredRoomPrefab = GetRoomPrefab(requiredRoom.type);
            if (requiredRoomPrefab.xMin <= xMax && requiredRoomPrefab.yMin <= yMax && requiredRoomPrefab.zMin <= zMax) {
                cutoffs.Add((total, (requiredRoom.type, requiredRoomIndex)));
                total += CalculateRequiredRoomWeight(requiredRoom, nextRoomLocation);
            }
        }

        // Add room types based on the superMap weights
        // Only add if they fit
        if (superMap.roomCount < superMap.targetRoomCount) {
            foreach (KeyValuePair<int, double> roomWeight in superMap.roomWeights) {
                int type = roomWeight.Key;
                RoomPrefab roomPrefab = GetRoomPrefab(type);
                if ((!superMap.roomCounts.ContainsKey(type) || !superMap.roomMaxes.ContainsKey(type) || superMap.roomCounts[type] < superMap.roomMaxes[type]) && roomPrefab.xMin <= xMax && roomPrefab.yMin <= yMax && roomPrefab.zMin <= zMax) {
                    cutoffs.Add((total, (type, -1)));
                    total += roomWeight.Value;
                }
            }
        }

        // Check to see if there were eligible rooms to add, otherwise return -1
        if (cutoffs.Count() == 0) {
            return -1;
        }

        // Roll and see which room it lands on
        double roll = random.NextDouble() * total;
        int requiredIndex = -1;
        int result = -1;

        foreach ((double, (int, int)) cutoff in cutoffs) {
            // If we are still above the latest cutoff we haven't hit
            // the item AFTER the last eligible item
            if (roll > cutoff.Item1){
                result = cutoff.Item2.Item1;
                requiredIndex = cutoff.Item2.Item2;
            } else {
                break;
            }
        }

        // If it has a valid required room index, we evict that required room
        if (requiredIndex != -1) {
            superMap.requiredRooms.RemoveAt(requiredIndex);
        }

        return result;
    }

    // Based on a roomsekelton and the point the room is being positioned off, use the superMap placement
    // variables to find where the room will be placed within the bounds of the open hyperrectangle
    // It is assumed the room is a hyperrectangle being placed within the free hyperrectangle
    public static Point PickAnchorPoint(SuperMap superMap, System.Random random, Point nextRoomLocation, (Point, Point) nextRoomBounds, RoomSkeleton nextRoomSkeleton) {
        // Debug.Log("Lower Bound = " + nextRoomBounds.Item1.x + ", y = " + nextRoomBounds.Item1.y + ", z = " + nextRoomBounds.Item1.z);
        // Debug.Log("Upder Bound = " + nextRoomBounds.Item2.x + ", y = " + nextRoomBounds.Item2.y + ", z = " + nextRoomBounds.Item2.z);
        // Determine the free space after the room is placed along each dimension
        // this is given by the total free minus the amount of space the room takes up
        int xMaxDistance = nextRoomBounds.Item2.x - nextRoomBounds.Item1.x - nextRoomSkeleton.xLength;
        int zMaxDistance = nextRoomBounds.Item2.z - nextRoomBounds.Item1.z - nextRoomSkeleton.zLength;
        int yMaxDistance = nextRoomBounds.Item2.y - nextRoomBounds.Item1.y - nextRoomSkeleton.yLength;

        // Determine x y z
        int x = 0;
        int y = 0;
        int z = 0;

        // TODO: this should place on the lateral axes understand that next location picks y at the bottom of the last room

        // Depending on the direction of the face we are placing off of, move away from the
        // face a random distance based on sparsity, and then give a random alignment along
        // the other dimensions
        if (nextRoomLocation.x == nextRoomBounds.Item1.x) {
            x = nextRoomLocation.x + (int)(superMap.horizontalSparsity * random.NextDouble());
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else if (nextRoomLocation.z == nextRoomBounds.Item1.z) {
            z = nextRoomLocation.z + (int)(superMap.horizontalSparsity * random.NextDouble());
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else if (nextRoomLocation.y == nextRoomBounds.Item1.y) {
            y = nextRoomLocation.y + (int)(superMap.verticalSparsity * random.NextDouble());
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
        } else if (nextRoomLocation.x == nextRoomBounds.Item2.x) {
            x = nextRoomLocation.x - (int)(superMap.horizontalSparsity * random.NextDouble()) - nextRoomSkeleton.xLength;
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else if (nextRoomLocation.x == nextRoomBounds.Item2.z) {
            z = nextRoomLocation.z - (int)(superMap.horizontalSparsity * random.NextDouble()) - nextRoomSkeleton.zLength;
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            y = nextRoomLocation.y + (int)(superMap.verticalAlignMent * (random.NextDouble() - 0.5));
        } else {
            y = nextRoomLocation.y - (int)(superMap.verticalSparsity * random.NextDouble()) - nextRoomSkeleton.yLength;
            x = nextRoomLocation.x + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.xLength / 2);
            z = nextRoomLocation.z + (int)(superMap.horizontalAlignMent * (random.NextDouble() - 0.5)) - (nextRoomSkeleton.zLength / 2);
        }

        // Check if the alignment or sparsity changes put the room over its bounds
        // If so adjust the room back within the bounds
        if (x < nextRoomBounds.Item1.x) {
            x = nextRoomBounds.Item1.x;
        } else if (x > nextRoomBounds.Item2.x - nextRoomSkeleton.xLength) {
            x = nextRoomBounds.Item2.x - nextRoomSkeleton.xLength;
        }

        if (z < nextRoomBounds.Item1.z) {
            z = nextRoomBounds.Item1.z;
        } else if (z > nextRoomBounds.Item2.z - nextRoomSkeleton.zLength) {
            z = nextRoomBounds.Item2.z - nextRoomSkeleton.zLength;
        }

        if (y < nextRoomBounds.Item1.y) {
            y = nextRoomBounds.Item1.y;
        } else if (y > nextRoomBounds.Item2.y - nextRoomSkeleton.yLength) {
            y = nextRoomBounds.Item2.y - nextRoomSkeleton.yLength;
        }

        return new Point(x, y, z);
    }

    // Based on an anchor point, draw the values of a room skeleton into the superMap skeleton
    public static SuperMap PlaceRoom(SuperMap superMap, RoomSkeleton roomSkeleton, Point anchorPoint, RoomPrefab roomPrefab) {
        Debug.Log("Starting PlaceRoom");

        Debug.Log("Ancher Point x = " + anchorPoint.x + ", y = " + anchorPoint.y + ", z = " + anchorPoint.z);
        Debug.Log("Skeleton x = " + roomSkeleton.skeleton.GetLength(0) + ", y = " + roomSkeleton.skeleton.GetLength(1) + ", z = " + roomSkeleton.skeleton.GetLength(2));
        // Iterate through the entirety of the roomskeleton and draw it onto the superMap
        for (int x = 0; x < roomSkeleton.skeleton.GetLength(0); x++) {
            for (int y = 0; y < roomSkeleton.skeleton.GetLength(1); y++) {
                for (int z = 0; z < roomSkeleton.skeleton.GetLength(2); z++) {
                    if (roomSkeleton.skeleton[x,y,z] != 0) {
                        superMap.skeleton[anchorPoint.x + x, anchorPoint.y + y, anchorPoint.z + z] = roomSkeleton.skeleton[x,y,z];
                    }
                }
            }
        }

        // Create the new room and add it to the superMap
        Room room = new Room(roomPrefab, anchorPoint, roomSkeleton);
        superMap.rooms.Add(room);

        // Increment the roomcount
        superMap.roomCount += 1;
        if (superMap.roomCounts.ContainsKey(room.type)) {
            superMap.roomCounts[room.type] += 1;
        } else {
            superMap.roomCounts[room.type] = 1;
        }

        // Add the required rooms 
        foreach(int requiredRoomType in roomPrefab.requiredRooms) {
            RoomPrefab requiredRoomPrefab = GetRoomPrefab(requiredRoomType);
            if (requiredRoomPrefab.hasPreference && requiredRoomPrefab.hasDispreference) {
                superMap.requiredRooms.Add(new RequiredRoom(requiredRoomType, room.midPoint, roomPrefab.preference, room.midPoint, roomPrefab.dispreference, roomPrefab.weight));
            } else if (requiredRoomPrefab.hasPreference) {
                superMap.requiredRooms.Add(new RequiredRoom(requiredRoomType, true, room.midPoint, roomPrefab.preference, roomPrefab.weight));
            } else if (requiredRoomPrefab.hasDispreference) {
                superMap.requiredRooms.Add(new RequiredRoom(requiredRoomType, false, room.midPoint, roomPrefab.dispreference, roomPrefab.weight));
            } else {
                superMap.requiredRooms.Add(new RequiredRoom(requiredRoomType, roomPrefab.weight));
            }
        }
        
        // Add the faces portals and nodes to superMaps
        foreach (Face face in roomSkeleton.openFaces) {
            superMap.openFaces.Add(new Face(face, anchorPoint));
        }
        superMap.portals = superMap.portals.Concat(room.portals).ToList();
        superMap.nodes = superMap.nodes.Concat(room.nodes).ToList();
        
        return superMap;
    }
}