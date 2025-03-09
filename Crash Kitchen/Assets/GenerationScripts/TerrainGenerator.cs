using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : Generator
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [Tooltip("Number of streets along the x-axis (#vericies on x-axis)")]
    [SerializeField] private int xSize = 10;

    [Tooltip("Number of streets along the y-axis (#vericies on y-axis)")]
    [SerializeField] private int zSize = 10;

    [Tooltip("Distance from one street to next parallel street")]
    [SerializeField] private int spacing = 1;

    [Tooltip("[0-1] Scale of Perlin Noise")]
    [SerializeField, Range(0f, 1f)] private float perlinNoiseScale = 0.07f;

    [Tooltip("Factor to multiply the height of each vertex")]
    [SerializeField] private float yAmplification = 4f;

    public override void CreateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateTerrain();
        UpdateMesh();
        ClearMesh();
    }

    public Vector3[] GenerateTerrainMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateTerrain();
        UpdateMesh();
        return vertices;
    }

    public void ClearMesh()
    {
        if (mesh)
        {
            mesh.Clear();
        }
    }

    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.Optimize();
    }

    public void CreateTerrain()
    {
        // Verts (1 additional for the vertex grid format)
        vertices = new Vector3[(xSize +1) * (zSize +1) +1];

        for (int i = 0, z = 0; z <= zSize * spacing; z += spacing)
        {
            for (int x = 0; x <= xSize * spacing; x += spacing, i++)
            {
                float y = Mathf.PerlinNoise((x + Random.value) * perlinNoiseScale, (z + Random.value) * perlinNoiseScale) * yAmplification;
                vertices[i] = new Vector3(x, y, z);
            }
        }
        // Last vertex encodes information of the grid size. 0 means nothing
        vertices[vertices.Length - 1] = new Vector3(xSize, zSize, 0);

        // Primitive Triangles (each grid square has 2 triangles, 6 points)
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tri = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++, vert++, tri += 6)
            {
                triangles[tri] = vert + 0;
                triangles[tri + 1] = vert + xSize + 1;
                triangles[tri + 2] = vert + 1;
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + xSize + 1;
                triangles[tri + 5] = vert + xSize + 2;
            }
            vert++;
        }
    }
}
