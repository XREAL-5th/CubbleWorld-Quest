using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO> THIS IS AN ANSWER CLASS
public class ShareButton : MonoBehaviour {
    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
    }

    private void Clicked() {
        string s = Serializer.Main.Serialize();
        GUIUtility.systemCopyBuffer = DeepLinkManager.GetOpenURL(s);
        Debug.Log("Copied link to clipboard!");
    }
}
