using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateNearestNeighbor : Editor
{
    [MenuItem("Generate/Generate Nearest Neighbors")]
    static void GenerateNearestNeighborFile()
    {
        string nearestNeighborString = GetNearestNeighborsString();

        System.IO.File.WriteAllText(@"D:\GitHub\GMTKGameJam2019\Assets\Resources\NearestNeighborData.txt", nearestNeighborString);
        Debug.Log("Generated File...");
    }

    static string GetNearestNeighborsString()
    {
        GameObject obj = Resources.Load<GameObject>("WallMesh");
        Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

        SmartVertex[] smartVertices = new SmartVertex[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            SmartVertex vertex = new SmartVertex();
            vertex.thisVertexIndex = i;
            smartVertices[i] = vertex;
        }

        string s = JsonHelper.ToJson(smartVertices);
        Debug.Log(s);

        return s;
    }

    static string GetClosestVertices(Mesh mesh, int vertex, int limit)
    {
        List<int> list = new List<int>();

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            if (i != vertex)
            {
                list.Add(i);
            }
        }
        return "";
    }
}

[System.Serializable]
public class SmartVertex
{
    public int thisVertexIndex;
    public SmartVertex[] nearestVertices;
}

public class JsonHelper
{

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
