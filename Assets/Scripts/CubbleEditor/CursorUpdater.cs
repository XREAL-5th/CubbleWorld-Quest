using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorUpdater : MonoBehaviour {
    private const int MIN_POS = -2, MAX_POS = 2;

    [Header("Settings")]
    [SerializeField] private CubeList cubeList;
    [SerializeField] private LayerMask backgroundLayer, cubeLayer;
    [SerializeField] private Color validColor = Color.yellow, invalidColor = Color.red;

    [Header("Component References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform cubesRoot;
    [SerializeField] private MeshRenderer mrenderer;

    public bool hasTarget = false, validPosition = false;

    private RaycastHit[] hits = new RaycastHit[15]; //we can expect at most 15 items in a straight line
    private Collider[] res = new Collider[1];


    private void UpdateInput() {
        //todo: Place or remove cube at local position
        //the cursor localPosition and localRotation should be used to place the cube, as a child of cubesRoot
        //two public bools (set in the skeleton code) can be used - hasTarget is true when the cursor is over some area inside of the room jar;
        //validPosition is trun when hasTarget is true AND the cursor position is not obstructed.
        //this method is called each frame, whether the cursor is in focus or not. Use !EventSystem.IsPointerOverGameObject to validate UI focus blocking.

        ///////////////////////////////////////////////////////
        //////////////// TODO: WRITE CODE HERE ////////////////
        ///////////////////////////////////////////////////////
    }

    //Below is some skeleton code responsible for raycasting and positioning the cursor before UpdateInput() is called.
    //You may alter some code if you know what you are doing; but do note that the example answer did not modify code below so you should try not modifying the code too.
    #region SKELETON
    //////////////// SKELETON CODE - do not touch ////////////////

    void Start() {
        hasTarget = validPosition = false;
        mrenderer.enabled = false;
    }

    void Update() {
        UpdatePosition();

        mrenderer.enabled = hasTarget;
        if (hasTarget) {
            //check availability
            int n = Physics.OverlapBoxNonAlloc(transform.position, Vector3.one * 0.2f, res, Quaternion.identity, cubeLayer);
            validPosition = n < 1;

            //color cursor
            mrenderer.material.color = validPosition ? validColor : invalidColor;
        }
        else {
            validPosition = false;
        }

        UpdateInput();
    }

    private void UpdatePosition() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        int n = Physics.RaycastNonAlloc(ray, hits, 50, cubeLayer);


        if (n == 0) {
            n = Physics.RaycastNonAlloc(ray, hits, 50, backgroundLayer);
        }

        hasTarget = n > 0;

        if (hasTarget) {
            RaycastHit target = GetMinDistanceHit(n);
            Vector3 pos = target.point + 0.5f * target.normal;
            transform.position = pos;

            if (!InBounds(transform.localPosition)) {
                hasTarget = false;
            }
            else {
                transform.localPosition = new Vector3(ClampPos(transform.localPosition.x), ClampPos(transform.localPosition.y), ClampPos(transform.localPosition.z));
            }
        }
    }

    private RaycastHit GetMinDistanceHit(int n) {
        RaycastHit target = hits[0];
        float dist = (hits[0].transform.position - cam.transform.position).sqrMagnitude;

        for (int i = 1; i < n; i++) {
            float d = (hits[i].transform.position - cam.transform.position).sqrMagnitude;
            if (d < dist) {
                dist = d;
                target = hits[i];
            }
        }

        return target;
    }

    private bool InBounds(Vector3 p) {
        int x = Mathf.RoundToInt(p.x);
        if (x < MIN_POS || x > MAX_POS) return false;
        x = Mathf.RoundToInt(p.y);
        if (x < MIN_POS || x > MAX_POS) return false;
        x = Mathf.RoundToInt(p.z);
        if (x < MIN_POS || x > MAX_POS) return false;
        return true;
    }

    private int ClampPos(float p) {
        return Mathf.Clamp(Mathf.RoundToInt(p), MIN_POS, MAX_POS);
    }

    //////////////// SKELETON CODE - do not touch ////////////////
    #endregion
}
