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

    public Portal() {
        this.type = -1;
        this.point = new Point();
        this.width = -1;
        this.height = -1;
        this.direction = 0;
        this.node = new Node();
        this.edgeCount = 0;
        this.setup = false;
        this.edges = new List<Edge>();
    }

    public Portal(Portal portal, Point anchorPoint) {
        this.type = portal.type;
        this.width = portal.width;
        this.height = portal.height;
        this.direction = portal.direction;
        this.node = portal.node;
        this.edgeCount = portal.edgeCount;
        this.setup = portal.setup;
        this.edges = portal.edges;

        this.point = portal.point + anchorPoint;
    }
}