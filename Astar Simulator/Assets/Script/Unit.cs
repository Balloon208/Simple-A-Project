using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // public Transform target;
    private Camera mainCamera;

    float speed = 20;
    Vector3[] path;
    int targetIndex;

    public Material[] mat = new Material[2];

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        targetIndex = 0;
        while(true)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed*Time.deltaTime);
            yield return null;
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        // PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        //if (Input.GetMouseButtonDown(1))
        {
            // 마우스로 찍은 위치의 좌표 값을 가져온다
            if (Physics.Raycast(ray, out hit, 10000f))
            {
                StopAllCoroutines();
                PathRequestManager.RequestPath(transform.position, hit.point, OnPathFound);
            }
        }
    }
}
