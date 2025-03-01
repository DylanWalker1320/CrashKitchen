using UnityEngine;
using UnityEditor;

public abstract class Generator : MonoBehaviour
{
    public void Generate()
    {
        CreateMesh();
    }

    public abstract void CreateMesh();
}

[CustomEditor(typeof(Generator), true)]
public class Generate : Editor
{
    Generator generator;
    private void Awake()
    {
        generator = (Generator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
    }
}
