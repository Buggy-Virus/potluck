using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public delegate int VTTMethod (float value);

public delegate int ITTMethod (int depth);

public class Terrain : MonoBehaviour {
    public GameObject CUBE_PLACEHOLDER;

    public Dictionary<int, GameObject> terrainDict; 

    public int GenerationTestTerrain(int depth) {
        if (depth <= 0) {
            return 1;
        } else {
            return 0;
        }
    }

    public int GenerationTestTerrain(float value) {
        if (value <= 0) {
            return 1;
        } else {
            return 0;
        }
    }

    void Start() {
        terrainDict = new Dictionary<int, GameObject>(){
            {1, CUBE_PLACEHOLDER}
        };
    }
}