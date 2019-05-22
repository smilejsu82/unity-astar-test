using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IComparable<Node>
{
    public Node parentNode;
    public Vector2 coord;
    public float f;
    public float g;
    public float h;

    public Node()
    {

    }

    public Node(Vector2 coord)
    {
        this.coord = coord;
    }

    public int CompareTo(Node other)
    {
        if (other == null) return 1;
        else return this.f.CompareTo(other.f);
    }
}
