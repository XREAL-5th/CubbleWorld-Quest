using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorUpdater : MonoBehaviour
{
    private const int MIN_POS = -2, MAX_POS = 2;

    [Header("Settings")] [SerializeField] private CubeList cubeList;
    [SerializeField] private LayerMask backgroundLayer, cubeLayer;
    [SerializeField] private Color validColor = Color.yellow, invalidColor = Color.red;

    [Header("Component References")] [SerializeField]
    private Camera cam;

    [SerializeField] private Transform cubesRoot;
    [SerializeField] private MeshRenderer mrenderer;

    public bool hasTarget = false, validPosition = false;

    private RaycastHit[] hits = new RaycastHit[15]; //we can expect at most 15 items in a straight line
    private Collider[] res = new Collider[1];


    private void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlock();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeleteBlock();
        }
    }

    private void PlaceBlock()
    {
        if (!hasTarget) return;
        
        // TODO: select cube
        // var cubePrefab = cubeList.cubes[0];
        var selectedCube = CubeManager.Instance.SelectedCube;
        var cubePrefab = selectedCube == null ? cubeList.cubes[0] : selectedCube;
        var cubeGameObject = Instantiate(cubePrefab, transform.position, transform.rotation);
        cubeGameObject.transform.SetParent(cubesRoot);
    }

    private void DeleteBlock()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int n = Physics.RaycastNonAlloc(ray, hits, 50, cubeLayer);
        if (n <= 0)
        {
            return;
        }

        var target = GetMinDistanceHit(n);
        var cube = target.collider.gameObject;
        Destroy(cube);
    }

    //Below is some skeleton code responsible for raycasting and positioning the cursor before UpdateInput() is called.
    //You may alter some code if you know what you are doing; but do note that the example answer did not modify code below so you should try not modifying the code too.

    #region SKELETON

    //////////////// SKELETON CODE - do not touch ////////////////

    void Start()
    {
        hasTarget = validPosition = false;
        mrenderer.enabled = false;
    }

    void Update()
    {
        UpdatePosition();

        mrenderer.enabled = hasTarget;
        if (hasTarget)
        {
            //check availability
            int n = Physics.OverlapBoxNonAlloc(transform.position, Vector3.one * 0.2f, res, Quaternion.identity,
                cubeLayer);
            validPosition = n < 1;

            //color cursor
            mrenderer.material.color = validPosition ? validColor : invalidColor;
        }
        else
        {
            validPosition = false;
        }

        UpdateInput();
    }

    private void UpdatePosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int n = Physics.RaycastNonAlloc(ray, hits, 50, cubeLayer);


        if (n == 0)
        {
            n = Physics.RaycastNonAlloc(ray, hits, 50, backgroundLayer);
        }

        hasTarget = n > 0;

        if (hasTarget)
        {
            RaycastHit target = GetMinDistanceHit(n);
            Vector3 pos = target.point + 0.5f * target.normal;
            transform.position = pos;

            if (!InBounds(transform.localPosition))
            {
                hasTarget = false;
            }
            else
            {
                transform.localPosition = new Vector3(ClampPos(transform.localPosition.x),
                    ClampPos(transform.localPosition.y), ClampPos(transform.localPosition.z));
            }
        }
    }

    private RaycastHit GetMinDistanceHit(int n)
    {
        RaycastHit target = hits[0];
        float dist = (hits[0].transform.position - cam.transform.position).sqrMagnitude;

        for (int i = 1; i < n; i++)
        {
            float d = (hits[i].transform.position - cam.transform.position).sqrMagnitude;
            if (d < dist)
            {
                dist = d;
                target = hits[i];
            }
        }

        return target;
    }

    private bool InBounds(Vector3 p)
    {
        int x = Mathf.RoundToInt(p.x);
        if (x < MIN_POS || x > MAX_POS) return false;
        x = Mathf.RoundToInt(p.y);
        if (x < MIN_POS || x > MAX_POS) return false;
        x = Mathf.RoundToInt(p.z);
        if (x < MIN_POS || x > MAX_POS) return false;
        return true;
    }

    private int ClampPos(float p)
    {
        return Mathf.Clamp(Mathf.RoundToInt(p), MIN_POS, MAX_POS);
    }

    //////////////// SKELETON CODE - do not touch ////////////////

    #endregion
}