using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoomRotator : MonoBehaviour {
    [Header("Input Settings")]
    [SerializeField] private float panSensitivity = 1f;
    [SerializeField] private float restrictRotationThresh = 15f;

    private bool panning, resettingRotation;
    private bool freeRotation;
    private Vector3 lastMousePos, startMousePos;
    private Vector3 unrestrictedRot;

    void Start() {
        panning = false;
    }

    void Update() {
        if (panning) {
            if (Input.GetMouseButton(2)) {
                Vector3 delta = Input.mousePosition - lastMousePos;
                transform.Rotate(new Vector3(panSensitivity * delta.y, -panSensitivity * delta.x, 0), Space.World);
                unrestrictedRot = transform.rotation.eulerAngles;

                if(!freeRotation) {
                    //check if restriction is valid
                    if (Mathf.Abs(Input.mousePosition.y - startMousePos.y) > restrictRotationThresh && Mathf.Abs(Input.mousePosition.y - startMousePos.y) > Mathf.Abs(Input.mousePosition.x - startMousePos.x)) {
                        freeRotation = true;
                    }
                    else {
                        //restrict XZ rotation if necessary
                        transform.rotation = Quaternion.Euler(0, unrestrictedRot.y, 0);
                    }
                }

                lastMousePos = Input.mousePosition;
            }
            else {
                panning = false;
            }
        }
        else if (Input.GetMouseButtonDown(2) && CanPanStart()) {
            if (resettingRotation) {
                StopAllCoroutines();
                resettingRotation = false;
            }
            panning = true;
            lastMousePos = startMousePos = Input.mousePosition;
        }
        else {
            if (Input.GetKeyDown(KeyCode.Space)) {
                resettingRotation = true;
                StartCoroutine(IResetRotation(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0)));
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                resettingRotation = true;
                StartCoroutine(IResetRotation(Quaternion.identity));
            }
        }
    }

    IEnumerator IResetRotation(Quaternion resetTo) {
        float f = 0;
        while(f < 1f) {
            f += Time.deltaTime * 2f;
            transform.rotation = Quaternion.Lerp(transform.rotation, resetTo, f);
            yield return null;
        }
        transform.rotation = resetTo;
        resettingRotation = freeRotation = false;
    }

    private bool CanPanStart() {
        return !EventSystem.current.IsPointerOverGameObject();
    }
}
