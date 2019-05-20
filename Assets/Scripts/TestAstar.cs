using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestAstar : MonoBehaviour
{
    private int[,] arrMap;
    public int col;
    public int row;
    public int tileWidth = 100;
    public int tileHeight = 100;

    public Vector2 startCoord;
    public Vector2 endCoord;
    public Vector2[] arrBlockCoords;


    private GameObject tilePrefab;
    private Dictionary<Node, Tile> dicTile = new Dictionary<Node, Tile>();
    private List<Node> openList = new List<Node>();
    private Node startNode;

    void Start()
    {
        Debug.Log("Hello World!");

        this.tilePrefab = Resources.Load<GameObject>("Tile");
        this.startNode = new Node(startCoord);

        this.CreateMap();
        this.MoveCamera();
        this.AdjacentNode(this.startNode);

        foreach (var node in this.openList)
        {
            Debug.LogFormat("{0}", node.coord);
            var tile = this.dicTile[node];
            tile.SetColor(Color.yellow);
            tile.ShowArrow();
            tile.ShowFGH();
        }
    }

    private void MoveCamera()
    {
        var tcol = (int)this.col / 2;
        var trow = (int)this.row / 2;

        var offsetX = 0;
        var offsetY = 0;

        if (this.col % 2 == 0)
        {
            offsetX = this.tileWidth / 2;
        }
        if (this.row % 2 == 0)
        {
            offsetY = this.tileHeight / 2;
        }

        var offsetVec2 = new Vector2(offsetX, offsetY);

        Debug.Log(offsetVec2);

        var tpos = this.Map2World(new Vector2(tcol, trow), offsetVec2);

        Camera.main.transform.position = new Vector3(tpos.x, tpos.y, -10);

    }

    private void CreateMap()
    {
        var enumerator = this.arrBlockCoords.GetEnumerator();

        for (int i = 0; i < this.col; i++)
        {
            for (int j = 0; j < this.row; j++)
            {
                var coord = new Vector2(i, j);
                this.CreateTile(coord);
            }
        }
    }

    private void CreateTile(Vector2 coord)
    {

        var tileGo = Instantiate(this.tilePrefab);
        var screenPos = new Vector2(coord.x * this.tileWidth, coord.y * -this.tileHeight);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        tileGo.transform.position = worldPos;
        var tile = tileGo.GetComponent<Tile>();

        var enumerator = this.arrBlockCoords.GetEnumerator();

        if (coord == this.startCoord)
        {
            tile.SetColor(Color.green);
            tile.HideArrow();
        }
        else if (coord == this.endCoord)
        {
            tile.SetColor(Color.red);
            tile.HideArrow();
        }
        else
        {
            tile.SetColor(Color.black);
            tile.HideArrow();
        }

        while (enumerator.MoveNext())
        {
            if ((Vector2)enumerator.Current == coord)
            {
                tile.IsBlock = true;
                tile.SetColor(Color.blue);
            }
        }

        tile.HideFGH();
        tile.Init(coord);
        this.dicTile.Add(tile.Node, tile);
    }

    private Vector2 Map2World(Vector2 coord, Vector2 offsetPos)
    {
        var screenPos = new Vector2(coord.x * this.tileWidth, coord.y * -this.tileHeight);
        screenPos.x -= offsetPos.x;
        screenPos.y += offsetPos.y;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }

    //private Vector2 World2Map(Vector2 pos)
    //{

    //}

    private void AdjacentNode(Node node)
    {
        
        System.Action<Vector2, float, int> addOpenList = (coord, angle, g) => {
            if (coord.x >= 0 && coord.y >= 0) {
                //InvalidOperationException: Sequence contains no elements
                try
                {
                    var tile = this.dicTile.Where(x => x.Value.Node.coord == coord).First().Value;
                    var foundCoord = tile.Node.coord;
                    Debug.LogFormat("foundCoord: {0}, {1}", tile.Node.coord.x, tile.Node.coord.y);
                    Debug.LogFormat("endCoord: {0}, {1}", endCoord.x, endCoord.y);



                    float dx = Mathf.Abs(endCoord.x - foundCoord.x);
                    float dy = Mathf.Abs(endCoord.y - foundCoord.y);
                    float h = 100 * (dx + dy);

                    tile.Node.g = g;
                    tile.Node.h = h;
                    tile.Node.f = g + h;

                    if (!tile.IsBlock)
                    {
                        this.openList.Add(tile.Node);
                    }
                    tile.arrowAngle = angle;
                }
                catch (InvalidOperationException e)
                {
                    Debug.Log(e.StackTrace);
                    Debug.LogFormat("<color=red>coord: {0}</color>", coord);
                }
                
            }
        };

        var left = node.coord + Vector2.left;
        var right = node.coord + Vector2.right;
        var up = node.coord + Vector2.up;
        var down = node.coord + Vector2.down;
        var leftUp = node.coord + Vector2.left + Vector2.up;
        var rightUp = node.coord + Vector2.right + Vector2.up;
        var leftDown = node.coord + Vector2.left+ Vector2.down;
        var rightDown = node.coord + Vector2.right + Vector2.down;

        addOpenList(left, -90, 100);
        addOpenList(right, 90, 100);
        addOpenList(up, 0, 100);
        addOpenList(down, -180, 100);
        addOpenList(leftUp, -45, 140);
        addOpenList(rightUp, 45, 140);
        addOpenList(leftDown, -135, 140);
        addOpenList(rightDown, 135, 140);
    }
}
