using UnityEngine;
using UnityEditor;

public class CubeGenerator : Generator
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField, Range(1.0f, 10.0f)] private float width = 5.0f;
    [SerializeField, Range(1.0f, 100.0f)] private float maxHeight = 10.0f;
    [SerializeField, Range(1.0f, 10.0f)] private float depth = 5.0f;
    public Vector3 origin = new Vector3(0, 0, 0);

    [Tooltip("Name of the material, make sure it is listed under -Allways Included Shaders- in Project Settings/Graphics/Shader Settings")]
    [SerializeField] private string shaderType = "Universal Render Pipeline/Lit";

    public override void CreateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        RandomCubeGen();
        UpdateMesh();
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
        Material randMat = new Material(s);
        randMat.name = $"{gameObject.name}Material";
        randMat.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        randMat.EnableKeyword("_EMISSION");
        randMat.SetColor("_EmissionColor", randMat.color);

        GetComponent<Renderer>().material = randMat;
    }

    void RandomCubeGen()
    {
        CreateCube(width, Random.Range(1.0f, maxHeight), depth);
        ApplyRandomMaterial();
    }

    void CreateCube(float w, float h, float d) {
        //width w, height h, depth d
        //Cubes have 8 vertices
        vertices = new Vector3[8];

        vertices[0] = origin;
        vertices[1] = new Vector3(origin.x + w, origin.y, origin.z);
        vertices[2] = new Vector3(origin.x, origin.y + h, origin.z);
        vertices[3] = new Vector3(origin.x + w, origin.y + h, origin.z);
        vertices[4] = new Vector3(origin.x, origin.y, origin.z + d);
        vertices[5] = new Vector3(origin.x + w, origin.y, origin.z + d);
        vertices[6] = new Vector3(origin.x, origin.y + h, origin.z + d);
        vertices[7] = new Vector3(origin.x + w, origin.y + h, origin.z + d);

        //Tri is 3 verts, 2 tri for quad, 6 quad for cube
        //Tri verts have to be counterclockwise for automatic normals
        triangles = new int[36];

        //back
        triangles[0] = 1;
        triangles[1] = 0;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;

        //right
        triangles[6] = 4;
        triangles[7] = 6;
        triangles[8] = 2;
        triangles[9] = 2;
        triangles[10] = 0;
        triangles[11] = 4;

        //left
        triangles[12] = 5;
        triangles[13] = 1;
        triangles[14] = 3;
        triangles[15] = 3;
        triangles[16] = 7;
        triangles[17] = 5;

        //front
        triangles[18] = 5;
        triangles[19] = 7;
        triangles[20] = 6;
        triangles[21] = 6;
        triangles[22] = 4;
        triangles[23] = 5;

        //top
        triangles[24] = 6;
        triangles[25] = 7;
        triangles[26] = 3;
        triangles[27] = 3;
        triangles[28] = 2;
        triangles[29] = 6;

        //bottom - not really the most nessescary for buildings, but doesn't matter
        triangles[30] = 4;
        triangles[31] = 0;
        triangles[32] = 1;
        triangles[33] = 1;
        triangles[34] = 5;
        triangles[35] = 4;
    }
}
