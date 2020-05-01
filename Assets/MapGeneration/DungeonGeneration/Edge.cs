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
    public bool hitSource;
    public bool hitSink;
    public bool connectable;
    public int type;
    public int conditional;
    public int key;

    public Edge(Portal source, Portal sink) {
        this.source = source;
        this.sink = sink;
        this.type = source.type;
        this.width = Math.Max(this.sink.width, this.source.width);
        this.height = Math.Max(this.sink.height, this.source.height);
        this.connectable = true;
        this.hitSource = false;
        this.hitSink = false;
        this.length = 0;
        this.conditional = 0;
        this.key = 0;
        path = new List<Point>();
        directions = new List<int>();
        source.node.edges.Add(this);
        sink.node.edges.Add(this);
        source.edges.Add(this);
        sink.edges.Add(this);
        source.edgeCount += 1;
        sink.edgeCount += 1;
    }
}