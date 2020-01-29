using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[System.Serializable]
public class NeuralNetwork : IComparable<NeuralNetwork>
{
    #region Variables
    public string n;
    public int rank;
    public int[] layers;
    private float[][] neurons;
    private float[][] biases;
    public float[][][] weights;
    private int[] activations;
    public float fitness = 0;
    #endregion
    
    public NeuralNetwork(int[] layers, string name){
        
        //makes layers array of the NN equal to that passed through the function
        this.layers = new int[layers.Length]; 
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        this.n = name;

        InitializeNeurons();
        InitializeBiases();
        InitializeWeights();
    }
    
    void InitializeNeurons(){
        List<float[]> neuronList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronList.Add(new float[layers[i]]);
        }
        neurons = neuronList.ToArray();
    }

    void InitializeBiases(){
        List<float[]> biasList =  new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-.5f, .5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }
    
    private void InitializeWeights(){
        List <float[][]> weightList = new List<float[][]>();
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-.5f, .5f);
                }
                layerWeightList.Add(neuronWeights);
            }
            weightList.Add(layerWeightList.ToArray());
        }
        weights = weightList.ToArray();
    }

    public float Activate(float value){
        return (float)Math.Tanh(value);
    }

    public float[] FeedForward (float[] inputs){
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = Activate(value + biases[i][j]);
            }
        }
        return neurons[neurons.Length - 1];
    }

    public int CompareTo(NeuralNetwork other){
        if (other == null){
            return 1;
        }
        if (fitness > other.fitness){
            return 1;
        } else if (fitness < other.fitness){
            return -1;
        } else {
            return 0;
        }
    }
    
    public void Load(string path){
        Debug.Log("loading");
        if (path == null){
            Debug.LogError("no path found");
            return;
        }
        TextReader tr = new StreamReader(path);
        int numberOfLines = (int) new FileInfo(path).Length;
        Debug.Log(numberOfLines);
        string[] listLines = new string[numberOfLines];
        //Debug.Log("3");
        int dex = 1;
        for (int i = 0; i < numberOfLines; i++)
        {
            string s = tr.ReadLine();
            Debug.Log(s);
            listLines[i] = s;
        }
        //Debug.Log("4");
        tr.Close();
        if (new FileInfo(path).Length > 0){
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    Debug.Log(listLines[dex]);
                    biases[i][j] = float.Parse(listLines[dex]);
                    dex++;
                }
            }
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = float.Parse(listLines[dex]);
                        dex++;
                    }
                }
            }
        }
    }

    public void Mutate(int chance, float val){
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5f) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f,chance)<=5) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];
                }
            }
        }
    }

    public void Save(string path){
        //Debug.Log("saving, fit:" + fitness);
        File.Create(path).Close();
        StreamWriter wrtier = new StreamWriter(path, true);
        
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                wrtier.WriteLine(biases[i][j]);
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    wrtier.WriteLine(weights[i][j][k]);
                }
            }
        }
        wrtier.Close();
        StreamWriter writer = new StreamWriter("Assets/fitnessRanks.txt", true);
        writer.WriteLine(fitness);
        writer.Close();
    }

    public NeuralNetwork Copy(NeuralNetwork nn){
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                nn.biases[i][j] = biases[i][j];
            }
        }

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    nn.weights[i][j][k] = weights [i][j][k];
                }
            }
        }
        return nn;
    }
}
