using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequiredRoom {
    public int type;

    public bool hasPreference;
    public Point preferPoint;
    public double preference;
    public bool hasDispreference;
    public Point dispreferPoint;
    public double dispreference;
    public double weight;

    public RequiredRoom(int type, Point preferPoint, double preference, Point dispreferPoint, double dispreference, double weight) {
        this.type = type;
        this.hasPreference = true;
        this.preferPoint = preferPoint;
        this.preference = preference;
        this.hasDispreference = true;
        this.dispreferPoint = dispreferPoint;
        this.dispreference = dispreference;
        this.weight = weight;
    }

    public RequiredRoom(int type, bool preferred, Point point, double strength, double weight) {
        if (preferred) {
            this.type = type;
            this.hasPreference = true;
            this.preferPoint = point;
            this.preference = strength;
            this.hasDispreference = false;
            this.weight = weight;
        } else {
            this.type = type;
            this.hasPreference = false;
            this.hasDispreference = true;
            this.dispreferPoint = point;
            this.dispreference = strength;
            this.weight = weight;
        }
    }

    public RequiredRoom(int type, double weight) {
        this.type = type;
        this.hasPreference = false;
        this.hasDispreference = false;
        this.weight = weight;
    }
}