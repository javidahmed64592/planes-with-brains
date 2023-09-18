using System.Collections;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    // Population variables
    [SerializeField] PopulationController population;

    // Genetic Algorithm
    int count = 0;
    [SerializeField] int maxCount = 100;

    int generation = 1;

    // UI
    [SerializeField] TextMeshProUGUI hudText;

    private void Start()
    {
        StartCoroutine("Running", 1f);
    }

    private void Update()
    {
        hudText.text = "Generation: " + generation
            + "\nCount: " + count + "/" + maxCount
            + "\n\nPlanes stopped: " + population.numStopped + "/" + population.populationSize
            + "\nPlanes alive: " + population.numAlive + "/" + population.populationSize
            + "\nPlanes dead: " + (population.populationSize - population.numAlive) + "/" + population.populationSize;
    }

    private IEnumerator Running()
    {
        while (true)
        {
            if (population.numAlive == 0 && count != 0 || count == maxCount)
            {
                ResetAll();
            }
            else
            {
                count++;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }


    private void ResetAll()
    {
        population.Evaluate();
        count = 0;
        generation++;
    }
}
