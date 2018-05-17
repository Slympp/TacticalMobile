using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    public static int GetDistanceToTarget(Unit unit, Vector3 pos) {
        float dist = Mathf.Abs(unit.CurrentCell.X - pos.x) + Mathf.Abs(unit.CurrentCell.Z - pos.z);
        return (int)dist;
    }

    public static int GetDistanceBetweenCells(Cell cellA, Cell cellB) {
        int dstX = Mathf.Abs(cellA.X - cellB.X);
        int dstY = Mathf.Abs(cellA.Z - cellB.Z);

        return dstX + dstY;
    }

    public static void RenameKey<TKey, TValue>(IDictionary<TKey, TValue> dic,
                                      TKey fromKey, TKey toKey) {
        TValue value = dic[fromKey];
        dic.Remove(fromKey);
        dic[toKey] = value;
    }
}

