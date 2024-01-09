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

    private void Start()
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
                grid[x, y] = new ANode(walkable, worldPoint);
            }
        }
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
