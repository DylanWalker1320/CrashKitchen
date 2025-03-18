using UnityEngine;
using System.Collections.Generic;

public class CityMapGenerator : Generator
{
    //Fed in vertex list from Terrain Generator
    //Modify it to look more like a city
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private CubeGenerator cubeGenerator;
    private Mesh mesh;
    private Vector3[] vertices;
    private List<int[]> blocks;
    private List<Vector3[]> verts;
    private Vector3[] verts_flat_array;
    private int[] tris_flat_array;
    private List<int[]> tris;
    private int xSize;
    [SerializeField] private float streetWidth;
    [SerializeField] private int buildingsPerSide;

    public override void CreateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        terrainGenerator.ClearMesh();
        cubeGenerator.ClearMesh();
        CreateBlocks();
        CreateCity();

        mesh.vertices = verts_flat_array;
        mesh.triangles = tris_flat_array;
        foreach (var item in verts_flat_array)
        {
            print(item.ToString());
        }
        mesh.RecalculateNormals();
        mesh.Optimize();
        if (mesh)
        {
            mesh.Clear();
        }
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
            upperRight.x += streetWidth;
            upperRight.z += streetWidth;

            GenerateBlock(lowerLeft, upperRight);
        }
    }

    private void GenerateBlock(Vector3 ll, Vector3 ur)
    {
        //width and depth of all the buildings in the block
        float building_w = (ur.x - ll.x) / buildingsPerSide;
        float building_d = (ll.z - ur.z) / buildingsPerSide;
        verts = new List<Vector3[]>();
        tris = new List<int[]>();
        for (int w = 0; w < buildingsPerSide; w++)
        {
            for (int d = 0; d < buildingsPerSide; d++)
            {
                (Vector3[], int[]) b = cubeGenerator.GenerateCubeMesh(new Vector3(building_w, 20, building_d), new Vector3(ll.x + building_w * w, 0.0f, ll.z + building_d * d));
                verts.Add(b.Item1);
                tris.Add(b.Item2);
            }
        }
        // 8 verticies per cube mesh
        verts_flat_array = new Vector3[verts.Count * 8];
        int count = 0;
        for (int i = 0; i < verts.Count; i++)
        {
            for (int j = 0; j < verts[i].Length; j++)
            {
                verts_flat_array[count] = verts[i][j];
                count++;
            }
        }

        // 36 triangle points per cube mesh (3 per triangle, 12 triangles)
        tris_flat_array = new int[tris.Count * 36];
        count = 0;
        for (int i = 0; i < tris.Count; i++)
        {
            for (int j = 0; j < tris[i].Length; j++)
            {
                tris_flat_array[count] = tris[i][j]+ i * 8;
                count++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < verts_flat_array.Length; i++)
        {
            Gizmos.DrawSphere(verts_flat_array[i], 0.1f);
        }
    }

}

