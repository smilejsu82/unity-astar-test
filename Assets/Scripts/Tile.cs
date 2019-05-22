using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, IComparable<Tile>
{
    public GameObject arrowGo;
    public SpriteRenderer spBg;
    public SpriteRenderer spStroke;
    private TextMesh textMesh;

    public TextMesh textMeshF;
    public TextMesh textMeshG;
    public TextMesh textMeshH;

    public Node Node { get; private set; }
    public bool IsBlock { get; set; }

    private void Awake()
    {
        this.Node = new Node();
        this.textMesh = this.transform.Find("textmesh_coord").GetComponent<TextMesh>();
    }

    public void Init(Vector2 coord)
    {
        this.Node.coord = coord;
        this.textMesh.text = coord.x + " , " + coord.y;
    }

    public void HideArrow()
    {
        this.arrowGo.SetActive(false);
    }
    public void ShowArrow()
    {
        var testAstar = GameObject.FindObjectOfType<TestAstar>();
        if (this.Node.parentNode != null)
        {

            var parentTile = testAstar.GetTile(this.Node.parentNode);
            var relative = this.arrowGo.transform.InverseTransformPoint(parentTile.transform.position);
            var angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
            this.arrowGo.SetActive(true);
            this.arrowGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
        }
        
    }

    private Vector2 Map2World(Vector2 coord)
    {
        var screenPos = new Vector2(coord.x * 100, coord.y * -100);
        //screenPos.x -= offsetPos.x;
        //screenPos.y += offsetPos.y;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    public void SetStrokeColor(string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(string.Format("#{0}", hexColor), out color))
        {
            this.spStroke.color = color;
        }
    }

    public void SetStrokeColor(Color color)
    {
        this.spStroke.color = color;
    }
    public void SetBgColor(string hexColor)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(string.Format("#{0}", hexColor), out color))
        {
            this.spBg.color = color;
        }
    }

    public void SetBgColor(Color color)
    {
        this.spBg.color = color;
    }

    public void UpdateFGH()
    {
        this.textMeshF.text = this.Node.f.ToString();
        this.textMeshG.text = this.Node.g.ToString();
        this.textMeshH.text = this.Node.h.ToString();
    }

    public void ShowFGH()
    {
        this.UpdateFGH();

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

    public int CompareTo(Tile other)
    {
        if (this.Node.f.Equals(other.Node.f))  //if both f values are same
            return -this.Node.coord.y.CompareTo(other.Node.coord.y); // then compare coord y
        else
            return this.Node.f.CompareTo(other.Node.f);
    }
}
