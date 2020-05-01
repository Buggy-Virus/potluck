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

    public int HORIZONTAL_SPARSITY = 5;
    public int VERTICAL_SPARSITY = 7;
    public int SPARSITY_FACTOR = 10;
    public int HORIZONTAL_ALIGNMENT = 5;
    public int VERTICAL_ALIGNMENT = 0;

    public int FIRST_ROOM = 1;
    public Dictionary<int, int> ROOM_MAXES = new Dictionary<int, int>() {
        {0, 99}
    };
    public List<int> REQUIRED_ROOMS = new List<int>() {2, 2};
    public Dictionary<int, double> ROOM_WEIGHTS = new Dictionary<int, double>() {
        {3, 100.0}
    };

    public double PORTAL_DISTANCE_FACTOR = 0.7;
    public Dictionary<int, double> ZONE_EDGE_WEIGHTS = new Dictionary<int, double>() {
        {1, 0.0},
        {2, 0.1}
    };
    

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
                    if (skeleton[x, y, z] == 1) {
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

        superMap = new SuperMap(X_SIZE, Y_SIZE, Z_SIZE,
                                TARGET_ROOM_COUNT, TARGET_ZONE_SIZE, ZONE_RANGE,
                                TARGET_ZONE_EDGES, ZONE_EDGES_RANGE,
                                HORIZONTAL_SPARSITY, VERTICAL_SPARSITY, SPARSITY_FACTOR,
                                HORIZONTAL_ALIGNMENT, VERTICAL_ALIGNMENT,
                                FIRST_ROOM, REQUIRED_ROOMS, ROOM_WEIGHTS, ROOM_MAXES,
                                PORTAL_DISTANCE_FACTOR, ZONE_EDGE_WEIGHTS);
    }

    void Update() {
        
    }
}
