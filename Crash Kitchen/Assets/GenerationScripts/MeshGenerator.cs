using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;
    [SerializeField] private float yAmplification = 4f;
    [SerializeField] private float perlinNoiseScale = 0.07f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateTerrain();
        UpdateMesh();
    }

    private void Update()
    {

    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.Optimize();
    }

    void CreateTerrain()
    {
        // Verts
        vertices = new Vector3[(xSize +1) * (zSize +1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float y = Mathf.PerlinNoise((x + Random.value) * perlinNoiseScale, (z + Random.value) * perlinNoiseScale) * yAmplification;
                vertices[i] = new Vector3(x, y, z);
            }
        }

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
