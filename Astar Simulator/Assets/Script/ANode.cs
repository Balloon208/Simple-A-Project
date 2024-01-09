using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANode : MonoBehaviour
{
    public bool isWalkable;
    public Vector3 worldPos;

    public ANode(bool nWalkable, Vector3 nWorldPos)
    {
        isWalkable = nWalkable;
        worldPos = nWorldPos;
    }
}
