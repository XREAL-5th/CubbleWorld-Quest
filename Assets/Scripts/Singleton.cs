using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    
    public static T Instance
    {
        get {
            if (_instance == null)
            {
                GameObject newInstance = new(typeof(T).Name);
                _instance = newInstance.AddComponent<T>();
            }

            return _instance;
        }
    }

    public GameObject SelectedCube { get; set; }
}
