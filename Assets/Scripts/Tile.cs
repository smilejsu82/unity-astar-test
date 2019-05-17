using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject arrowGo;
    private SpriteRenderer spriteRenderer;
    private TextMesh textMesh;

    public TextMesh textMeshF;
    public TextMesh textMeshG;
    public TextMesh textMeshH;

    [HideInInspector]
    public float arrowAngle;

    public Node Node { get; private set; }
    public bool IsBlock { get; set; }

    private void Awake()
    {
        this.Node = new Node();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.textMesh = this.transform.Find("textmesh_coord").GetComponent<TextMesh>();
        this.textMeshF.color = Color.black;
        this.textMeshG.color = Color.black;
        this.textMeshH.color = Color.black;
    }

    public void Init(Vector2 coord)
    {
        this.Node.coord = coord;
        this.textMesh.text = coord.x + " , " + coord.y;
    }

    public void SetColor(Color color)
    {
        this.spriteRenderer.color = color;
    }

    public void HideArrow()
    {
        this.arrowGo.SetActive(false);
    }
    public void ShowArrow()
    {
        this.arrowGo.SetActive(true);
        this.arrowGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.arrowAngle));
    }

    public void ShowFGH()
    {
        this.textMeshF.text = this.Node.f.ToString();
        this.textMeshG.text = this.Node.g.ToString();
        this.textMeshH.text = this.Node.h.ToString();

        this.textMeshF.gameObject.SetActive(true);
        this.textMeshG.gameObject.SetActive(true);
        this.textMeshH.gameObject.SetActive(true);
    }

    public void HideFGH()
    {
        this.textMeshF.gameObject.SetActive(false);
        this.textMeshG.gameObject.SetActive(false);
        this.textMeshH.gameObject.SetActive(false);
    }
}
