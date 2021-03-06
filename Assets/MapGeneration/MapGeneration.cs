﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration {
    static public int[,,] GenereateDepthMap2D(int height, int size,
                                       float frequency, int octaves, float lacunarity, float persistence,
                                       System.Random random) {
        int[,,] depthMap = new int[size, height, size];

        float stepSize = 1f / size;

        int xOffset = random.Next(0, 30);
        int zOffset = random.Next(0, 30);

        Vector3 point00 = new Vector3(xOffset, zOffset);
        Vector3 point10 = new Vector3(xOffset + 1, zOffset);
        Vector3 point01 = new Vector3(xOffset, zOffset + 1);
        Vector3 point11 = new Vector3(xOffset + 1, zOffset + 1);
        for (int z = 0; z < size; z++) {
            Vector3 point0 = Vector3.Lerp(point00, point01, z * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, z * stepSize);
            for (int x = 0; x < size; x++) {
                Vector3 point = Vector3.Lerp(point0, point1, x * stepSize);
                float sample = Noise.Sum(Noise.Simplex2D, point, frequency, octaves, lacunarity, persistence).value;
                int y = (int)(height * (sample * 0.5f + 0.5f));
                for (int i = height - 1; i >= 0; i--) {
                    depthMap[x, i, z] = i - y;
                }
            }
        }

        return depthMap;
    }

    static public float[,,] GenerateValueMap(int height, int size,
                                    float frequency, int octaves, float lacunarity, float persistence,
                                    System.Random random) {
        float[,,] valueMap = new float[size, size, size];

        float stepSize = 1f / size;

        int xOffset = random.Next(0, 30);
        int yOffset = random.Next(0, 30);
        int zOffset = random.Next(0, 30);

        Vector3 point000 = new Vector3(xOffset, yOffset, zOffset);
        Vector3 point100 = new Vector3(xOffset + 1, yOffset, zOffset);
        Vector3 point010 = new Vector3(xOffset, yOffset + 1, zOffset);
        Vector3 point001 = new Vector3(xOffset, yOffset, zOffset + 1);
        Vector3 point110 = new Vector3(xOffset + 1, yOffset + 1, zOffset);
        Vector3 point101 = new Vector3(xOffset + 1, yOffset, zOffset + 1);
        Vector3 point011 = new Vector3(xOffset, yOffset + 1, zOffset + 1);
        Vector3 point111 = new Vector3(xOffset + 1, yOffset + 1, zOffset + 1);
        for (int y = 0; y < size; y++) {
            Vector3 point00 = Vector3.Lerp(point000, point010, y * stepSize);
            Vector3 point10 = Vector3.Lerp(point100, point110, y * stepSize);
            Vector3 point01 = Vector3.Lerp(point001, point011, y * stepSize);
            Vector3 point11 = Vector3.Lerp(point101, point111, y * stepSize);
            for (int z = 0; z < size; z++) {
                Vector3 point0 = Vector3.Lerp(point00, point01, z * stepSize);
                Vector3 point1 = Vector3.Lerp(point10, point11, z * stepSize);
                for (int x = 0; x < size; x++) {
                    Vector3 point = Vector3.Lerp(point0, point1, x * stepSize);
                    float sample = Noise.Sum(Noise.Simplex3D, point, frequency, octaves, lacunarity, persistence).value;
                    valueMap[x, y, z] = sample;
                }
            }
        }

        return valueMap;
    }

    static public int[,,] DepthMapToSkeleton(int[,,] depthMap, ITTMethod terrainMethod) {
        int xSize = depthMap.GetLength(0);
        int ySize = depthMap.GetLength(1);
        int zSize = depthMap.GetLength(2);
        int[,,] skeleton = new int[xSize, ySize, zSize];

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    skeleton[x, y, z] = terrainMethod(depthMap[x, y, z]);
                }
            }
        }

        return skeleton;
    }

    static public int[,,] ValueMapToSkeleton(float[,,] valueMap, VTTMethod terrainMethod) {
        int xSize = valueMap.GetLength(0);
        int ySize = valueMap.GetLength(1);
        int zSize = valueMap.GetLength(2);
        int[,,] skeleton = new int[xSize, ySize, zSize];

        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                for (int z = 0; z < zSize; z++) {
                    skeleton[x, y, z] = terrainMethod(valueMap[x, y, z]);
                }
            }
        }

        return skeleton;
    }
}
