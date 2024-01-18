using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    PathRequestManager requestManager;
    AGrid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<AGrid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos)
    {
        Debug.Log(startPos);
        Debug.Log(endPos);
        StartCoroutine(FindPath(startPos, endPos));
    }

    double GetDistanceCost(ANode node1, ANode node2)
    {
        int distX = Mathf.Abs(node1.gridX - node2.gridX);
        int distY = Mathf.Abs(node1.gridY - node2.gridY);

        if(distX > distY)
            return 1.414 * distY + 1.0 * (distX - distY);
        return 1.414 * distX + 1.0 * (distY - distX);
    }

    Vector3[] SimplifyPath(List<ANode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    Vector3[] TracePath(ANode startNode, ANode endNode)
    {
        List <ANode> path = new List<ANode>();
        ANode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        ANode startNode = grid.GetNodeFromWorldPoint(startPos);
        ANode targetNode = grid.GetNodeFromWorldPoint(targetPos);


        if (startNode.isWalkable && targetNode.isWalkable)
        {
            List<ANode> openList = new List<ANode>();
            HashSet<ANode> closedList = new HashSet<ANode>();
            openList.Add(startNode);
        
            while (openList.Count > 0)
            {
                ANode currentNode = openList[0];

                /*
                for (int i = 1; i < openList.Count; i++)
                {
                    // f 1순위, h 2순위
                    if (currentNode.fCost > openList[i].fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                    {
                        currentNode = openList[i];
                    }
                }
                */


                openList.Remove(currentNode);
                closedList.Add(currentNode);;

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (ANode node in grid.GetNeighbours(currentNode))
                {
                    if (!node.isWalkable || closedList.Contains(node))
                    {
                        continue;
                    }

                    double newCurrentToNeighbourCost = currentNode.gCost + GetDistanceCost(currentNode, node);


                    if (newCurrentToNeighbourCost < node.gCost || !openList.Contains(node)) // 기존 C++ 의 if(cellDetails[ny][nx].f == INF || cellDetails[ny][nx].f > nf) 부분
                    {
                        node.gCost = newCurrentToNeighbourCost;
                        node.hCost = GetDistanceCost(node, targetNode);
                        node.parentNode = currentNode;

                        if (!openList.Contains(node))
                        {
                            openList.Add(node);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess)
        {
            waypoints = TracePath(startNode, targetNode);
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }
}
