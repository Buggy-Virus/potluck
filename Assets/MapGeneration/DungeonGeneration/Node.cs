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
    public bool goal;
    public bool origin;

    public Node() {
        this.id = -1;
        this.zone = -1;
        this.midPoint = new Point();
        this.portals = new List<Portal>();
        this.edges = new List<Edge>();
        this.goal = false;
        this.origin = false;
    }
}