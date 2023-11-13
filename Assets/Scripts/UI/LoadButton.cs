using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO> THIS IS AN ANSWER CLASS
public class LoadButton : MonoBehaviour {
    [SerializeField] private Button loadConfirmButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private GameObject inputFrame;
    [SerializeField] private GameObject room;

    private bool inputShown;

    void Start() {
        GetComponent<Button>().onClick.AddListener(Clicked);
        loadConfirmButton.onClick.AddListener(Confirmed);

        inputShown = false;
        inputFrame.SetActive(false);
    }

    private void Clicked() {
        if (!inputShown) {
            inputFrame.SetActive(true);
            inputField.text = "";
        }
    }

    private void Confirmed() {
        string s = inputField.text;

        inputFrame.SetActive(false);
        inputShown = false;

        Serializer.Main.Deserialize(s);
        room.transform.rotation = Quaternion.identity;
    }
}
