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
    public Button btnShowArrow;


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
        this.openList.Add(this.selectedTile);
        this.AddToOpenList();

        //닫힌 목록에 추가 
        this.AddCloseList();

        this.ShowFGH();

        this.MoveCamera();
    }

    private void ShowFGH()
    {
        foreach (var pair in this.dicTile)
        {
            var tile = pair.Value;

            if (this.openList.Contains(tile) 
                || this.closeList.Contains(tile)
                || tile.Node.coord.Equals(this.startCoord)
                || tile.Node.coord.Equals(this.endCoord))
            {
                tile.ShowFGH();
            }
            else
            {
                tile.HideFGH();
            }
        }
    }

    private void CalcFG(Tile tile)
    {
        //Coord -> World
        var adjacentTile = this.dicTile[tile.Node];
        var selectedTile = this.dicTile[this.selectedTile.Node];
        var parentTile = this.dicTile[tile.Node.parentNode];

        var distance = Vector2.Distance(adjacentTile.transform.position, parentTile.transform.position);
        //var g = distance * 10f;

        var g = Mathf.RoundToInt(distance * 10);

        var f = g + tile.Node.h;

        tile.Node.f = (int)f;
        tile.Node.g = tile.Node.parentNode.g + g;
    }


    private void CalcFGH()
    {
        foreach (var tile in this.openList)
        {
            //Coord -> World
            var adjacentTile = this.dicTile[tile.Node];
            var g = 0;

            if (tile.Node.parentNode != null)
            {
                var parentTile = this.dicTile[tile.Node.parentNode];
                var distance = Vector2.Distance(adjacentTile.transform.position, parentTile.transform.position);
                g = Mathf.RoundToInt(distance * 10) + tile.Node.parentNode.g;
            }

            var dx = Mathf.Abs(this.endCoord.x - tile.Node.coord.x);
            var dy = Mathf.Abs(this.endCoord.y - tile.Node.coord.y);
            var h = (dx + dy) * 10;

            var f = g + h;

            tile.Node.g = (int)g;
            tile.Node.h = (int)h;
            tile.Node.f = (int)f;
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
        Tile targetTile = null;


        foreach (var tile in this.adjacentTiles)
        {
            if (!this.openList.Contains(tile))
            {
                list.Add(tile);

                tile.Node.parentNode = this.selectedTile.Node;
                this.openList.Add(tile);
                
                //FGH계산 
                this.CalcFGH();

                tile.SetStrokeColor("0CFF00");
            }
            else
            {
                targetTile = tile;
            }
        }

        //g값 비교
        if (targetTile != null) {
            foreach (var tile in this.adjacentTiles)
            {
                var adjacentTile = this.openList.Find(x => x.Node.coord == tile.Node.coord);

                if (adjacentTile != targetTile)
                {
                    var distance = Vector2.Distance(targetTile.transform.position, adjacentTile.transform.position);
                    var g = Mathf.RoundToInt(distance * 10) + targetTile.Node.g;
                    
                    if (g < adjacentTile.Node.g)
                    {
                        adjacentTile.Node.g = g;
                        adjacentTile.Node.f = adjacentTile.Node.h + g;
                        adjacentTile.Node.parentNode = targetTile.Node;
                        
                        adjacentTile.UpdateFGH();

                        Debug.LogFormat("coord: {0}, parent: {1}", adjacentTile.Node.coord, adjacentTile.Node.parentNode.coord);
                        
                    }
                }
            }
        }

        foreach (var tile in this.openList)
        {
            tile.ShowArrow();
        }
        

        return list;
    }

    private void InitButtonEvents()
    {
        this.btnNext.onClick.AddListener(() => {

            //닫힌 목록에 추가 
            this.AddCloseList();

            //타일선택
            this.GetSelectTile();

            //인접타일 구하기 
            this.GetAdjecentTiles();

            //열린 목록에 넣기 
            this.AddToOpenList();

            //선택된 타일 닫힌 목록에 넣기
            this.AddCloseList();

            this.ShowFGH();

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

        this.btnShowArrow.onClick.AddListener(() => {
            foreach (var tile in this.openList)
            {
                tile.ShowArrow();
            }
        });
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
            this.openList.Sort();
            this.selectedTile = this.openList.FirstOrDefault();
        }

        this.selectedTile.SetStrokeColor(Color.cyan);
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
        var rightDown = coord + Vector2.right + Vector2.down;
        var leftDown = coord + Vector2.left + Vector2.down;
        var leftUp = coord + Vector2.left + Vector2.up;
        var rightUp = coord + Vector2.right + Vector2.up;

        Vector2[] arrAdjecentTileCoords = { leftUp, up, rightUp, left, right, leftDown, down, rightDown };

        System.Func<Vector2, Vector2, bool> checkCorner = (tileCoord, dir) => {
            var blockCoord = tileCoord + dir;
            if (blockCoord.x > 0 && blockCoord.y > 0)
            {
                var blockTile = this.dicTile.Where(x => x.Value.Node.coord == blockCoord).FirstOrDefault().Value;
                if (blockTile.IsBlock)
                {
                    var relative = this.selectedTile.transform.InverseTransformPoint(blockTile.transform.position);
                    Debug.LogFormat("{0}", relative);
                    if (relative == Vector3.right)
                    {
                        Debug.LogFormat("blockTile: {0}", blockTile.Node.coord);
                        return true;
                    }
                }
            }
            return false;
        };

        foreach (var cord in arrAdjecentTileCoords)
        {
            var tile = this.dicTile.Where(x => x.Value.Node.coord == cord).FirstOrDefault().Value;
            if (tile != null)
            {
                if (!tile.IsBlock && !this.closeList.Contains(tile))
                {

                    if (checkCorner(tile.Node.coord, Vector2.right) ||
                        checkCorner(tile.Node.coord, Vector2.left) ||
                        checkCorner(tile.Node.coord, Vector2.up) ||
                        checkCorner(tile.Node.coord, Vector2.down))
                    {
                        continue;
                    }

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
                tile.SetBgColor("008500");
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
                    tile.HideArrow();
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

    public Tile GetTile(Node node)
    {
        return this.dicTile[node];
    }
}
