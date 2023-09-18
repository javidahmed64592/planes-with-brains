using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;
    public float stretch = 50f;
    public float horSmoothing = 0.4f;
    public float horScale = 50f;
    public float vertScale = 50f;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateShape();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    float yScaling(int x, int z)
    {
        return -Mathf.Sin(Mathf.PI * (x + (stretch / 2)) / (xSize + stretch)) * Mathf.Sin(Mathf.PI * (z + (stretch / 2)) / (zSize + stretch)) + 1;
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * horSmoothing, z * horSmoothing) * yScaling(x, z);
                vertices[i] = new Vector3((x - (xSize / 2)) * horScale, y * vertScale, (z - (zSize / 2)) * horScale);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; ++z)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }

            vert++;
        }

    }

    void UpdateShape()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
