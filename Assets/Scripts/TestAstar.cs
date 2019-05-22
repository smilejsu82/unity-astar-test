using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TestAstar : MonoBehaviour
{
    private int[,] arrMap;
    public int col;
    public int row;
    public int tileWidth = 100;
    public int tileHeight = 100;
    public Vector2[] arrBlockCoords;
    public Vector2 startCoord;
    public Vector2 endCoord;
    public GameObject tilePrefab;
    private Dictionary<Node, Tile> dicTile;

    public Button btnNext;
    public Button btnSelected;
    public Button btnAdjacent;
    public Button btnClear;
    public Button btnOpenList;
    public Button btnCloseList;


    //선택된 타일 
    private Tile selectedTile;
    //인접한 타일들 
    private List<Tile> adjacentTiles;
    //열린목록 
    private List<Tile> openList;
    //닫힌목록 
    private List<Tile> closeList;

    private void Awake()
    {
        this.dicTile = new Dictionary<Node, Tile>();
        this.adjacentTiles = new List<Tile>();
        this.openList = new List<Tile>();
        this.closeList = new List<Tile>();
    }

    private void Start()
    {
        this.CreateMap();
        this.InitTileColor();
        this.InitButtonEvents();

        //타일선택 
        this.GetSelectTile();

        //선택타일의 인접타일들을 검색 
        this.GetAdjecentTiles();

        //인접타일들을 열린목록에 넣기
        this.AddToOpenList();

        //닫힌 목록에 추가 
        this.AddCloseList();

        //열린목록에 있는 모든 타일들의 비용을 측정
        this.CalcFGH();

        this.ShowFGH();

        this.MoveCamera();
    }

    private void ShowFGH()
    {
        foreach (var pair in this.dicTile)
        {
            var tile = pair.Value;
            if (this.openList.Contains(tile))
            {
                tile.ShowFGH();
            }
            else
            {
                tile.HideFGH();
            }
        }
    }

    private void CalcFGH(Tile tile)
    {
        //Coord -> World
        var adjacentTilePos = Map2World(tile.Node.coord, Vector2.zero);
        var selectedTilePos = Map2World(this.selectedTile.Node.coord, Vector2.zero);
        var distance = Vector2.Distance(adjacentTilePos, selectedTilePos);
        var g = (float)Math.Round(distance, 2) * 10;

        var dx = Mathf.Abs(this.endCoord.x - tile.Node.coord.x);
        var dy = Mathf.Abs(this.endCoord.y - tile.Node.coord.y);
        var h = (dx + dy) * 10;

        var f = g + h;

        tile.Node.f = f;
        tile.Node.g = tile.Node.parentNode.g + g;
        tile.Node.h = h;
    }


    private void CalcFGH()
    {
        foreach (var tile in this.openList)
        {
            //Coord -> World
            var adjacentTilePos = Map2World(tile.Node.coord, Vector2.zero);
            var selectedTilePos = Map2World(this.selectedTile.Node.coord, Vector2.zero);
            var distance = Vector2.Distance(adjacentTilePos, selectedTilePos);
            var g = (float)Math.Round(distance, 2) * this.tileWidth;

            var dx = Mathf.Abs(this.endCoord.x - tile.Node.coord.x);
            var dy = Mathf.Abs(this.endCoord.y - tile.Node.coord.y);
            var h = (dx + dy) * this.tileWidth;

            var f = g + h;

            tile.Node.f = f;
            tile.Node.g = tile.Node.parentNode.g + g;
            tile.Node.h = h;
        }
    }

    private void AddCloseList()
    {
        if (this.openList.Contains(this.selectedTile))
        {
            this.openList.Remove(this.selectedTile);
        }

        this.closeList.Add(this.selectedTile);
    }

    private List<Tile> AddToOpenList()
    {
        var list = new List<Tile>();
        foreach (var tile in this.adjacentTiles)
        {
            if (!this.openList.Contains(tile))
            {
                list.Add(tile);

                tile.Node.parentNode = this.selectedTile.Node;
                this.openList.Add(tile);

                tile.ShowArrow();
            }
        }
        return list;
    }

    private void InitButtonEvents()
    {
        this.btnNext.onClick.AddListener(() => {

            //타일선택
            this.GetSelectTile();

            //인접타일 구하기 
            this.GetAdjecentTiles();

            //열린 목록에 넣기 
            this.AddToOpenList();

            //닫힌 목록에 추가 
            this.AddCloseList();

            //FGH계산 
            this.CalcFGH();

            //열린목록에 잇는 모든 타일들의 F값을 표시 
            this.DisplayFGH();

        });

        this.btnSelected.onClick.AddListener(() => {
            this.selectedTile.SetBgColor(Color.cyan);
        });

        this.btnClear.onClick.AddListener(() => {
            this.InitTileColor();
        });

        this.btnAdjacent.onClick.AddListener(() => {
            foreach (var tile in this.adjacentTiles)
            {
                tile.SetBgColor(Color.yellow);
            }
        });

        this.btnOpenList.onClick.AddListener(() => {
            foreach (var tile in this.openList)
            {
                tile.SetBgColor(Color.yellow);
            }
        });

        this.btnCloseList.onClick.AddListener(() => {
            foreach (var tile in this.closeList)
            {
                tile.SetBgColor(Color.yellow);
            }
        });

    }

    private void DisplayFGH()
    {
        foreach (var pair in this.dicTile)
        {
            var tile = pair.Value;
            if (this.openList.Contains(tile))
            {
                tile.ShowFGH();
            }
            else
            {
                tile.HideFGH();
            }
        }
    }

    //타일을 선택한다.
    private void GetSelectTile()
    {
        if (this.selectedTile == null)
        {
            this.selectedTile = this.dicTile.Where(x => x.Value.Node.coord == this.startCoord).FirstOrDefault().Value;
        }
        else
        {
            //다음 노드 선택 
            //F값이 제일 작은타일들을 열린목록에서 선택한다.

            var list = new List<Node>();
            foreach (var tile in this.openList)
            {
                list.Add(tile.Node);
            }
            list.Sort();
            var node = list.FirstOrDefault();
            this.selectedTile = this.dicTile[node];

            Debug.LogFormat("selectedTile: {0}", selectedTile);
            //this.openList.Sort();
            //this.selectedTile = this.openList.First();
        }
    }

    //선택된 주변 타일들을 검색 한다.
    private void GetAdjecentTiles()
    {
        this.adjacentTiles.Clear();

        var coord = this.selectedTile.Node.coord;
        var left = coord + Vector2.left;
        var right = coord + Vector2.right;
        var up = coord + Vector2.up;
        var down = coord + Vector2.down;
        var leftUp = coord + Vector2.left + Vector2.up;
        var rightUp = coord + Vector2.right + Vector2.up;
        var leftDown = coord + Vector2.left + Vector2.down;
        var rightDown = coord + Vector2.right + Vector2.down;

        Vector2[] arrAdjecentTileCoords = { left, right, up, down, leftUp, rightUp, leftDown, rightDown };
        //InvalidOperationException: Sequence contains no elements
        foreach (var cord in arrAdjecentTileCoords)
        {
            var tile = this.dicTile.Where(x => x.Value.Node.coord == cord).FirstOrDefault().Value;
            if (tile != null)
            {
                if (!tile.IsBlock && !this.closeList.Contains(tile))
                {
                    this.adjacentTiles.Add(tile);
                }
            }
        }
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
        tile.Init(coord);
        this.dicTile.Add(tile.Node, tile);
    }

    private void InitTileColor()
    {
        foreach (var pair in this.dicTile)
        {
            var tile = pair.Value;

            var enumerator = this.arrBlockCoords.GetEnumerator();

            if (tile.Node.coord == this.startCoord)
            {
                tile.SetBgColor(Color.green);
                tile.HideArrow();
            }
            else if (tile.Node.coord == this.endCoord)
            {
                tile.SetBgColor(Color.red);
                tile.HideArrow();
            }
            else
            {
                tile.SetBgColor(Color.black);
                tile.HideArrow();
            }

            while (enumerator.MoveNext())
            {
                if ((Vector2)enumerator.Current == tile.Node.coord)
                {
                    tile.IsBlock = true;
                    tile.SetBgColor(Color.blue);
                }
            }
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

        Debug.Log("offsetVec2: " + offsetVec2);

        var tpos = this.Map2World(new Vector2(tcol, trow), offsetVec2);

        Camera.main.transform.position = new Vector3(tpos.x, tpos.y, -10);

    }

    public Vector2 Map2World(Vector2 coord, Vector2 offsetPos)
    {
        var screenPos = new Vector2(coord.x * this.tileWidth, coord.y * -this.tileHeight);
        screenPos.x -= offsetPos.x;
        screenPos.y += offsetPos.y;
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
