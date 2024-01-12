using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANode
{
    public bool isWalkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;

    public double gCost;
    public double hCost;
    public double fCost;
    public ANode parentNode;

    public ANode(bool nWalkable, Vector3 nWorldPos, int nGridX, int nGridY)
    {
        isWalkable = nWalkable;
        worldPos = nWorldPos;
        gridX = nGridX;
        gridY = nGridY;
    }
}
