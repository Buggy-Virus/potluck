using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    // 0 is start
    // 1 is objective
    public int id;
    public int zone;
    public Point midPoint;
    public List<Portal> portals;
    public List<Edge> edges;
    public int type;    
}