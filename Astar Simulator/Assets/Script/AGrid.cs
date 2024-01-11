using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    ANode[,] grid;

    float nodeDiameter;
    int gridsizeX;
    int gridsizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridsizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridsizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new ANode[gridsizeX, gridsizeY];
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.y / 2);
        Vector3 worldPoint;
        for(int x = 0; x < gridsizeX; x++)
        {
            for(int y = 0; y < gridsizeY; y++)
            {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new ANode(walkable, worldPoint, x, y);
            }
        }
    }

    public List<ANode> GetNeighbours(ANode node)
    {
        List<ANode> neighbours = new List<ANode>();
        for(int x = -1; x <= -1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridsizeX && checkY >= 0 && checkY < gridsizeY)
                {
                    neighbours.Add(grid[x, y]);
                }
            }
        }

        return neighbours;
    }

    public ANode GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridsizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridsizeY - 1) * percentY);

        Debug.Log(string.Format("Find [{0}, {1}]", x, y));

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null)
        {
            foreach (ANode node in grid)
            {
                Gizmos.color = (node.isWalkable ? Color.white : Color.red);
                Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
