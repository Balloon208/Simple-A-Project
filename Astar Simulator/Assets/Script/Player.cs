using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera mainCamera;

    int layerMask;

    public Unit selectedUnit;

    // Start is called before the first frame update

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        layerMask = 1 << LayerMask.NameToLayer("Unit");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 10000f, layerMask))
            {
                if (selectedUnit != null)
                {
                    selectedUnit.transform.GetComponent<MeshRenderer>().material = selectedUnit.mat[0];
                }
                selectedUnit = hit.transform.GetComponent<Unit>();
                selectedUnit.transform.GetComponent<MeshRenderer>().material = selectedUnit.mat[1];
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if(selectedUnit != null)
            {
                selectedUnit.Move();
            }
        }
    }
}
