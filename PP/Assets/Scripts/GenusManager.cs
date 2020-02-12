using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;

public class GenusManager : MonoBehaviour
{
    // Start is called before the first frame update
    public SpeciesManager[] managers;
    public float timeframe;
    public int maxGens;
    float lastSpawnTime;
    public event Action GenFinish;
    [Range(0.1f, 20f)] public float gameSpeed = 1f;
    public bool demoMode;
    public int generationNumber = 0;
    public bool PC;
    Plane b;
    public Material pcMAT;
    public int livingBots;
    public GameObject numText;
    public GameObject numTextParent;
    public GameObject bot;
    int totalBots;
    

    [Header("Objects")]
    public TextMeshProUGUI genSign;
    public TextMeshProUGUI botNumSign;
    public Slider timeSlider;
    public Slider genSlider;
    public Slider botNumSlider;
    public SpeciesManager[] m;
    public TextMeshProUGUI[] topFitnessSigns;
    public TextMeshProUGUI[] avgFitnessSigns;
    public CameraFollow cam;
    [Tooltip("The location of the generation UI text")]
    public Transform genLoc;
    public Bound chaser;
    const string letters = "abcdefghijklmnopqrstuvwxyz";



    void Awake(){
        generationNumber = 1;
        UpdateUI();
        UpdateManagers();
        for (int i = 0; i < managers.Length; i++)
        {
            GameObject f = Instantiate(numText, new Vector3 (0, 0, 0), new Quaternion (0, 0, 0, 0), numTextParent.transform);
            FollowPlane j = f.GetComponent<FollowPlane>();
            j.man = managers[i];
            if (j.man.popSize == 0){
                j.man.popSize = 60/managers.Length;
            }
            f.name = "TextHolder" + letters[i];
            j.numToFollow = j.man.popSize-1;
            j.enabled = true;
        }
    }

    void Update()
    {
        //bool check = b.GetComponent<Rigidbody2D>() == null;
        timeSlider.value = (Time.time-lastSpawnTime)/timeframe;
        botNumSign.text = "living bots:" + livingBots;
        botNumSlider.value = (float)livingBots/(float)totalBots;

        if ((CheckManagers()) || (Time.time-lastSpawnTime >= timeframe)){
            if (generationNumber < maxGens || maxGens == 0){
                if (GenFinish != null){
                FinishGen();
                } else {
                    Debug.LogError("nobody subscribed to genfinish");
                }
                UpdateUI();
            } else {
                Debug.Log("designated gens finished");
            }
        }

        Time.timeScale = gameSpeed;

        int j = 0;
        for (int i = 0; i < managers.Length; i++)
        {
            j += managers[i].livingBots;
        }
        livingBots = j;
    }

    public void UpdateUI(){
        if (maxGens != 0){
            genSlider.gameObject.SetActive(true);
            genSlider.value = (float)generationNumber/(float)maxGens;
            genSign.text = "gen #"+generationNumber + "/" + maxGens;
        } else {
            genSlider.gameObject.SetActive(false);
            if (generationNumber <= 5){
                genSign.text = "gen #"+generationNumber + "/∞";
            } else {
                genSign.text = "gen #"+generationNumber;
            }
            
        }
        
        UpdateManagerRankingUI();
    }

    public void UpdateManagerRankingUI(){
        m = managers;

        m = m.OrderBy(x => x.bestFit).Reverse().ToArray();
        
        for (int i = 0; i < topFitnessSigns.Length; i++)
        {
            if (i < m.Length){
                topFitnessSigns[i].gameObject.SetActive(true);
                topFitnessSigns[i].color = m[i].goodMat.color;
            } else {
                topFitnessSigns[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < topFitnessSigns.Length; i++)
        {
            if (i < m.Length){
                topFitnessSigns[i].text = Math.Round(m[i].bestFit, 2).ToString();
            }
        }

        m = m.OrderBy(x => x.avgTopFit).Reverse().ToArray();

        for (int i = 0; i < avgFitnessSigns.Length; i++)
        {
            if (i < m.Length){
                avgFitnessSigns[i].gameObject.SetActive(true);
                avgFitnessSigns[i].color = m[i].goodMat.color;
            } else {
                avgFitnessSigns[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < avgFitnessSigns.Length; i++)
        {
            if (i < m.Length){
                avgFitnessSigns[i].text = Math.Round(m[i].avgTopFit, 2).ToString();
            }
        }
    }

    public void UpdateManagers(){
        managers = FindObjectsOfType<SpeciesManager>().OrderBy( go => go.name ).ToArray();
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].n = letters[i].ToString();
            managers[i].num = i + 1;
        }
    }

    void FinishGen(){
        totalBots = 0;

        for (int i = 0; i < managers.Length; i++)
        {
            totalBots += managers[i].popSize;
        }
        Debug.Log(totalBots);

        GenFinish();
        lastSpawnTime = Time.time;
        StartCoroutine("LateOnGenFinish");
        generationNumber++;
        if (PC && b != null){
            GameObject.Destroy(b.gameObject);
        }
    }

    IEnumerator LateOnGenFinish (){
        yield return new WaitForSeconds (.1f);
        float highestFit = 0;
        for (int i = 0; i < managers.Length; i++){
            if (managers[i].bestFit>highestFit){
                cam.objectToFollow = managers[i].cars[managers[i].popSize-1].gameObject;
                highestFit = managers[i].bestFit;
            }
        }
        if (PC){
            b = Instantiate(bot, new Vector3 (0, 0, 0), new Quaternion (0, 0, -0.7f, 0.7f)).GetComponent<Plane>();
            b.playerControlled = true;
            b.gameObject.name = "pc";
            b.gameObject.GetComponent<Renderer>().material = pcMAT;
        }
    }
    //cam.objectToFollow = car.gameObject;

    bool CheckManagers(){
        foreach (SpeciesManager man in managers)
        {
            if (man.livingBots>0){
                return false;
            }
        }
        return true;
    }

    public void KillAll(){
        FinishGen();
    }
}
