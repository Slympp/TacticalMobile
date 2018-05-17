using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding {

    public static List<Cell> FindPath(MapInfos map, Cell startCell, Cell targetCell, bool exceptLastCell = false) {
        // find path
        List<Cell> path = _ImpFindPath(map, startCell, targetCell, exceptLastCell);

        ClearPathfinding(map);

        return path;
    }

    // internal function to find path, don't use this one from outside
    private static List<Cell> _ImpFindPath(MapInfos map, Cell startCell, Cell targetCell, bool exceptLastCell) {
        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        openSet.Add(startCell);

        while (openSet.Count > 0) {
            Cell currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetCell) {
                return RetracePath(startCell, targetCell);
            }

            foreach (Cell neighbour in GetNeighbours(map, currentNode)) {
                if (exceptLastCell && targetCell.IsTaken && neighbour == targetCell) {
                    neighbour.Parent = currentNode;
                    return RetracePath(startCell, neighbour);
                }

                if (!neighbour.IsWalkable || (!exceptLastCell && neighbour.IsTaken != null) || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.GCost + Utils.GetDistanceBetweenCells(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour)) {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = Utils.GetDistanceBetweenCells(neighbour, targetCell);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private static List<Cell> RetracePath(Cell startCell, Cell endCell) {
        List<Cell> path = new List<Cell>();
        Cell currentNode = endCell;

        while (currentNode != startCell) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }

    private static List<Cell> GetNeighbours(MapInfos map, Cell cell) {
        List<Cell> neighbours = new List<Cell>();

        if (cell.X + 1 >= 0 && cell.X + 1 < map.Size.x &&
            cell.Z >= 0 && cell.Z < map.Size.y &&
            Mathf.Abs(map.Map[cell.X + 1, cell.Z].Y - cell.Y) <= 1) {
            neighbours.Add(map.Map[cell.X + 1, cell.Z]);
        }

        if (cell.X - 1 >= 0 && cell.X - 1 < map.Size.x &&
            cell.Z >= 0 && cell.Z < map.Size.y &&
            Mathf.Abs(map.Map[cell.X - 1, cell.Z].Y - cell.Y) <= 1) {
            neighbours.Add(map.Map[cell.X - 1, cell.Z]);
        }

        if (cell.X >= 0 && cell.X < map.Size.x &&
            cell.Z + 1 >= 0 && cell.Z + 1 < map.Size.y &&
            Mathf.Abs(map.Map[cell.X, cell.Z + 1].Y - cell.Y) <= 1) {
            neighbours.Add(map.Map[cell.X, cell.Z + 1]);
        }

        if (cell.X >= 0 && cell.X < map.Size.x &&
            cell.Z - 1 >= 0 && cell.Z - 1 < map.Size.y &&
            Mathf.Abs(map.Map[cell.X, cell.Z - 1].Y - cell.Y) <= 1) {
            neighbours.Add(map.Map[cell.X, cell.Z - 1]);
        }

        return neighbours;
    }

    private static void ClearPathfinding(MapInfos map) {

        for (int x = 0; x < map.Size.x; x++)
            for (int y = 0; y < map.Size.y; y++) {
                map.Map[x, y].GCost = 0;
                map.Map[x, y].HCost = 0;
                map.Map[x, y].Parent = null;
            }
    }
}
