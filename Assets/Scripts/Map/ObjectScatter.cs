using UnityEngine;

public class ObjectScatter : MonoBehaviour
{
    [SerializeField] int numObjects = 200;
    [SerializeField] float xMax;
    float xMin { get { return -xMax; } }
    [SerializeField] float xInner;
    [SerializeField] float zMax;
    float zMin { get { return -zMax; } }
    [SerializeField] float zInner;
    [SerializeField] float vertScale;


    [SerializeField] GameObject[] objects;
    [SerializeField] float[] objectScale = new float[2] { 0.8f, 1.2f };
    float randomScale { get { return Random.Range(objectScale[0], objectScale[1]); } }

    private void Start()
    {
        for (int _ = 0; _ < numObjects; _++)
        {
            float randX = Random.Range(xMin, xMax);
            float randZ = Random.Range(zMin, zMax);
            bool withinInnerBox = Mathf.Abs(randX) <= xInner && Mathf.Abs(randZ) <= zInner;
            if (!withinInnerBox)
            {
                GameObject obj = Instantiate(objects[Random.Range(0, objects.Length)], new Vector3(randX, calculateY(randX, randZ), randZ), Quaternion.identity);
                obj.transform.parent = transform;
                obj.transform.Rotate(Vector3.up, Random.Range(-90f, 90f));
                obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            }
        }
    }

    float calculateY(float x, float z)
    {
        return -(Mathf.Cos((Mathf.PI * x) / (2 * xMax)) * Mathf.Cos((Mathf.PI * z) / (2 * zMax)) - 1) * vertScale;
    }

}
