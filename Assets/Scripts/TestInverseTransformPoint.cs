using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInverseTransformPoint : MonoBehaviour
{
    public Tile2 tilePrefab;
    public int tileWidth;
    public int tileHeight;
    public Button btn;

    void Start()
    {
        Debug.Log("Hello World!");

        var tileParent = this.CreateTile(new Vector2(0, 2));
        var tile1 = this.CreateTile(new Vector2(1, 1));
        var tile2 = this.CreateTile(new Vector2(1, 2));

        tile1.parent = tileParent;
        tile2.parent = tileParent;

        tileParent.SetBgColor(Color.green);
        tileParent.HideArrow();

        this.btn.onClick.AddListener(() => {
            tile1.UpdateArrow();
            tile2.UpdateArrow();
        });

        this.MoveCamera(tileParent.transform.position);
    }

    private void MoveCamera(Vector3 pos)
    {
        pos.z = -10;
        Camera.main.transform.position = pos;
    }

    public Tile2 CreateTile(Vector2 coord)
    {
        var tile = Instantiate(this.tilePrefab);
        tile.transform.position = this.Map2World(coord);
        return tile.GetComponent<Tile2>();
    }

    public Vector2 Map2World(Vector2 coord)
    {
        var screenPos = new Vector2(coord.x * this.tileWidth, coord.y * -this.tileHeight);

        Debug.Log(screenPos);

        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return worldPos;
    }
}
