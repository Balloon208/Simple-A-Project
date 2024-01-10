using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    AGrid grid;

    public Transform startObject;
    public Transform targetObject;

    private void Awake()
    {
        grid = GetComponent<AGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        FindPath(startObject.position, targetObject.position);
    }

    double GetDistanceCost(ANode node1, ANode node2)
    {
        return Mathf.Sqrt(Mathf.Pow(node1.gridX - node2.gridX, 2) + Mathf.Pow(node1.gridY - node2.gridY, 2));
    }

    void TracePath(ANode startNode, ANode endNode)
    {
        List <ANode> path = new List <ANode>();
        ANode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }

        path.Reverse();
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
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

            if(currentNode == targetNode)
            {
                TracePath(startNode, targetNode);
                return;
            }

            foreach(ANode node in grid.GetNeighbours(currentNode))
            {
                if(!node.isWalkable || closedList.Contains(node))
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
    }
}
