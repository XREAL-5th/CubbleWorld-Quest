using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeSaveBehavior : MonoBehaviour
{
    private static readonly byte Separator = byte.MaxValue;
    [SerializeField] private CubeList cubeList;
    [SerializeField] private Transform cubesRoot;

    public void OnSaveMapToCopyBuffer()
    {
        var saveData = OnSaveMap();
        GUIUtility.systemCopyBuffer = saveData;
    }
    
    public string OnSaveMap()
    {
        List<byte> byteList = new List<byte>();
        List<byte> typeList = new List<byte>();
        byte tmp = 0x0;
        for (var i = 0;  i < transform.childCount; i++)
        {
            var cube = transform.GetChild(i).gameObject;
            var (pos, type) = CubeToByteTyple(cube);
            byteList.Add(pos);
            if (i % 2 == 0)
            {
                tmp = (byte)(type << 4);
            }
            else
            {
                typeList.Add((byte)(tmp | type));
                tmp = 0x0;
            }
        }
        if (transform.childCount % 2 == 1)
        {
            typeList.Add(tmp);
        }
        // separater
        byteList.Add(Separator);
        byteList.AddRange(typeList);
        return Convert.ToBase64String(byteList.ToArray());
    }

    public void OnLoadMapFromCopyBuffer()
    {
        string saveData = GUIUtility.systemCopyBuffer;
        OnLoadMap(saveData);
    }

    public void OnLoadMap(string saveData)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var cube = transform.GetChild(i).gameObject;
            Destroy(cube);
        }

        byte[] byteArr = Convert.FromBase64String(saveData);

        var separatorIdx = Array.IndexOf(byteArr, Separator);
        byte[] posArr = byteArr[..separatorIdx];
        byte[] typeMergedArr = byteArr[(separatorIdx + 1)..];

        for (var i = 0; i < posArr.Length; i++)
        {
            byte pos = posArr[i];
            int posInt = pos;
            int z = posInt % 5 - 2;
            int y = (posInt / 5) % 5 - 2;
            int x = posInt / 25 - 2;
            var positionVec = new Vector3(x, y, z);
            
            byte mergedType = typeMergedArr[i / 2];
            int type = i % 2 == 0 ? mergedType >> 4 : mergedType % 16;
            
            var cubePrefab = cubeList.cubes[type];
            var cubeObject = Instantiate(cubePrefab);
            cubeObject.transform.SetParent(cubesRoot);
            cubeObject.transform.localPosition = positionVec;
            cubeObject.transform.localRotation = Quaternion.identity;
            var cubeBehavior = cubeObject.AddComponent<CubeBehavior>();
            cubeBehavior.CubeTypeIdx = type;
        }
    }

    private Tuple<byte, byte> CubeToByteTyple(GameObject cube)
    {
        // pos: -2 ~ 2
        var coord = cube.transform.localPosition;
        int positionInt = PosToInt(coord.x) * 25 +
                           PosToInt(coord.y) * 5 +
                           PosToInt(coord.z);
        var cubeType = cube.GetComponent<CubeBehavior>().CubeTypeIdx;
        return new Tuple<byte, byte>((byte)positionInt, (byte)cubeType);
    }

    private int PosToInt(double d)
    {
        return (int)d + 2;
    }
}
