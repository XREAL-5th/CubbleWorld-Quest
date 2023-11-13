using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShareBehavior : MonoBehaviour
{
    [SerializeField] private CubeSaveBehavior cubeSaveBehavior;

    public static ShareBehavior Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Screen.fullScreen = false;
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDeepLinkActivated(string url)
    {
        string[] urlAndQuery = url.Split('?');
        if(urlAndQuery.Length == 1)
        {
            return;
        }

        cubeSaveBehavior.OnLoadMap(urlAndQuery[1]);

        Screen.fullScreen = false;
    }

    public void CopyShareUrl()
    {
        var saveData = cubeSaveBehavior.OnSaveMap();
        GUIUtility.systemCopyBuffer = $"cubble://world?{saveData}";
    }
}