using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using TMPro;
using UnityEditor;
using System.Linq;


public class SpeciesManager : MonoBehaviour
{
    #region Variables

    public string n;
    public int num;
    [Header("Network Settings")]
    public int[] layers = new int[3] { 5, 3, 2 };
    public GameObject botPrefab;
    public int popSize;
    [Range(0, 1)]
    public float epWeight;
    public Plane.fitnessMode mode;
    
    [Range(0.0001f, 1f)] public float mutationChance = 0.1f;
    [Range(0f, 1f)] public float mutationStrength = 0.5f;

    public bool loadFromFile = false;
    public bool useWind;

    [HeaderAttribute("Material Settings")]
    public bool codedMaterials;
    public Material bestMat;
    public Material goodMat;
    public Material badMat;
    public Material textColor;
    public Color lineColor;
    Material[] goodMats;

    public List<NeuralNetwork> networks;
    public string[] oldPositions;
    string[] newPositions;
    public List<Plane> cars;
    int planeNum;
    [HeaderAttribute("Statistics")]
    int generationNumber = 0;
    public float bestFit;
    public float bestFitChange;
    public float avgTopFit;
    public float avgTopFitChange;
    public float avgAllFit;
    public int positionChanges;
    [Tooltip("The number of newly mutated networks that made it into the top 50% this round")]
    public int goGetters;
    float lastSpawnTime;
    public int livingBots;

    #endregion

    void Start(){
        if (popSize % 2 != 0){
            popSize++;
        }

        FindObjectOfType<GenusManager>().GenFinish += OnGenFinish;

        oldPositions = new string[popSize/2];
        newPositions = new string[popSize/2];
        InitializeNetworks();

        generationNumber = 0;

        File.Delete("Assets/fitnessRanks.txt");
        
        int matsToMake = popSize/2-1;
        goodMats = new Material[matsToMake];

        if (codedMaterials){
        for (int i = 0; i < goodMats.Length; i++)
            {
                Color matColor = goodMat.color;
                goodMats[i] = new Material (goodMat);
                
                float colorOffset = (i*125/(float)matsToMake);
                
                goodMats[i].color = new Color ((float)100/255, (50f+colorOffset)/255, (float)254/255, 1);
                goodMats[i].name = "goodMat_" + i;
            }
        }

        CreateBots();
    }

    public void InitializeNetworks(){
        networks = new List<NeuralNetwork>();
        for (int i = 0; i < popSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers, num + "." + i);
            net.rank = 1;
            if (loadFromFile){
                net.Load("Assets/Save.txt");
            }
            networks.Add(net);
        }
        
    }

    #if (UNITY_EDITOR)
        void OnDrawGizmos(){ //remove before build
            Gizmos.color = lineColor;
            Gizmos.DrawLine(new Vector3 (bestFit, -5.08f, 2), new Vector3 (bestFit, 5.08f, 2));
        }
    #endif

    void CreateBots(){
        lastSpawnTime = Time.time;
        if (cars != null){
            for (int i = 0; i < cars.Count; i++)
            {
                if(cars[i] != null){
                    //cars[i].UpdateFitness();
                    GameObject.Destroy(cars[i].gameObject);
                }
            }
            SortNetworks();
        }
        livingBots = 0;
        cars = new List<Plane>();
        
        for (int i = 0; i < popSize; i++)
        {
            
            Plane car = (Instantiate(botPrefab, new Vector3(0, 0, 2), new Quaternion(0, 0, -0.7f, 0.7f))).GetComponent<Plane>();
            car.genNum = generationNumber;
            car.spawner = GetComponent<SpeciesManager>();
            car.epWeight = epWeight;
            car.cpWeight = 1 - epWeight;
            car.mode = mode;
            

            planeNum++;
            livingBots++;

            if(car == null){
                Debug.Log("no plane script found on instantiated object");
            }
            car.network = networks[i];
            car.name = "plane_"+ n + planeNum + "(n" + car.network.n + ")";
            
            if (codedMaterials){
                if(i<popSize/2){
                    car.gameObject.GetComponent<Renderer>().material = badMat;
                } else if (i == popSize-1){
                    car.gameObject.GetComponent<Renderer>().material = bestMat;
                } else {
                    if(i-25<goodMats.Length){
                        car.gameObject.GetComponent<Renderer>().material = goodMats[(i-25)];
                    } else{
                    }
                }
            } else {
                if(i<popSize/2){
                    car.gameObject.GetComponent<Renderer>().material = badMat;
                } else if (i == popSize-1){
                    car.gameObject.GetComponent<Renderer>().material = bestMat;
                } else {
                    car.gameObject.GetComponent<Renderer>().material = goodMat;
                }
            }

            cars.Add(car);
        }
    }

    public void SortNetworks(){
        positionChanges = 0;

        Debug.ClearDeveloperConsole();
        for (int i = 0; i < popSize; i++)
        {
            if (i >= cars.Count){
                Debug.Log("i: " + i + "greater than cars.count: " + cars.Count);
                return;
            }
            if (cars[i] != null){
                cars[i].UpdateFitness();
            }
        }

        //sorts with best algorithms having higher i values and thus being at the end of the array
        networks.Sort();
        float a = 0;

        for (int i = popSize; i > popSize/2; i--)
        {
            newPositions[i-(popSize/2+1)] = networks[i-1].n;
        }
        goGetters = popSize/2;
        for (int i = 0; i < newPositions.Length; i++)
        {
            if (oldPositions.Contains(newPositions[i])){
                goGetters--;
            }
        }

        for (int i = popSize; i > popSize/2; i--)
        {
            a = a + networks[i-1].fitness;
            if (oldPositions[i-(popSize/2+1)] != networks[i-1].n) {
                oldPositions[i-(popSize/2+1)] = networks[i-1].n;
                positionChanges++;
            }
        }
        float j = a/(popSize/2);
        avgTopFitChange = j - avgTopFit;
        avgTopFit = j;

        float b = 0;
        for (int i = 0; i < popSize-1; i++)
        {
            b = b + networks[i].fitness;
        }

        avgAllFit = b/popSize;

        for (int i = 0; i < networks.Count-1; i++)
        {
            if (i>networks.Count-6){
                //Debug.Log(networks[i].fitness); 
            }
            networks[i].rank = networks.Count-i;
        }
        networks[popSize - 1].Save("Assets/Save.txt");//saves most effective network to file to preserve progress
        
        float k = networks[popSize - 1].fitness;
        bestFitChange = k - bestFit;
        bestFit = k;

        for (int i = 0; i < popSize/2; i++)
        {
            networks[i] = networks[i + popSize/2].Copy(new NeuralNetwork(layers, networks[i].n));
            networks[i].Mutate((int)(1/mutationChance), mutationStrength);
        }

        generationNumber++;
    }

    void OnGenFinish(){
        CreateBots();
    }

    public void KillAll(){
        if (cars != null){
            for (int i = 0; i < cars.Count; i++)
            {
                if(cars[i] != null){
                    Destroy(cars[i].GetComponent<Rigidbody2D>());
                }
            }
        }
    }

    public void ResetVariables(){
        int[] layers = new int[3] { 5, 3, 2 };
        FindObjectOfType<GenusManager>().UpdateManagers();
        popSize = 60/FindObjectOfType<GenusManager>().managers.Length;
        epWeight = 0f;
        loadFromFile = false;
        codedMaterials = false;
    }
}