using Godot;
using System;
using System.Linq;

public class AstarPath : TileMap
{
    private AStar2D aStar;
    private Godot.Collections.Array usedCells;
    private Godot.Vector2[] path;
    private Vector2 pathStartPosition;
    private Vector2 pathEndPosition;

    public override void _Ready()
    {
        aStar = new AStar2D();
        usedCells = GetUsedCells();
        _AddPoints();
        _ConnectPoints();
    }

    private void _AddPoints()
    {
        foreach (Vector2 cell in usedCells)
        {
            aStar.AddPoint(id(cell), cell);
        }
    }
    private void _ConnectPoints()
    {
        foreach (Vector2 cell in usedCells)
        {
            Vector2[] closeTiles = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
            foreach (Vector2 tile in closeTiles)
            {
                Vector2 nextTile = cell + tile;
                if (usedCells.Contains(nextTile))
                {
                    aStar.ConnectPoints(id(cell), id(nextTile), false);
                }
            }
        }
    }

    private void _SetPathStartPosition(Vector2 value)
    {
        /* For Debug purposes
        SetCell((int)pathStartPosition.x, (int)pathStartPosition.y, -1);
        SetCell((int)value.x, (int)value.y, 1);
        */
        pathStartPosition = value;
        if (pathEndPosition != null && pathEndPosition != pathStartPosition)
        {
            _GetPath();
        }
    }

    public void SetPathEndPosition(Vector2 value)
    {
        /* For Debug purposes
        SetCell((int)pathStartPosition.x, (int)pathStartPosition.y, -1);
        SetCell((int)value.x, (int)value.y, 1);
        */
        pathEndPosition = value;
        if (pathStartPosition != null)
        {
            _GetPath();
        }
    }

    public void _GetPath()
    {
        path = aStar.GetPointPath(id(pathStartPosition), id(pathEndPosition));
        path = path.Skip(1).ToArray();  // Not needed, delete if necessary.
    }

    // Cantor's pairing function to generate unique id's
    private int id(Vector2 point)
    {
        float x = point.x;
        float y = point.y;
        return (int)((x + y) * (x + y + 1) / 2 + y);
    }
}