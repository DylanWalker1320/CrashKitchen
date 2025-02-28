using UnityEngine;

public class CubeMeshGen : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField] private int xSize = 0;
    [SerializeField] private int ySize = 0;
    [SerializeField] private int zSize = 0;

    [SerializeField] private string shaderType = "Universal Render Pipeline/Lit";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyRandomMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
