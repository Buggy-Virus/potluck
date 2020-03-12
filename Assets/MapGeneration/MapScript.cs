using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    public int MAP_HEIGHT = 100;
    public int MAP_SIZE = 100;

    public float FREQUENCY = 3f;
    public int OCTAVES = 5;
    public float LACUNARITY = 2f;
    public float PERSISTENCE = 0.5f;

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

    void Start() {
        random = new System.Random();
    }

    void Update() {
        
    }
}
