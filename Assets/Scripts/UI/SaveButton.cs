using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO> THIS IS AN ANSWER CLASS
public class SaveButton : MonoBehaviour {
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    private void Clicked() {
        string s = Serializer.Main.Serialize();
        GUIUtility.systemCopyBuffer = s;
        Debug.Log("Copied to clipboard!");
    }
}
