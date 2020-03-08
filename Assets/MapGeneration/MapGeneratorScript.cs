using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorScript : MonoBehaviour
{
    public int MAP_HEIGHT = 100;
    public int MAP_WIDTH = 100;
    public int MAP_LENGTH = 100;

    public float FREQUENCY = 3f;
    public int OCTAVES = 5;
    public float LACUNARITY = 2f;
    public float PERSISTENCE = 0.5f;

    public GameObject TEST_PREFAB;
    public GameObject EMPTY_PREFAB;

    bool mapExists = false;
    System.Random random;
    int[] hash;

    public void GenerateMap(int[] hash, 
                     int height, int width, int length, 
                     float frequency, int octaves, float lacunarity, float persistence) {
        for (int i = 0; i < height; i++) {
            GameObject row = Instantiate(EMPTY_PREFAB, gameObject.transform);
            row.name = "row_" + i;
            for (int j = 0; j < width; j++) {
                for (int k = 0; k < length; k++) {
                    Vector3 currentPoint = new Vector3(j, i, k);
                    NoiseSample currentPointNoise = Noise.Sum(Noise.Simplex3D, currentPoint, ref hash, frequency, octaves, lacunarity, persistence);

                    if (currentPointNoise.value >= 0) {
                        Instantiate(TEST_PREFAB, currentPoint, Quaternion.identity, row.transform);
                    }
                }
            }
        }

        mapExists = true;
    }

    public void GenerateMap2d(int[] hash, 
                     int height, int width, int length, 
                     float frequency, int octaves, float lacunarity, float persistence) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                // NoiseSample currentPointNoise = Noise.Sum(Noise.Simplex2D, new Vector3(i, j), ref hash, frequency, octaves, lacunarity, persistence);
                NoiseSample currentPointNoise = Noise.Simplex2D(new Vector3(i / 10, j / 10), frequency, ref hash);
                float currentPointHeight = (int)(height * ((currentPointNoise.value + 1) / 2));
                Instantiate(TEST_PREFAB, new Vector3(i, currentPointHeight, j), Quaternion.identity, gameObject.transform);
            }
        }

        mapExists = true;
    }

    public void ClearMap() {
        foreach (Transform child in gameObject.transform) {
            Destroy(child.gameObject);
        }

        mapExists = false;
    }

    public void TestTerrainGeneration() {
        if (mapExists) {
            ClearMap();
        }
        GenerateMap(hash, MAP_HEIGHT, MAP_WIDTH, MAP_LENGTH, FREQUENCY, OCTAVES, LACUNARITY, PERSISTENCE);
    }

    public void TestTerrainGeneration2d() {
        if (mapExists) {
            ClearMap();
        }
        GenerateMap2d(hash, MAP_HEIGHT, MAP_WIDTH, MAP_LENGTH, FREQUENCY, OCTAVES, LACUNARITY, PERSISTENCE);
    }

    void Start() {
        random = new System.Random();
        hash = Noise.GenerateHash(256, random);
    }

    void Update()
    {
        
    }
}
