using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO> THIS IS AN ANSWER CLASS
public class DeepLinkManager : MonoBehaviour {
    public const string LINKNAME = "cubble";

    public static DeepLinkManager Main { get; private set; }

    private void Awake() {
        if (Main == null) {
            Main = this;
            Screen.fullScreen = false;
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL)) {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                OnDeepLinkActivated(Application.absoluteURL);
            }
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnDeepLinkActivated(string url) {
        Debug.Log($"DEEPLINK URL: {url}");
        // cubble://open?<base64>
        var s = url.Substring(9).Split('?');
        string key = s[0];

        Debug.Log($"s: {s} key: {key}");

        if (key.StartsWith("open")) {
            string code = s[1];
            Serializer.Main.Deserialize(code);
        }
    }

    public static string GetOpenURL(string code) {
        return $"{LINKNAME}://open?{code}";
    }
}