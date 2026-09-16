using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ObjLoader : MonoBehaviour
{
    [Header("OBJ文件路径（放在Assets/StreamingAssets文件夹）")]
    public string objFileName = "tree.obj";
    private Mesh loadedMesh;

    void Start()
    {
        LoadObjFromFile(Path.Combine(Application.streamingAssetsPath, objFileName));
    }

    void LoadObjFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("OBJ文件不存在：" + filePath);
            return;
        }

        var lines = File.ReadAllLines(filePath);
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        foreach (var line in lines)
        {
            var parts = line.Trim().Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            if (parts[0] == "v")
            {
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                float z = float.Parse(parts[3]);
                vertices.Add(new Vector3(x, y, z));
            }
            else if (parts[0] == "f")
            {
                List<int> faceIndices = new List<int>();
                // 解析一行f里所有顶点索引
                for (int i = 1; i < parts.Length; i++)
                {
                    var idxStr = parts[i].Split('/')[0];
                    int index = int.Parse(idxStr) - 1;
                    faceIndices.Add(index);
                }
                // 核心：多边形转三角面，n边形 fan三角化
                for (int i = 2; i < faceIndices.Count; i++)
                {
                    triangles.Add(faceIndices[0]);
                    triangles.Add(faceIndices[i - 1]);
                    triangles.Add(faceIndices[i]);
                }
            }
        }

        loadedMesh = new Mesh();
        loadedMesh.vertices = vertices.ToArray();
        loadedMesh.triangles = triangles.ToArray();
        loadedMesh.RecalculateNormals();
        loadedMesh.RecalculateBounds();

        // 平移模型到原点
        Vector3 center = loadedMesh.bounds.center;
        Vector3[] verts = loadedMesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] -= center;
        }
        loadedMesh.vertices = verts;
        loadedMesh.RecalculateBounds();

        GameObject modelObj = new GameObject("LoadedOBJModel");
        modelObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        MeshFilter mf = modelObj.AddComponent<MeshFilter>();
        MeshRenderer mr = modelObj.AddComponent<MeshRenderer>();
        mf.mesh = loadedMesh;

        mr.material = new Material(Shader.Find("Standard"));
        mr.material.SetFloat("_Cull", 0); //双面渲染

        // 打印顶点信息
        Vector3[] vertsOut = loadedMesh.vertices;
        Debug.Log($"模型顶点总数：{vertsOut.Length}");
        for (int i = 0; i < Mathf.Min(5, vertsOut.Length); i++)
        {
            Debug.Log($"顶点{i}: {vertsOut[i]}");
        }
    }

    public Mesh GetMesh()
    {
        return loadedMesh;
    }
}