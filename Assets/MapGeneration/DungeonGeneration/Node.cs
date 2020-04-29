using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public int id;
    public int zone;
    public Point midPoint;
    public List<Portal> portals;
    public List<Edge> edges;
    public int type;
    public bool goal;
    public bool origin;
}