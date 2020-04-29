using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Face {
    public Point corner00;
    public Point corner10;
    public Point corner01;
    public Point corner11; 
    public int direction;

    public Face(Point corner00, Point corner01, Point corner10, Point corner11, int direction) {
        this.corner00 = corner00;
        this.corner01 = corner01;
        this.corner10 = corner10;
        this.corner11 = corner11;
        this.direction = direction;
    }

    public Face(Face face, Point anchorPoint) {
        this.corner00 = face.corner00 + anchorPoint;
        this.corner01 = face.corner01 + anchorPoint;
        this.corner10 = face.corner10 + anchorPoint;
        this.corner11 = face.corner11 + anchorPoint;
        this.direction = face.direction;
    }
}