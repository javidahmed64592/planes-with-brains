using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    // Camera variables
    [SerializeField] Transform camTransform;

    [SerializeField] Vector3 offset;
    [SerializeField] float smoothStep = 10f;

    // Population variables
    [SerializeField] GameObject[] PlanePrefabs;

    public int populationSize;
    [SerializeField] float mutationRate;

    List<PlaneController> population = new List<PlaneController>();

    float maxAliveFitness = 0;
    float _maxFitness = 0f;

    public void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        for (int _ = 0; _ < populationSize; _++)
        {
            GameObject Plane = Instantiate(PlanePrefabs[Random.Range(0, PlanePrefabs.Length)], transform.position, Quaternion.identity);
            Plane.transform.parent = transform;
            population.Add(Plane.GetComponent<PlaneController>());
        }
    }

    public int bestAgent()
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

    private void FixedUpdate()
    {
        // Target
        Transform target = population[bestAgent()].transform;

        // Update position and rotation
        Vector3 targetPosition = target.position + offset;
        camTransform.SetPositionAndRotation(Vector3.Lerp(camTransform.position, targetPosition, 1 / smoothStep),
            Quaternion.Slerp(camTransform.rotation, Quaternion.LookRotation(target.position - camTransform.position), 1 / smoothStep));
    }
}
