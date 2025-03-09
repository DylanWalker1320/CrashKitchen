using UnityEngine;
using System.Collections.Generic;

public class CityMapGenerator : Generator
{
    //Fed in vertex list from Terrain Generator
    //Modify it to look more like a city
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private CubeGenerator cubeGenerator;
    private Vector3[] vertices;
    private List<int[]> blocks;
    private int xSize;
    private int zSize;
    private float streetWidth;
    private float buildingsPerSide;

    public override void CreateMesh()
    {
        CreateBlocks();
    }

    private void GetVertices()
    {
        vertices = terrainGenerator.CreateTerrain();
        xSize = (int) vertices[vertices.Length - 1].x;
        //zSize is encoded in y component. z component is empty
        zSize = (int)vertices[vertices.Length - 1].y;
    }

    private List<int[]> CreateBlocks()
    {
        GetVertices();
        //Add current vert, vert+1, vert+row, vert+row+1
        //go until size-row (secondlast row)
        for (int i = 0; i < vertices.Length; i++)
        {
            //Counter-clockwise
            blocks.Add(new int[] {i, i + xSize , i + xSize + 1 , i + 1 });
        }
        return blocks;
    }

    private void CreateCity()
    {
        //for every block (array in List)
        //basically gotta fetch the coordinates
        Vector3 lowerLeft;
        Vector3 upperRight;
        foreach (var verts in blocks)
        {
            lowerLeft = vertices[verts[0]];
            upperRight = vertices[verts[2]];

            //Buildings generate inside, streets as a border
            lowerLeft.x += streetWidth;
            lowerLeft.z += streetWidth;
            upperRight.x -= streetWidth;
            upperRight.z -= streetWidth;
        }
    }

    private void GenerateBlock(float ll, float ur)
    {
        //will need a double for loop

    }

}

