using UnityEngine;
using System.Collections.Generic;

public class CityMapGenerator : Generator
{
    //Fed in vertex list from Terrain Generator
    //Modify it to look more like a city
    [SerializeField] private TerrainGenerator terrainGenerator;
    private Vector3[] vertices;
    private List<int[]> streets;
    private int xSize;
    private int zSize;

    public override void CreateMesh()
    {
        GetVertices();
        CreateStreets();
    }

    private void GetVertices()
    {
        vertices = terrainGenerator.CreateTerrain();
        xSize = (int) vertices[vertices.Length - 1].x;
        //zSize is encoded in y component. z component is empty
        zSize = (int)vertices[vertices.Length - 1].y;
    }

    private void CreateStreets()
    {
        //Connect every vertex horizonatally as grid first
        for (int i = 0; i < vertices.Length; i++)
        {
            //Makes sure vertex at end of 'row' doesn't connect to start of next row
            if ((i+1) % zSize != 0)
            {
                int[] line = { i, i + 1 };
                streets.Add(line);
            }
        }

        foreach (int[] e in streets)
        {
            print("/n" + e.ToString());
        }
    }
}

