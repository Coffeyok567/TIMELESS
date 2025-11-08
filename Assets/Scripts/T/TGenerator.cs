using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.XR;

//[ExecuteInEditMode]
public class TGenerator : MonoBehaviour
{
    [SerializeField] int chunkSize;
    [SerializeField] Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        //кусок от ГПТ
        meshCollider.sharedMesh = GetComponent<MeshFilter>().mesh; // Сначала назначить меш
        meshCollider.convex = true; // Затем установить convex
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Generate()
    {
        //обьявляю меш
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "test";
        
        //задаю вершины и передаю их
        vertices = new Vector3[(chunkSize) * (chunkSize)];
        Vector2[] uv = new Vector2[vertices.Length];
        //for(int y = 0; y < chunkSize; y++)
        //{
            for (int i = 0, z = 0; z < chunkSize; z++)
            {
                for (int x = 0; x < chunkSize; x++, i++)
                {
                    vertices[i] = new Vector3(x, 0, z); //TODO: рандомить Y
                    uv[i] = new Vector2((float)x / chunkSize, (float)z / chunkSize);
                }
            }
        //}
        mesh.vertices = vertices;
        mesh.uv = uv;

        //задаю массив треугольников по определенному порядку для получения квадов и отдаю мешу
        int[] triangles = new int[6 * (chunkSize) * (chunkSize)];
        
        for (int vi = 0, ti = 0, z = 0; z < chunkSize - 1; z++, vi++)
        {
            for(int x = 0; x < chunkSize - 1; x++, vi++, ti += 6)
            {
                triangles[ti + 0] = vi;
                triangles[ti + 1] = triangles[ti + 3] = chunkSize + vi;
                triangles[ti + 2] = triangles[ti + 5] = vi + 1;
                triangles[ti + 4] = chunkSize + vi + 1;
                Debug.Log(vi + " " + ti + " " + (6 * chunkSize * chunkSize));
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    //дебаг отрисовка
    private void OnDrawGizmos()
    {
        //счетчик индексов (гпт не нашел как их воровать из форич)
        int indexCount = 0;

        Gizmos.color = Color.red;
        if (vertices == null)
        {
            return;
        }
        foreach(Vector3 giz in vertices)
        {
            Gizmos.DrawSphere(giz, 0.05f);
            //Handles.Label(giz, indexCount.ToString());
            indexCount++;
        }
    }
}
