using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAstar : MonoBehaviour
{
    private int[,] arrMap;
    public int col;
    public int row;
    public int tileWidth = 100;
    public int tileHeight = 100;

    private GameObject tilePrefab;
    void Start()
    {
        Debug.Log("Hello World!");
        this.tilePrefab = Resources.Load<GameObject>("Tile");

        this.CreateMap();
        this.MoveCamera();
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
        for (int i = 0; i < this.col; i++)
        {
            for (int j = 0; j < this.row; j++)
            {
                var coord = new Vector2(i, j);

                if (coord == new Vector2(1, 2))
                {
                    this.CreateTile(coord, Color.green);
                }
                else if (coord == new Vector2(3, 1) || coord == new Vector2(3, 2) || coord == new Vector2(3, 3))
                {
                    this.CreateTile(coord, Color.blue);
                }
                else if (coord == new Vector2(5, 2))
                {
                    this.CreateTile(coord, Color.red);
                }
                else
                {
                    this.CreateTile(coord, Color.black);
                }
            }
        }
    }

    private void CreateTile(Vector2 coord, Color color)
    {
        var tileGo = Instantiate(this.tilePrefab);
        var screenPos = new Vector2(coord.x * this.tileWidth, coord.y * -this.tileHeight);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        tileGo.transform.position = worldPos;
        var tile = tileGo.GetComponent<Tile>();
        tile.Init(coord, color);
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
}
