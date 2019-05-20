using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node 
{
    //public Node parentNode;
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
}
