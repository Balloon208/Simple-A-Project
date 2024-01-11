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
        return Mathf.Sqrt(Mathf.Pow(node1.gridX - node2.gridX, 2) + Mathf.Pow(node1.gridY - node2.gridY, 2));
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

        List<ANode> openList = new List<ANode>();
        HashSet<ANode> closedList = new HashSet<ANode>();
        openList.Add(startNode);

        while(openList.Count > 0)
        {
            ANode currentNode = openList[0];

            for(int i = 1; i < openList.Count; i++)
            {
                // f 1순위, h 2순위
                if (currentNode.fCost > openList[i].fCost || (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            Debug.Log(string.Format("{0}, {1}", currentNode.gridX, currentNode.gridY));
            Debug.Log(string.Format("{0}, {1}", targetNode.gridX, targetNode.gridY));

            if (currentNode == targetNode)
            {
                TracePath(startNode, targetNode);
                break;
            }
            
            foreach (ANode node in grid.GetNeighbours(currentNode))
            {
                
                if (!node.isWalkable || closedList.Contains(node))
                {
                    continue;
                }

                double ng = node.gCost + 1.0f;
                double nh = GetDistanceCost(node, targetNode);
                double nf = ng + nh;

                if(node.gCost > nf || !openList.Contains(node)) // 기존 C++ 의 if(cellDetails[ny][nx].f == INF || cellDetails[ny][nx].f > nf) 부분
                {
                    node.gCost = ng;
                    node.hCost = nh;
                    node.fCost = nf;
                    node.parentNode = currentNode;

                    if(!openList.Contains(node))
                    {
                        openList.Add(node);
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
