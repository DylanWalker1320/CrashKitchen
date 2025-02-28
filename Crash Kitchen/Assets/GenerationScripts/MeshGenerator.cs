using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;
    public float noiseScale = 2f;
    public float noiseReduction = 0.3f;
    [SerializeField] private string shaderType = "Universal Render Pipeline/Lit";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        ApplyRandomMaterial();
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

    void ApplyRandomMaterial()
    {
        Shader s = Shader.Find(shaderType);
        if (!s)
        {
            print("FUCKING HELL");
        }
        Material randMat = new Material(s);
        randMat.name = $"{nameof(gameObject)}Material";
        randMat.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        randMat.EnableKeyword("_EMISSION");
        randMat.SetColor("_EmissionColor", randMat.color);

        GetComponent<Renderer>().material = randMat;
    }

    void CreateShape()
    {
        // Verts
        vertices = new Vector3[(xSize +1) * (zSize +1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise((x + Random.value) * noiseReduction, (z + Random.value) * noiseReduction) * noiseScale;
                vertices[i] = new Vector3(x, y, z);
                i++;
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
