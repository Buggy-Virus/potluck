using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal {
    public int type;
    public Point point;
    public int width;
    public int height;
    public int direction;
    public Node node;
    public int edgeCount;
    public bool setup;
    public List<Edge> edges;
}