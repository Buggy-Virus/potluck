using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    // Terrain Stuff
    public int MAP_HEIGHT = 100;
    public int MAP_SIZE = 100;

    public float FREQUENCY = 3f;
    public int OCTAVES = 5;
    public float LACUNARITY = 2f;
    public float PERSISTENCE = 0.5f;

    // Dungeon Stuff
    SuperMap superMap;
    public int X_SIZE = 200;
    public int Y_SIZE = 200;
    public int Z_SIZE = 200;

    public int TARGET_ROOM_COUNT = 20;
    public int TARGET_ZONE_SIZE = 5;
    public int ZONE_RANGE = 3;

    public int TARGET_ZONE_EDGES = 1;
    public int ZONE_EDGES_RANGE = 1;

    public float HORIZONTAL_SPARSITY = 5;
    public float VERTICAL_SPARSITY = 7;
    public float SPARSITY_FACTOR = 10;
    public float HORIZONTAL_ALIGNMENT = 5;
    public float VERTICAL_ALIGNMENT = 0;

    public List<Tuple<int, Point>> REQUIRED_ROOMS = new List<Tuple<int, Point>>() {new Tuple<int, Point>(0, new Point(30, 30, 30))};
    public List<Tuple<double, int>> ROOM_WEIGHTS = new List<Tuple<double, int>>() {new Tuple<double, int>(1.0, 0)};
    public List<Tuple<double, int>> ZONE_EDGE_WEIGHTS = new List<Tuple<double, int>>() {
        new Tuple<double, int>(1.0, 1),
        new Tuple<double, int>(1.0, 2) 
    };
    public Dictionary<int, int> ROOM_MAXES = new Dictionary<int, int>() {{0, 99}};

    public GameObject EMPTY_PREFAB;

    public Terrain terrainScript;

    public int[,,] mapSkeleton;
    public Index[,,] map;

    System.Random random;
    bool mapExists = false;

    public void ClearMap() {
        foreach (Transform child in gameObject.transform) {
            Destroy(child.gameObject);
        }

        map = null;
        mapExists = false;
    }

    public void DrawSkeleton(int[,,] skeleton) {
        for (int y = 0; y < skeleton.GetLength(1); y++) {
            GameObject row = Instantiate(EMPTY_PREFAB, gameObject.transform);
            row.name = "row_" + y;
            for (int x = 0; x < skeleton.GetLength(0); x++) {
                for (int z = 0; z < skeleton.GetLength(2); z++) {
                    if (skeleton[x, y, z] > 0) {
                        Instantiate(terrainScript.terrainDict[skeleton[x, y, z]], new Vector3(x, y, z), Quaternion.identity, row.transform);
                    }
                }
            }
        }
        mapExists = true;
    }

    public void GenerateMap(int[,,] skeleton) {
        map = new Index[skeleton.GetLength(0), skeleton.GetLength(1), skeleton.GetLength(2)];

        for (int y = 0; y < skeleton.GetLength(1); y++) {
            GameObject row = Instantiate(EMPTY_PREFAB, gameObject.transform);
            row.name = "row_" + y;
            for (int x = 0; x < skeleton.GetLength(0); x++) {
                for (int z = 0; z < skeleton.GetLength(2); z++) {
                    if (skeleton[x, y, z] > 0) {
                        GameObject curShape = Instantiate(terrainScript.terrainDict[skeleton[x, y, z]], new Vector3(x, y, z), Quaternion.identity, row.transform);
                        map[x,y,z].shape = curShape;
                    }
                }
            }
        }
        mapExists = true;
    }

    public void TestTerrainGeneration2d() {
        if (mapExists) {
            ClearMap();
            random = new System.Random();
        }
        int[,,] depthMap = MapGeneration.GenereateDepthMap2D(MAP_HEIGHT, MAP_SIZE, FREQUENCY, OCTAVES, LACUNARITY, PERSISTENCE, random);
        int[,,] skeleton = MapGeneration.DepthMapToSkeleton(depthMap, terrainScript.GenerationTestTerrain);
        DrawSkeleton(skeleton);
    }

    public void TestTerrainGeneration() {
        if (mapExists) {
            ClearMap();
            random = new System.Random();
        }
        float[,,] valueMap = MapGeneration.GenerateValueMap(MAP_HEIGHT, MAP_SIZE, FREQUENCY, OCTAVES, LACUNARITY, PERSISTENCE, random);
        int[,,] skeleton = MapGeneration.ValueMapToSkeleton(valueMap, terrainScript.GenerationTestTerrain);
        DrawSkeleton(skeleton);
    }

    public void TestDungeonGeneration() {
        if (mapExists) {
            ClearMap();
            random = new System.Random();
        }
        int[,,] skeleton = DungeonGeneration.GenerateDungeonSuperMap(superMap, random).skeleton;
        DrawSkeleton(skeleton);
    } 

    void Start() {
        random = new System.Random();

        superMap = new SuperMap();
        superMap.xSize = X_SIZE;
        superMap.ySize = Y_SIZE;
        superMap.zSize = Z_SIZE;
        superMap.skeleton = new int[X_SIZE,Y_SIZE,Z_SIZE];
        superMap.targetRoomCount = TARGET_ROOM_COUNT;
        superMap.targetZoneSize = TARGET_ZONE_SIZE;
        superMap.zoneRange = ZONE_RANGE;
        superMap.targetZoneEdges = TARGET_ZONE_EDGES;
        superMap.zoneEdgesRange = ZONE_EDGES_RANGE;

        superMap.horizontalSparsity = HORIZONTAL_SPARSITY;
        superMap.verticalSparsity = VERTICAL_SPARSITY;
        superMap.sparsityFactor = SPARSITY_FACTOR;
        superMap.horizontalAlignMent = HORIZONTAL_ALIGNMENT;
        superMap.verticalAlignMent = VERTICAL_ALIGNMENT;

        superMap.requiredRooms = REQUIRED_ROOMS;
        superMap.roomWeights = ROOM_WEIGHTS;

        superMap.zoneEdgeWeights = ZONE_EDGE_WEIGHTS;

        superMap.roomMaxes = ROOM_MAXES;
        superMap.roomCount = 0;
        superMap.roomCounts = new Dictionary<int, int>();

        superMap.zones = new List<Zone>();
        superMap.rooms = new List<Room>();
        superMap.openFaces = new List<Face>();
        superMap.portals = new List<Portal>();
        superMap.nodes = new List<Node>();

        superMap.currentKey = 0;

        superMap.naiveGraph = new int[0,0];
        superMap.roomGraph = new int[0,0];
        superMap.specialGraph = new int[0,0];
        superMap.zoneGraph = new int[0,0];
        superMap.zoneEdgeGraph = new List<Edge>[0,0];
    }

    void Update() {
        
    }
}
