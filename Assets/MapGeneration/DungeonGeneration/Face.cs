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

    public Face(Point corner00, Point corner11, int direction) {
        this.direction = direction;

        if (direction == 1 || direction == 4) {
            this.corner00 = corner00;
            this.corner10 = new Point(corner00.x, corner11.y, corner00.z);
            this.corner01 = new Point(corner00.x, corner00.y, corner11.z);
            this.corner11 = corner11;
        } else if (direction == 2 || direction == 5) {
            this.corner00 = corner00;
            this.corner10 = new Point(corner11.x, corner00.y, corner00.z);
            this.corner01 = new Point(corner00.x, corner00.y, corner11.z);
            this.corner11 = corner11;
        } else {
            this.corner00 = corner00;
            this.corner10 = new Point(corner11.x, corner00.y, corner00.z);
            this.corner01 = new Point(corner00.x, corner11.y, corner00.z);
            this.corner11 = corner11;
        }
    }

    public Face(Face face, Point anchorPoint) {
        this.corner00 = face.corner00 + anchorPoint;
        this.corner01 = face.corner01 + anchorPoint;
        this.corner10 = face.corner10 + anchorPoint;
        this.corner11 = face.corner11 + anchorPoint;
        this.direction = face.direction;
    }

    public override bool Equals(object obj) {
        return obj is Face && Equals((Face) obj);
    }

    public bool Equals(Face f) {
        return (corner00 == f.corner00 && corner11 == f.corner11) || (corner00 == f.corner01 && corner11 == f.corner10) || (corner00 == f.corner11 && corner11 == f.corner00) || (corner00 == f.corner10 && corner11 == f.corner01);
    }
}