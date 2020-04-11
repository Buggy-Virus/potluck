using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
    public static double Distance(Point a, Point b) {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2) + Math.Pow(a.z - b.z, 2));
    }

    public static Point Average(Point a, Point b) {
        return new Point((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
    }

    public static bool CheckEmpty(SuperMap superMap, Point a, Point b) {
        if (a.x < b.x) {
            for (int i = a.x; i <= b.x; i++) {
                if (a.y < b.y) {
                    for (int j = a.y; j <= b.y; j++) {
                        if (a.z < b.z) {
                            for (int k = a.z; k <= b.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        } else {
                            for (int k = b.z; k <= a.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        }
                    }
                } else {
                    for (int j = b.y; j <= a.y; j++) {
                        if (a.z < b.z) {
                            for (int k = a.z; k <= b.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        } else {
                            for (int k = b.z; k <= a.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        } else {
            for (int i = b.x; i <= a.x; i++) {
                if (a.y < b.y) {
                    for (int j = a.y; j <= b.y; j++) {
                        if (a.z < b.z) {
                            for (int k = a.z; k <= b.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        } else {
                            for (int k = b.z; k <= a.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        }
                    }
                } else {
                    for (int j = b.y; j <= a.y; j++) {
                        if (a.z < b.z) {
                            for (int k = a.z; k <= b.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        } else {
                            for (int k = b.z; k <= a.z; k++) {
                                if(superMap.skeleton[i, j, k] != 0) {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}