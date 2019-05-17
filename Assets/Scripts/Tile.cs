using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Vector2 coord;
    private TextMesh textMesh;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.textMesh = this.transform.Find("textmesh_coord").GetComponent<TextMesh>();
    }

    public void Init(Vector2 coord, Color color)
    {
        this.coord = coord;
        this.spriteRenderer.color = color;
        this.textMesh.text = coord.x + " , " + coord.y;
    }
}
