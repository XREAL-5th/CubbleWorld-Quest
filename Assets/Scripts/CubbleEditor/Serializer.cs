using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO> THIS IS AN ANSWER CLASS
public class Serializer : MonoBehaviour {
    private const int BLOCK_TYPE_BITS = 4;
    private const int BLOCK_POS_BITS = 8;

    //singleton
    public static Serializer Main {
        get {
            if (!_loaded) {
                _loaded = true;
                _main = FindObjectOfType<Serializer>();
            }
            return _main;
        }
    }

    private static Serializer _main;
    private static bool _loaded;

    public Transform cubeRoot;
    [SerializeField] private CubeList cubeList;

    private List<CubeData> cubeDataList = new();

    public string Serialize() {
        cubeDataList.Clear();
        foreach (Transform c in cubeRoot.transform) {
            cubeDataList.Add(c.GetComponent<CubeData>());
        }

        int total_size = cubeDataList.Count * (BLOCK_POS_BITS + BLOCK_TYPE_BITS);
        Debug.Log($"Estimated total bits: {total_size}");

        if(total_size == 0) {
            return "";
        }

        byte[] bytes = new byte[total_size / 8 + (total_size % 8 > 1 ? 1 : 0)];

        Debug.Log($"Bytestream length: {bytes.Length}");

        int pointer = 0;
        for (int i = 0; i < cubeDataList.Count; i++) {
            if(i % 2 == 0) {
                //starter
                //IIIIPPPP PPPP____ ________
                byte p = GetPositionIndex(cubeDataList[i]);
                bytes[pointer] = (byte)(cubeDataList[i].id << 4 | (0b00001111 & (p >> 4)));
                bytes[pointer + 1] = (byte)(0b11110000 & (p << 4));
            }
            else {
                //ender
                //________ XXXXIIII PPPPPPPP
                byte p = GetPositionIndex(cubeDataList[i]);
                bytes[pointer + 1] |= (byte)(0b00001111 & cubeDataList[i].id);
                bytes[pointer + 2] = p;
                pointer += 3;
            }
        }

        string o = Convert.ToBase64String(bytes);

        Debug.Log(o);

        return o;
    }

    public void Deserialize(string code) {
        ClearChildren(cubeRoot);

        byte[] bytes;
        try {
            bytes = Convert.FromBase64String(code);
        }
        catch {
            Debug.Log("Invalid base64!");
            return;
        }

        Debug.Log($"Bytestream length: {bytes.Length}");
        int n = bytes.Length / 3 * 2;
        if (bytes.Length % 3 > 0) n++;
        Debug.Log($"Chunks: {n}");

        int pointer = 0;
        for (int i = 0; i < n; i++) {
            if (i % 2 == 0) {
                //starter
                //IIIIPPPP PPPP____ ________
                int id = bytes[pointer] >> 4;
                int pos = ((bytes[pointer] & 0b00001111) << 4) | (bytes[pointer + 1] >> 4);
                AddCube(id, pos);
            }
            else {
                //ender
                //________ XXXXIIII PPPPPPPP
                int id = bytes[pointer + 1] & 0b00001111;
                int pos = bytes[pointer + 2];
                AddCube(id, pos);
                pointer += 3;
            }
        }
    }

    public void ClearRoom() {
        ClearChildren(cubeRoot);
    }

    private byte GetPositionIndex(CubeData cubeData) {
        int x = Mathf.RoundToInt(cubeData.transform.localPosition.x);
        int y = Mathf.RoundToInt(cubeData.transform.localPosition.y);
        int z = Mathf.RoundToInt(cubeData.transform.localPosition.z);

        x += 2; y += 2; z += 2;
        return (byte)(x * 25 + y * 5 + z);
    }

    private Vector3 FromPositionIndex(int p) {
        int z = p % 5;
        int y = (p / 5) % 5;
        int x = p / 25;

        return new Vector3(x - 2, y - 2, z - 2);
    }

    private static void ClearChildren(Transform o) {
        int n = o.childCount;
        if (n <= 0) return;
        for (int i = n - 1; i >= 0; i--) {
            GameObject.Destroy(o.GetChild(i).gameObject);
        }
    }

    private void AddCube(int id, int pos) {
        GameObject o = Instantiate(cubeList.cubes[id], cubeRoot);
        o.transform.localPosition = FromPositionIndex(pos);
        o.transform.localRotation = Quaternion.identity;
        o.AddComponent<CubeData>().id = id;
    }
}
