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
    [SerializeField] private float streetWidth;
    [SerializeField] private int buildingsPerSide;

    public override void CreateMesh()
    {
        terrainGenerator.ClearMesh();
        cubeGenerator.ClearMesh();
        CreateBlocks();
        CreateCity();
    }

    private void GetVertices()
    {
        vertices = terrainGenerator.GenerateTerrainMesh();
        xSize = (int) vertices[vertices.Length - 1].x;
        //zSize is encoded in y component. z component is empty
        //zSize = (int)vertices[vertices.Length - 1].y;
    }

    private List<int[]> CreateBlocks()
    {
        GetVertices();
        //Add current vert, vert+1, vert+row, vert+row+1
        //go until size-row (secondlast row)
        blocks = new List<int[]>();
        for (int i = 0; i < (vertices.Length-1)-xSize; i++)
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

            GenerateBlock(lowerLeft, upperRight);
        }
    }

    private void GenerateBlock(Vector3 ll, Vector3 ur)
    {
        //width and depth of all the buildings in the block
        float building_w = (ur.x - ll.x) / buildingsPerSide;
        float building_d = (ur.z - ll.z) / buildingsPerSide;

        for (int w = 0; w < buildingsPerSide; w++)
        {
            for (int d = 0; d < buildingsPerSide; d++)
            {
                //Look at my old code to see how rooms placed.
                cubeGenerator.GenerateCubeMesh(new Vector3(ll.x + building_w, ll.y, ll.z + building_d));
            }
        }
    }

}

