using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public int[] layerNodes;
    public Matrix[] weights;
    public Matrix[] biases;
    public Matrix[] newWeights;
    public Matrix[] newBiases;

    [SerializeField] float[] weightsRange = new float[2] { -1f, 1f };
    [SerializeField] float[] biasRange = new float[2] { -0.4f, 0.4f };

    private void Awake()
    {
        weights = new Matrix[layerNodes.Length - 1];
        biases = new Matrix[layerNodes.Length - 1];
        newWeights = new Matrix[layerNodes.Length - 1];
        newBiases = new Matrix[layerNodes.Length - 1];

        for (int index = 0; index < layerNodes.Length - 1; index++)
        {
            weights[index] = Matrix.randomMatrix(layerNodes[index + 1], layerNodes[index], weightsRange[0], weightsRange[1]);
            biases[index] = Matrix.randomMatrix(layerNodes[index + 1], 1, biasRange[0], biasRange[1]);
        }
    }

    public float[] feedforward(float[] inputArray)
    {
        Matrix data = Matrix.fromArray(inputArray);

        for (int index = 0; index < weights.Length; index++)
        {
            data = (weights[index] * data) + biases[index];
            if (index == 0) data = Matrix.map(data, linear);
            else if (index == weights.Length - 1) data = Matrix.map(data, sigmoid);
            else data = Matrix.map(data, linear);
        }

        return Matrix.toArray(data);
    }

    public void Crossover(NeuralNetwork NN, NeuralNetwork otherNN, float mutationRate)
    {
        for (int index = 0; index < weights.Length; index++)
        {
            newWeights[index] = Matrix.crossover(NN.weights[index], otherNN.weights[index], mutationRate, weightsRange[0], weightsRange[1]);
            newBiases[index] = Matrix.crossover(NN.biases[index], otherNN.biases[index], mutationRate, biasRange[0], biasRange[1]);
        }
    }

    public void ApplyMatrices()
    {
        for (int index = 0; index < weights.Length; index++)
        {
            weights[index] = newWeights[index];
            biases[index] = newBiases[index];
        }
    }

    // Activation functions
    private float sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    private float tanh(float x)
    {
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
    }

    private float relu(float x)
    {
        return Mathf.Max(0f, x);
    }

    private float linear(float x)
    {
        return x;
    }
}
