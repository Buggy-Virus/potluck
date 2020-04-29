using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge {
    public Portal source;
    public Portal sink;
    public int length;
    public int width;
    public int height;
    public List<Point> path; // THIS SHOULD BE A TUPLE
    public List<int> directions; // THIS SHOULD BE A TUPLE
    public bool connectable;
    public bool drawn;
    public int type;
    public int conditional;
    public int key;

    public Edge(Portal sourcePortal_input, Portal sinkPortal_input) {
        this.source = sourcePortal_input;
        this.sink = sinkPortal_input;
        this.type = sourcePortal_input.type;
        this.width = Math.Max(sink.width, source.width);
        this.height = Math.Max(sink.height, source.height);
        this.connectable = true;
        this.drawn = false;
        this.length = 0;
        this.conditional = 0;
        this.key = 0;
        path = new List<Point>();
        directions = new List<int>();
        sourcePortal_input.node.edges.Add(this);
        sinkPortal_input.node.edges.Add(this);
        sourcePortal_input.edges.Add(this);
        sinkPortal_input.edges.Add(this);
        sourcePortal_input.edgeCount += 1;
        sinkPortal_input.edgeCount += 1;
    }
}