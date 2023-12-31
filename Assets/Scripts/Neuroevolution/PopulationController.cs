using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    // Population variables
    [SerializeField] GameObject PlanePrefab;
    [SerializeField] Mesh[] PlaneMeshes;
    [SerializeField] Mesh[] PlanePropellerMeshes;

    public int populationSize;
    [SerializeField] float mutationRate;
    [SerializeField] float spacing;

    public List<PlaneController> population = new List<PlaneController>();

    float maxAliveFitness = 0;
    float _maxFitness = 0f;

    public void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        for (int i = 0; i < populationSize; i++)
        {
            // Spawning a new plane
            Vector3 posOffset = new Vector3(-Mathf.Abs(i - (populationSize/2) + 0.5f), 0f, i - (populationSize / 2) + 0.5f) * spacing;
            GameObject PlaneObj = Instantiate(PlanePrefab, transform.position + posOffset, Quaternion.identity);
            PlaneObj.transform.parent = transform;
            PlaneController planeController = PlaneObj.GetComponent<PlaneController>();
            population.Add(planeController);

            // Selecting random mesh for different colour planes
            int randomPlaneIndex = Random.Range(0, PlaneMeshes.Length);
            PlaneObj.GetComponent<MeshFilter>().mesh = PlaneMeshes[randomPlaneIndex];
            PlaneObj.GetComponent<MeshCollider>().sharedMesh = PlaneMeshes[randomPlaneIndex];
            
            planeController.propeller.GetComponent<MeshFilter>().mesh = PlanePropellerMeshes[randomPlaneIndex];
            planeController.propeller.GetComponent<MeshCollider>().sharedMesh = PlanePropellerMeshes[randomPlaneIndex];
        }
    }

    public int bestAgent
    {
        get
        {
            maxAliveFitness = 0;
            int bestAgent = 0;

            for (int i = 0; i < population.Count; i++)
            {
                float fitness = population[i].fitness();
                bool isAlive = population[i].isAlive;

                if (fitness > maxAliveFitness && isAlive)
                {
                    maxAliveFitness = fitness;
                    bestAgent = i;
                }
            }

            return bestAgent;
        }
    }

    private float maxFitness()
    {
        float highest = 0f;

        foreach (PlaneController plane in population)
        {
            highest = Mathf.Max(highest, plane.fitness());
        }

        return highest;
    }

    public int numAlive
    {
        get
        {
            int num = 0;
            foreach (PlaneController Plane in population)
            {
                num += System.Convert.ToInt32(Plane.isAlive);
            }

            return num;
        }
    }

    public int numStopped
    {
        get
        {
            int num = 0;
            foreach (PlaneController Plane in population)
            {
                num += System.Convert.ToInt32(Plane.isStopped);
            }

            return num;
        }
    }


    public void Evaluate()
    {
        _maxFitness = maxFitness();

        for (int i = 0; i < populationSize; i++)
        {
            int indexA = parentIndex();
            int indexB = parentIndex();

            while (indexB == indexA) indexB = parentIndex();

            population[i].nn.Crossover(population[indexA].nn, population[indexB].nn, mutationRate);
            population[i].ResetToStart();
        }

        foreach (PlaneController Plane in population)
        {
            Plane.nn.ApplyMatrices();
        }
    }

    private int parentIndex()
    {
        int index = Random.Range(0, populationSize);
        while (Random.Range(0f, 1f) > population[index].fitness() / _maxFitness)
        {
            index = Random.Range(0, population.Count);
        }

        return index;
    }
}
