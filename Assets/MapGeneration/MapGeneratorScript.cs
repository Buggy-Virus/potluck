using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorScript : MonoBehaviour
{
    public int MAP_HEIGHT = 100;
    public int MAP_SIZE = 100;

    public float FREQUENCY = 3f;
    public int OCTAVES = 5;
    public float LACUNARITY = 2f;
    public float PERSISTENCE = 0.5f;

    public GameObject TEST_PREFAB;
    public GameObject EMPTY_PREFAB;

    System.Random random;
    bool mapExists = false;

    public void GenerateMap2d( 
                     int height, int size, 
                     float frequency, int octaves, float lacunarity, float persistence) {
        float stepSize = 1f / size;

        int offset = random.Next(0, 100);

        Vector3 point00 = new Vector3(offset, offset);
        Vector3 point10 = new Vector3(offset + 1, offset);
        Vector3 point01 = new Vector3(offset, offset + 1);
        Vector3 point11 = new Vector3(offset + 1, offset + 1);
        for (int z = 0; z < size; z++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, z * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, z * stepSize);
            for (int x = 0; x < size; x++) {
                Vector3 point = Vector3.Lerp(point0, point1, x * stepSize);
                float sample = Noise.Sum(Noise.Simplex2D, point, frequency, octaves, lacunarity, persistence).value;
                int y = (int)(height * (sample * 0.5f + 0.5f));
                for (int i = 0; i < y; i++) {
                    Instantiate(TEST_PREFAB, new Vector3(x, i, z), Quaternion.identity, gameObject.transform);
                }
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

    public void TestTerrainGeneration2d() {
        if (mapExists) {
            ClearMap();
            random = new System.Random();
        }
        GenerateMap2d(MAP_HEIGHT, MAP_SIZE, FREQUENCY, OCTAVES, LACUNARITY, PERSISTENCE);
    }

    void Start() {
        random = new System.Random();
    }

    void Update() {
        
    }
}
