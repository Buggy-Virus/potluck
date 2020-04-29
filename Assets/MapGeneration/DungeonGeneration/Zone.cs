using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone {
    public int id;
    public int type;

    public Point midPoint;

    public List<Node> nodes;
    public List<int> keys;

    public Zone(int id_input, int type_input) {
        this.id = id_input;
        this.type = type_input;
        this.nodes = new List<Node>();
        this.keys = new List<int>();
    }

    public Zone(int id, int type, Point midPoint, List<Node> nodes) {
        this.id = id;
        this.type = type;
        this.midPoint = midPoint;
        this.nodes = nodes;
        this.keys = new List<int>();
    }
}