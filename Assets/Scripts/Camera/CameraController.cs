using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera variables
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothStep = 10f;

    // Population variables
    PopulationController population;
    Transform target;

    private void Awake()
    {
        population = GameObject.FindGameObjectWithTag("Population").GetComponent<PopulationController>();
    }

    private void Update()
    {
        if (population)
        {
            // Target
            target = population.population[population.bestAgent].transform;
        }
    }

    private void FixedUpdate()
    {
        if (target)
        {
            // Update position and rotation
            Vector3 targetPosition = target.position + offset;
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, targetPosition, 1 / smoothStep),
                Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), 1 / smoothStep));
        }
    }
}
