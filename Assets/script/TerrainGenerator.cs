using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class TerrainGenerator : MonoBehaviour
{
    private Mesh mesh;
    // vector3 array of vertices
    private Vector3[] vertices;
    private Vector2[] uvs;

    private int[] triangles;

    // range of height for generating height offset
    public float range;
    // use 2^n to calculate the area of the square 
    public int dimension;


    private int terrainWidth;
    // store the maximum height and the minimum height of the terrain
    private float minHeight;
    private float maxHeight;

    public float[,] heightMap;
    private Color[] colourMap;
    

    private float landHeight;
    private float forestHeight;
    private float forestDarkHeight;
    private float mountainHeight;
    private float mountainDarkHeight;
    private float snowHeight;

    public Color landColor;
    public Color forestColorDark;
    public Color forestColor;
    public Color moutainColorDrak;
    public Color moutainColor;
    public Color snowColor;

    // Start is called before the first frame update
    void Start()
    {
        terrainWidth = (int)Mathf.Pow(2, dimension);
        // Create new mesh
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        // Assign created mesh to the object attached to.
        GetComponent<MeshFilter>().mesh = mesh;
        CreateMesh();
        DiamondSquare();
        GenerateTerrain();
        UpdateMesh();
    }

    void Update()
    {   
        //When press the space button, reset the terrain.
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Start();
        }
    }

    void CreateMesh()
    {
        vertices = new Vector3[terrainWidth * terrainWidth];
        uvs = new Vector2[terrainWidth * terrainWidth];
        triangles = new int[(terrainWidth - 1) * (terrainWidth - 1) * 6];
        colourMap = new Color[terrainWidth * terrainWidth];
        // create vertices
        int i = 0;
        for (int z = 0; z < terrainWidth; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        // create triangles
        int vert = 0;
        int tris = 0;
        for (int y = 0; y < terrainWidth - 1; y++)
        {
            for (int x = 0; x < terrainWidth - 1; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + terrainWidth;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + terrainWidth;
                triangles[tris + 5] = vert + terrainWidth + 1;
                uvs[vert] = new Vector2(x / (float)terrainWidth, y / (float)terrainWidth);
                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    // update newly generated mesh to mesh collider
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colourMap;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }


    void GenerateTerrain()
    {
        // declare the criterias of which kind of terrain should each level be
        landHeight = minHeight + (maxHeight - minHeight) * 0.5f;
        forestHeight = minHeight + (maxHeight - minHeight) * 0.65f;
        forestDarkHeight = minHeight + (maxHeight - minHeight) * 0.75f;
        mountainHeight = minHeight + (maxHeight - minHeight) * 0.825f;
        mountainDarkHeight = minHeight + (maxHeight - minHeight) * 0.875f;
        snowHeight = minHeight + (maxHeight - minHeight);


        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainWidth; y++)
            {
                //Map height to each vertex
                float currentHeight = heightMap[x, y];
                vertices[y * terrainWidth + x].y = currentHeight;

                //Assign the colour according to the height of the point
                if (currentHeight < landHeight)
                {
                    colourMap[y * terrainWidth + x] = landColor;
                }
                else if (currentHeight < forestHeight)
                {
                    colourMap[y * terrainWidth + x] = forestColorDark;
                }
                else if (currentHeight < forestDarkHeight)
                {
                    colourMap[y * terrainWidth + x] = forestColor;
                }
                else if (currentHeight < mountainHeight)
                {
                    colourMap[y * terrainWidth + x] = moutainColorDrak;
                }
                else if (currentHeight < mountainDarkHeight)
                {
                    colourMap[y * terrainWidth + x] = moutainColor;
                }
                else if (currentHeight < snowHeight)
                {
                    colourMap[y * terrainWidth + x] = snowColor;
                }

            }
        }
    }

    public void DiamondSquare()
    {
        float range = this.range;
        // use 2D array to store height
        heightMap = new float[terrainWidth + 1, terrainWidth + 1];

        // set seed for random number generator
        UnityEngine.Random.InitState(UnityEngine.Random.Range(0, 1000000));

        int width = terrainWidth - 1;
        int currentWidth = width;
        int xExtend, yExtend;
        int midX, midY;

        // set the initial value of four corners
        heightMap[0, terrainWidth] = 0;
        heightMap[0, 0] = 0;
        heightMap[terrainWidth, 0] = 0;
        heightMap[terrainWidth, terrainWidth] = 0;
        while (currentWidth > 1)
        { 
            //Use diamond algorithm to calculate the centre point of each square
            for (int x = 0; x < width; x += currentWidth)
            {
                for (int y = 0; y < width; y += currentWidth)
                {
                    xExtend = (x + currentWidth);
                    yExtend = (y + currentWidth);
                    midX = (int)(x + currentWidth / 2.0f);
                    midY = (int)(y + currentWidth / 2.0f);

                    float midValue = (heightMap[x, y] + heightMap[xExtend, y] + heightMap[x, yExtend]
                    + heightMap[xExtend, yExtend]) / 4.0f;

                    heightMap[midX, midY] = (float)(midValue + UnityEngine.Random.value * 2.0 * range - 0.9 * range);  
                }
            }

            //Use square algorithm to calculate the mid point of each edge of the squares
            //Calculate the value of each mid point on vertical edaes
            for (int x = 0; x < width; x += currentWidth)
            {
                for (int y = currentWidth / 2; y < width; y += currentWidth)
                {
                    if (x + currentWidth / 2 < width && x - currentWidth / 2 > 0)
                    {
                        heightMap[x, y] = (float)((heightMap[x, y - currentWidth / 2] + heightMap[x, y + currentWidth / 2] + heightMap[x - currentWidth / 2, y] + heightMap[x + currentWidth / 2, y]) / 4.0f + UnityEngine.Random.value * 2.0 * range - 0.9*range);
                    }
                }
            }

            //Calculate the value of each mid point on horizontal edaes

            for (int y = 0; y < width; y += currentWidth)
            {
                for (int x = currentWidth / 2; x < width; x += currentWidth)
                {
                    if (y + currentWidth / 2 < width && y - currentWidth / 2 > 0)
                    {
                        heightMap[x, y] = (float)((heightMap[x - currentWidth/2, y] + heightMap[x + currentWidth/2, y] + heightMap[x, y - currentWidth/2] + heightMap[x, y+currentWidth/2]) / 4.0f + UnityEngine.Random.value * 2.0 * range - 0.9*range);
                    }
                }
            }

            // Half the width to cut current square into 2^n small squares
            currentWidth = (int)((currentWidth / 2.0f) + 0.5);
            // Half the range to make the terrain rise an fall
            range *= 0.5f;
        }

        maxHeight = int.MinValue;
        minHeight = int.MaxValue;

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainWidth; y++)
            {
                if (heightMap[x, y] > maxHeight)
                {
                    maxHeight = heightMap[x, y];
                }

                if (heightMap[x, y] < minHeight)
                {
                    minHeight = heightMap[x, y];
                }
            }
        }

    }


}
