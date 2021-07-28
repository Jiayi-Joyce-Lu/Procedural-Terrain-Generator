// The mesh part has referenced from 
// https://docs.unity3d.com/ScriptReference/Mesh.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class WaterGenerator : MonoBehaviour
{
    private Mesh mesh;
    // vector3 array of vertices
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;

    // 2^n
    public int dimension;
    public float SeaLevel;
    // Update is called once per frame
    void Update()
    {
        this.transform.localPosition = new Vector3(0.0f, SeaLevel, 0.0f);
    }

    // number of vertices
    private int size;
    void Start()
    {
        size = (int)Mathf.Pow(2, dimension);
        // Create new mesh
        mesh = new Mesh();
        // Assign created mesh to the object attached to.
        GetComponent<MeshFilter>().mesh = mesh;
        CreateMesh();
        UpdateMesh();
    }

    // Create mesh by assigning vertices and triangles
    void CreateMesh()
    {
        vertices = new Vector3[size * size];
        uvs = new Vector2[size * size];
        triangles = new int[(size - 1) * (size - 1) * 6];
        // assign vertices
        int i = 0;
        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        // assign triangles
        int vert = 0;
        int tris = 0;
        for (int y = 0; y < size - 1; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size;
                triangles[tris + 5] = vert + size + 1;
                uvs[vert] = new Vector2(x / (float)size, y / (float)size);
                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        // Call Clear to start fresh
        mesh.Clear();
        // Assign new vertices and triangles
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        // Recalculates the normals of the Mesh from the triangles and vertices
        mesh.RecalculateNormals();

    }
}