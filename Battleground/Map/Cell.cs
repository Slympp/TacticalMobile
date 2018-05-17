using LlockhamIndustries.Decals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell {

    public Unit                 IsTaken = null;
    public bool                 IsWalkable = true;

    public int                  GCost;
    public int                  HCost;
    public int                  FCost { get { return GCost + HCost; } }
    public Cell                 Parent;

    public int                  X;
    public float                Y;
    public int                  Z;
    public float                RealY;

    public Material             Material;
    public Material             Filler;

    public Cell() { }

    public Cell(int x, int z, int y, Transform parent, GameObject cellObject, Material mat, Material filler) {
 
        X = x;
        Y = y;
        Z = z;
        Material = mat;
        Filler = filler;

        if (y <= (int)BlocHeight.NonWalkable)
            IsWalkable = false;
    }

    public Cell(int x, int z) {
        this.X = x;
        this.Y = (float)BlocHeight.Empty;
        this.Z = z;

        IsWalkable = false;
    }

    public void BuildCell(Transform parent, GameObject cellObject) {

        int y = (int)Y;
        if (y > (int)BlocHeight.NonWalkable)
            y--;

        RealY = y;
        RealY += Random.Range(-0.1f, 0.1f);

        GameObject go = Object.Instantiate(cellObject, new Vector3(X, RealY, Z), Quaternion.identity);
        go.GetComponent<MeshRenderer>().material = Material;
        go.transform.parent = parent;

        while (y > 0) {
            GameObject filler = Object.Instantiate(cellObject, new Vector3(X, RealY - y, Z), Quaternion.identity);
            filler.GetComponent<MeshRenderer>().material = Filler;
            filler.transform.parent = go.transform;
            y--;
        }
    }

    public Vector3 GetVector3Position() {
        return new Vector3(X, RealY + 1, Z);
    }

    public Vector3 GetVector3PositionSimple() {
        return new Vector3(X, Y, Z);
    }

    public string GetFormattedPosition() {
        string s = "[" + X + ", " + Z + "]";
        return s;
    }

    public int GetDistanceTo(Cell target) {
        return Utils.GetDistanceBetweenCells(this, target);
    }
}

public enum BlocHeight {
    Empty = -1,
    NonWalkable,
    SeaLevel,
    Ground,
    Middle,
    Top
}