using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile2 : MonoBehaviour
{
    public GameObject arrowGo;
    public Tile2 parent;
    
    public void SetBgColor(Color color)
    {
        this.GetComponent<SpriteRenderer>().color = color;
    }

    public void HideArrow()
    {
        this.arrowGo.SetActive(false);
    }

    public void UpdateArrow()
    {
        //부모를 바라보게 한다.
        
        var relative = this.arrowGo.transform.InverseTransformPoint(this.parent.transform.position);

        Debug.LogFormat("{0}\t{1} ---> {2}, {3}, {4}", this.parent.transform.position, this.arrowGo.transform.position, relative, this.transform.position, this.transform.localPosition);

        var angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
        this.arrowGo.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    
}
